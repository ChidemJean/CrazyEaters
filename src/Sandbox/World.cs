namespace CrazyEaters.Sandbox
{
    using Godot;
    using System;
    using Godot.Collections;
    using CrazyEaters.Managers;
    using CrazyEaters.Save;
    using CrazyEaters.Resources;

    public class World : Node
    {
        float effectiveRenderDistance = 0;
        bool _generating = false;
        bool _deleting = false;
        Vector3 renderPosition = Vector3.Zero;
        float renderDistance = 2;
        public Dictionary chunks;
        Vector3 chunkCenter = new Vector3(Chunk.CHUNK_SIZE / 2, Chunk.CHUNK_SIZE / 2, Chunk.CHUNK_SIZE / 2);
        [Export]
        public BlocksData blocksRefs;
        [Export]
        public ShaderMaterial shaderMaterial;
        [Export]
        public Material material;

        [Export(PropertyHint.Layers3dPhysics)]
        public uint chunkCollisionLayer;

        private GameManager gameManager;
        private SaveSystemNode saveSystemNode;

        [Export]
        public PackedScene chunkLabel;

        [Export]
        public NodePath tweenPath;
        public Tween tween;


        public override void _Ready()
        {
            chunks = new Dictionary();
            saveSystemNode = GetNode<SaveSystemNode>("/root/MainNode/SaveSystem");
            gameManager = GetNode<GameManager>("/root/GameManager");
            gameManager.world = this;
            tween = GetNode<Tween>(tweenPath);
            saveSystemNode.LoadGame(OnLoaded);

        }

        public void OnLoaded(GameData gameData) {
            if (gameData != null) {
                this.chunks = saveSystemNode.FromGameData(gameData);
            }

            if (this.chunks.Count > 0) {
                LoadChunks();
            } else {
                _generating = true;
            }
        }

        public override void _Process(float delta)
        {
            GenerateChunks();
        }

        public void LoadChunks() {
            foreach(Vector3 chunkPosition in this.chunks.Keys) 
            {
                Chunk chunk = (Chunk) this.chunks[chunkPosition];
                chunk.world = this;
                AddChild(chunk);
            }
        }

        public void GenerateChunks() {
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
                        Vector3 offsetedChunkPos = (chunkPosition * Chunk.CHUNK_SIZE) + chunkCenter;

                        float distanceFloor = Mathf.Floor(playerChunk.DistanceTo(offsetedChunkPos));
                        float scaledRenderDistance = renderDistance * Chunk.CHUNK_SIZE / 2;

                        if (distanceFloor > scaledRenderDistance) {
                            if (Mathf.Abs(offsetedChunkPos.x) < scaledRenderDistance 
                                && Mathf.Abs(offsetedChunkPos.z) < scaledRenderDistance 
                                && (offsetedChunkPos.y > scaledRenderDistance && offsetedChunkPos.y <= scaledRenderDistance * 2)) {
                                //
                            } else {
                                continue;
                            }
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
                chunk.RemoveBlockCollider(subPosition);
            } else {
                chunk.RemoveBlockCollider(subPosition);
                chunk.data[subPosition] = blockId;
                chunk.CreateBlockCollider(subPosition);
                Shockwave(blockGlobalPosition + Vector3.One / 2);
            }

            if (blockId < 60) {
                chunk.Regenerate();
                // We also might need to regenerate some neighboring chunks.
                if (Chunk.IsBlockTransparent(blockId)) {
                    if (subPosition.x == 0) {
                        Vector3 left = chunkPosition + Vector3.Left;
                        if (chunks.Contains(left)) ((Chunk) chunks[left]).Regenerate();

                    } else if (subPosition.x == Chunk.CHUNK_END_SIZE) {
                        Vector3 right = chunkPosition + Vector3.Right;
                        if (chunks.Contains(right)) ((Chunk) chunks[right]).Regenerate();

                    } else if (subPosition.z == 0) {
                        Vector3 forward = chunkPosition + Vector3.Forward;
                        if (chunks.Contains(forward)) ((Chunk) chunks[forward]).Regenerate();

                    } else if (subPosition.z == Chunk.CHUNK_END_SIZE) {
                        Vector3 backward = chunkPosition + Vector3.Back;
                        if (chunks.Contains(backward)) ((Chunk) chunks[backward]).Regenerate();

                    } else if (subPosition.y == 0) {
                        Vector3 down = chunkPosition + Vector3.Down;
                        if (chunks.Contains(down)) ((Chunk) chunks[down]).Regenerate();

                    } else if (subPosition.y == Chunk.CHUNK_END_SIZE) {
                        Vector3 up = chunkPosition + Vector3.Up;
                        if (chunks.Contains(up)) ((Chunk) chunks[up]).Regenerate();
                    }
                }
            }
        }

        public async void Shockwave(Vector3 shockwaveOrigin) 
        {
            tween.StopAll();

            foreach (Vector3 chunkP in chunks.Keys) {
                Chunk chunk = (Chunk) chunks[chunkP];
                if (chunk.mi != null) {
                    chunk.mi.MaterialOverride = shaderMaterial;
                }
            }
            //
            shaderMaterial.SetShaderParam("shockwave_origin", shockwaveOrigin);
            tween.InterpolateProperty(shaderMaterial, "shader_param/shockwave_percentage", 0, 1, .84f, Tween.TransitionType.Sine, Tween.EaseType.OutIn);
            tween.InterpolateProperty(shaderMaterial, "shader_param/shockwave_width", 4, 0, .7f, Tween.TransitionType.Sine, Tween.EaseType.OutIn);
            tween.InterpolateProperty(shaderMaterial, "shader_param/shockwave_strength", 0.415f, 0, .6f, Tween.TransitionType.Sine, Tween.EaseType.OutIn);
            tween.Start();
            await ToSignal(tween, "tween_completed");

            shaderMaterial.SetShaderParam("shockwave_percentage", 0);
            //
            foreach (Vector3 chunkP in chunks.Keys) {
                Chunk chunk = (Chunk) chunks[chunkP];
                if (chunk.mi != null) {
                    chunk.mi.MaterialOverride = material;
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