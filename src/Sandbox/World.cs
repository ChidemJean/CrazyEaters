namespace CrazyEaters.Sandbox
{
    using Godot;
    using System;
    using Godot.Collections;
    using CrazyEaters.Managers;

    public class World : Node
    {
        float effectiveRenderDistance = 0;
        bool _generating = true;
        bool _deleting = false;
        Vector3 renderPosition = Vector3.Zero;
        float renderDistance = 1;
        Dictionary chunks;

        [Export]
        public Material material;
        private GameManager gameManager;


        public override void _Ready()
        {
            chunks = new Dictionary();
            gameManager = GetNode<GameManager>("/root/GameManager");
            gameManager.world = this;
        }

        public override void _Process(float delta)
        {
            Vector3 playerChunk = (renderPosition / Chunk.CHUNK_SIZE).Round();

            if (_deleting) {
                _generating = true;
            }

            if (!_generating) return;

            var xRange = GD.Range((int)(playerChunk.x - effectiveRenderDistance), (int)(playerChunk.x + effectiveRenderDistance));
            var yRange = GD.Range((int)(playerChunk.y - effectiveRenderDistance), (int)(playerChunk.y + effectiveRenderDistance));
            var zRange = GD.Range((int)(playerChunk.z - effectiveRenderDistance), (int)(playerChunk.z + effectiveRenderDistance));

            foreach (int x in xRange) {
                foreach (int y in yRange) {
                    foreach (int z in zRange) {
                        Vector3 chunkPosition = new Vector3(x, y, z);
                        if (Mathf.Floor(playerChunk.DistanceTo(chunkPosition)) > renderDistance) {
                            continue;
                        }

                        if (chunks.Contains(chunkPosition)) {
                            continue;
                        }

                        Chunk chunk = new Chunk();
                        chunk.world = this;
                        chunk.chunkPosition = chunkPosition;
                        chunks[chunkPosition] = chunk;
                        AddChild(chunk);
                        return;
                    }
                }
            }

            if (effectiveRenderDistance < renderDistance) {
                effectiveRenderDistance += 1;
            } else {
                _generating = false;
            }

        }

        public int GetBlockGlobalPosition(Vector3 blockGlobalPosition) 
        {
            if (blockGlobalPosition == null) return 0;

            Vector3 chunkPosition = (blockGlobalPosition / Chunk.CHUNK_SIZE).Floor();
            if (chunks.Contains(chunkPosition)) {
                Chunk chunk = (Chunk) chunks[chunkPosition];
                Vector3 subPosition = blockGlobalPosition.PosMod(Chunk.CHUNK_SIZE);
                if (chunk.data.Contains(subPosition)) {
                    return (int) chunk.data[subPosition];
                }
            }

            return 0;
        }

        public void SetBlockGlobalPosition(Vector3 blockGlobalPosition, int blockId)
        {
            if (blockGlobalPosition == null) return;

            Vector3 chunkPosition = (blockGlobalPosition / Chunk.CHUNK_SIZE).Floor();
            if (!chunks.Contains(chunkPosition)) return;

            Chunk chunk = (Chunk) chunks[chunkPosition];
            Vector3 subPosition = blockGlobalPosition.PosMod(Chunk.CHUNK_SIZE);
            if (blockId == 0) {
                chunk.data.Remove(subPosition);
            } else {
                chunk.data[subPosition] = blockId;
            }

            chunk.Regenerate();

            // We also might need to regenerate some neighboring chunks.
            if (Chunk.IsBlockTransparent(blockId)) {
                if (subPosition.x == 0) {
                    ((Chunk) chunks[chunkPosition + Vector3.Left]).Regenerate();
                } else if (subPosition.x == Chunk.CHUNK_END_SIZE) {
                    ((Chunk) chunks[chunkPosition + Vector3.Right]).Regenerate();
                } else if (subPosition.z == 0) {
                    ((Chunk) chunks[chunkPosition + Vector3.Forward]).Regenerate();
                } else if (subPosition.z == Chunk.CHUNK_END_SIZE) {
                    ((Chunk) chunks[chunkPosition + Vector3.Back]).Regenerate();
                } else if (subPosition.y == 0) {
                    ((Chunk) chunks[chunkPosition + Vector3.Down]).Regenerate();
                } else if (subPosition.y == Chunk.CHUNK_END_SIZE) {
                    ((Chunk) chunks[chunkPosition + Vector3.Up]).Regenerate();
                }
            }
        }

        public void CleanUp()
        {
            foreach (Vector3 chunkPosition in chunks.Keys) {
                System.Threading.Thread thread = ((Chunk) chunks[chunkPosition])._Thread;
                if (thread != null) {
                    // thread.WaitToFinish();
                }
            }

            chunks.Clear();
            SetProcess(false);
            foreach (Node c in GetChildren()) {
                c.Free();
            }
        }

    }
}