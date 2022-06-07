namespace CrazyEaters.Sandbox
{
    using Godot;
    using System;
    using CrazyEaters.Entities;
    using Godot.Collections;

    public class Chunk : StaticBody
    {
        
        public static int CHUNK_SIZE = 9;
        public static int CHUNK_END_SIZE = CHUNK_SIZE - 1;
        public static int TEXTURE_SHEET_WIDTH = 8;

        public int CHUNK_LAST_INDEX = CHUNK_SIZE - 1;
        public static float TEXTURE_TILE_SIZE = 1.0f / TEXTURE_SHEET_WIDTH;

        public Dictionary data;
        public Vector3 chunkPosition = Vector3.Zero;
        
        public World world;
        public Thread _Thread;

        public override void _Ready()
        {
            GlobalTranslate(chunkPosition * CHUNK_SIZE);
            Name = chunkPosition.ToString();
            
            data = TerrainGenerator.HabitatGround(GlobalTransform.origin);

            GenerateChunkCollider();

            //TODO: Thread to create the mesh
            _Thread = new Thread();
            _Thread.Start(this, nameof(GenerateChunkMesh));

        }

        public void Regenerate() {
            foreach (Node c in GetChildren()) {
                RemoveChild(c);
                c.QueueFree();
            }

            GenerateChunkCollider();
            GenerateChunkMesh();
        }

#region CHUNK RENDER
        public void GenerateChunkMesh() {
            if (data.Count == 0) { return; }

            SurfaceTool surfaceTool = new SurfaceTool();
            surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

            foreach (Vector3 blockPosition in data.Keys) {
                int blockId = (int) data[blockPosition];
                DrawBlockMesh(surfaceTool, blockPosition, blockId);
            }

            surfaceTool.GenerateNormals();
            surfaceTool.GenerateTangents();
            surfaceTool.Index();
            ArrayMesh arrayMesh = surfaceTool.Commit();
            MeshInstance mi = new MeshInstance();
            mi.Mesh = arrayMesh;
            mi.MaterialOverride = world.material;
            AddChild(mi);
        }

        public void DrawBlockMesh(SurfaceTool surfaceTool, Vector3 blockSubPosition, int blockId) 
        {
            if (blockId == 0) return;

            Array<Vector3> verts = CalculateBlockVerts(blockSubPosition);
            Array<Vector2> uvs = CalculateBlockUVs(blockId);
            Array<Vector2> topUvs = uvs;
            Array<Vector2> bottomUvs = uvs;

            // Bush blocks get drawn in their own special way.
            if (blockId == 27 || blockId == 28) {
                DrawBlockFace(surfaceTool, new Array<Vector3> { verts[2], verts[0], verts[7], verts[5] }, uvs);
                DrawBlockFace(surfaceTool, new Array<Vector3> { verts[7], verts[5], verts[2], verts[0] }, uvs);
                DrawBlockFace(surfaceTool, new Array<Vector3> { verts[3], verts[1], verts[6], verts[4] }, uvs);
                DrawBlockFace(surfaceTool, new Array<Vector3> { verts[6], verts[4], verts[3], verts[1] }, uvs);
                return;
            }

            // Allow some blocks to have different top/bottom textures.
            if (blockId == 3) { // Grass
                topUvs = CalculateBlockUVs(0);
                bottomUvs = CalculateBlockUVs(2);
            } else if (blockId == 5) { // Furnace
                topUvs = CalculateBlockUVs(31);
                bottomUvs = topUvs;
            } else if (blockId == 12) { // Log
                topUvs = CalculateBlockUVs(30);
                bottomUvs = topUvs;
            } else if (blockId == 19) { // bookshelf
                topUvs = CalculateBlockUVs(4);
                bottomUvs = topUvs;
            }

            // Main rendering code for normal blocks.
            Vector3 otherBlockPos = blockSubPosition + Vector3.Left;
            int otherBlockId = 0;
            if (otherBlockPos.x == -1) {
                otherBlockId = world.GetBlockGlobalPosition(otherBlockPos + chunkPosition * CHUNK_SIZE);
            } else if (data.Contains(otherBlockPos)) {
                otherBlockId = (int) data[otherBlockPos];
            }
            if (blockId != otherBlockId && IsBlockTransparent(otherBlockId)) {
                DrawBlockFace(surfaceTool, new Array<Vector3> { verts[2], verts[0], verts[3], verts[1] }, uvs);
            }

            otherBlockPos = blockSubPosition + Vector3.Right;
            otherBlockId = 0;
            if (otherBlockPos.x == CHUNK_SIZE) {
                otherBlockId = world.GetBlockGlobalPosition(otherBlockPos + chunkPosition * CHUNK_SIZE);
            } else if (data.Contains(otherBlockPos)) {
                otherBlockId = (int) data[otherBlockPos];
            }
            if (blockId != otherBlockId && IsBlockTransparent(otherBlockId)) {
                DrawBlockFace(surfaceTool, new Array<Vector3> { verts[7], verts[5], verts[6], verts[4] }, uvs);
            }

            otherBlockPos = blockSubPosition + Vector3.Forward;
            otherBlockId = 0;
            if (otherBlockPos.z == -1) {
                otherBlockId = world.GetBlockGlobalPosition(otherBlockPos + chunkPosition * CHUNK_SIZE);
            } else if (data.Contains(otherBlockPos)) {
                otherBlockId = (int) data[otherBlockPos];
            }
            if (blockId != otherBlockId && IsBlockTransparent(otherBlockId)) {
                DrawBlockFace(surfaceTool, new Array<Vector3> { verts[6], verts[4], verts[2], verts[0] }, uvs);
            }

            otherBlockPos = blockSubPosition + Vector3.Back;
            otherBlockId = 0;
            if (otherBlockPos.z == CHUNK_SIZE) {
                otherBlockId = world.GetBlockGlobalPosition(otherBlockPos + chunkPosition * CHUNK_SIZE);
            } else if (data.Contains(otherBlockPos)) {
                otherBlockId = (int) data[otherBlockPos];
            }
            if (blockId != otherBlockId && IsBlockTransparent(otherBlockId)) {
                DrawBlockFace(surfaceTool, new Array<Vector3> { verts[3], verts[1], verts[7], verts[5] }, uvs);
            }

            otherBlockPos = blockSubPosition + Vector3.Down;
            otherBlockId = 0;
            if (otherBlockPos.y == -1) {
                otherBlockId = world.GetBlockGlobalPosition(otherBlockPos + chunkPosition * CHUNK_SIZE);
            } else if (data.Contains(otherBlockPos)) {
                otherBlockId = (int) data[otherBlockPos];
            }
            if (blockId != otherBlockId && IsBlockTransparent(otherBlockId)) {
                DrawBlockFace(surfaceTool, new Array<Vector3> { verts[4], verts[5], verts[0], verts[1] }, bottomUvs);
            }

            otherBlockPos = blockSubPosition + Vector3.Up;
            otherBlockId = 0;
            if (otherBlockPos.y == CHUNK_SIZE) {
                otherBlockId = world.GetBlockGlobalPosition(otherBlockPos + chunkPosition * CHUNK_SIZE);
            } else if (data.Contains(otherBlockPos)) {
                otherBlockId = (int) data[otherBlockPos];
            }
            if (blockId != otherBlockId && IsBlockTransparent(otherBlockId)) {
                DrawBlockFace(surfaceTool, new Array<Vector3> { verts[2], verts[3], verts[6], verts[7] }, topUvs);
            }
        }

        public void DrawBlockFace(SurfaceTool surfaceTool, Array<Vector3> verts, Array<Vector2> uvs)
        {
            surfaceTool.AddUv(uvs[1]); surfaceTool.AddVertex(verts[1]);
            surfaceTool.AddUv(uvs[2]); surfaceTool.AddVertex(verts[2]);
            surfaceTool.AddUv(uvs[3]); surfaceTool.AddVertex(verts[3]);
            
            surfaceTool.AddUv(uvs[2]); surfaceTool.AddVertex(verts[2]);
            surfaceTool.AddUv(uvs[1]); surfaceTool.AddVertex(verts[1]);
            surfaceTool.AddUv(uvs[0]); surfaceTool.AddVertex(verts[0]);
        }

        public static Array<Vector3> CalculateBlockVerts(Vector3 blockPosition) 
        {
            return new Array<Vector3> {
                new Vector3(blockPosition.x, blockPosition.y, blockPosition.z),
                new Vector3(blockPosition.x, blockPosition.y, blockPosition.z + 1),
                new Vector3(blockPosition.x, blockPosition.y + 1, blockPosition.z),
                new Vector3(blockPosition.x, blockPosition.y + 1, blockPosition.z + 1),
                new Vector3(blockPosition.x + 1, blockPosition.y, blockPosition.z),
                new Vector3(blockPosition.x + 1, blockPosition.y, blockPosition.z + 1),
                new Vector3(blockPosition.x + 1, blockPosition.y + 1, blockPosition.z),
                new Vector3(blockPosition.x + 1, blockPosition.y + 1, blockPosition.z + 1),
            };
        }
        
        public static Array<Vector2> CalculateBlockUVs(int blockId) 
        {
            int row = blockId / TEXTURE_SHEET_WIDTH;
            int col = blockId % TEXTURE_SHEET_WIDTH;

            return new Array<Vector2> {
                TEXTURE_TILE_SIZE * new Vector2(col, row),
                TEXTURE_TILE_SIZE * new Vector2(col, row + 1),
                TEXTURE_TILE_SIZE * new Vector2(col + 1, row),
                TEXTURE_TILE_SIZE * new Vector2(col + 1, row + 1)
            };
        }

        public static bool IsBlockTransparent(int blockId)
        {
            return blockId == 0 || (blockId > 25 && blockId < 30);
        }
#endregion

#region CHUNK PHYSICS
        public void GenerateChunkCollider() {
            if (data.Count == 0) {
                QueueFree();
                return;
            }

            CollisionLayer = 0xFFFFF;
            CollisionMask = 0xFFFFF;

            foreach (Vector3 blockPosition in data.Keys) {
                int blockId = (int) data[blockPosition];
                if (blockId != 27 && blockId != 28 && blockId != 0) {
                    CreateBlockCollider(blockPosition);
                }
            }
        }

        public void CreateBlockCollider(Vector3 blockSubPosition) 
        {
            CollisionShape collider = new CollisionShape();
            BoxShape shape =  new BoxShape();
            shape.Extents = Vector3.One / 2;
            collider.Shape = shape;
            collider.GlobalTranslate(blockSubPosition + Vector3.One / 2);
            AddChild(collider);
        }
#endregion

    }
}
