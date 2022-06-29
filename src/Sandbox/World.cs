namespace CrazyEaters.Sandbox
{
    using Godot;
    using System;
    using System.Threading.Tasks;
    using Godot.Collections;
    using CrazyEaters.Managers;
    using CrazyEaters.Save;
    using CrazyEaters.Resources;
    using CrazyEaters.DependencyInjection;

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

        [Inject] private GameManager gameManager;

        [Export]
        public PackedScene chunkLabel;

        [Export]
        public NodePath tweenPath;
        public Tween tween;
        public Dictionary chunksLoaded;

        [Export]
        public NodePath navigationPath;
        [Export]
        public NodePath btnBakePath;
        public Button btnBake;
        double initialBakingTime = 0;
        public NavigationMeshInstance navmesh;

        public Navigation navigation;
        [Export]
        public NodePath labelDirPath;
        public Label labelDir;

        [Inject] private SceneSwitcher sSwitcher = null;
        [Inject] private SaveSystemNode saveSystemNode = null;

        public override void _EnterTree()
        {
        }

        public override void _Ready()
        {
            this.ResolveDependencies();
            
            gameManager.world = this;
            
            btnBake = GetNode<Button>(btnBakePath);
            btnBake.Connect("button_up", this, nameof(OnClickBake));
            
            // IA
            navigation = GetNode<Navigation>(navigationPath);
            navmesh = navigation.GetNode<NavigationMeshInstance>("Navmesh");
            labelDir = GetNode<Label>(labelDirPath);

            navmesh.Connect("bake_finished", this, nameof(OnNavmeshChanged));
            chunks = new Dictionary();

            tween = GetNode<Tween>(tweenPath);

            sSwitcher.currentScene.Load(OnLoaded);
            GD.Print("world: ", sSwitcher.currentScene);
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventKey) {
                InputEventKey _event = (InputEventKey) @event;
                if (_event.IsPressed() && _event.Scancode == (uint) KeyList.B) {
                    OnClickBake();
                }
            }
        }

        public void OnClickBake()
        {
            BakeNavmesh();
        }

        public void BakeNavmesh()
        {
            initialBakingTime = OS.GetSystemTimeSecs();
            navmesh.BakeNavigationMesh(true);
            labelDir.Text = "Baking... ";
        }

        public void OnNavmeshChanged() 
        {
            double bakeTime = OS.GetSystemTimeSecs() - initialBakingTime;
            labelDir.Text = "Bake finished time: " + bakeTime+"s";
        }

        public void OnLoaded(GameData gameData) {
            if (gameData != null && gameData is HabitatGameData) {
                chunksLoaded = saveSystemNode.FromGameData((HabitatGameData) gameData);
            }

            // if (chunksLoaded.Count > 0) {
            //     // LoadChunks();
            // }

            _generating = true;
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
                MergeChunksLoaded();
                BakeNavmesh();
            }
        }

        public void MergeChunksLoaded()
        {
            if (chunksLoaded != null && chunksLoaded.Count > 0) {
                foreach(Vector3 chunkPosition in this.chunksLoaded.Keys) 
                {
                    Chunk chunk_Loaded = (Chunk) this.chunksLoaded[chunkPosition];
                    Dictionary blocks_Loaded = chunk_Loaded.data;
                    
                    if (this.chunks[chunkPosition] != null) {
                        Chunk chunk = (Chunk) this.chunks[chunkPosition];

                        foreach (Vector3 blockPosition in blocks_Loaded.Keys)
                        {
                            int blockLoadedId = (int) blocks_Loaded[blockPosition];
                            if (chunk.data[blockPosition] != null) {
                                chunk.data[blockPosition] = blockLoadedId;
                            }                            
                        }
                    }
                }
                RegenerateChunks();
            }
        }

        public void RegenerateChunks()
        {
            foreach(Vector3 chunkPosition in this.chunks.Keys) 
            {
                Chunk chunk = (Chunk) this.chunks[chunkPosition];
                chunk.GenerateChunk();
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

        public async void SetBlockGlobalPosition(Vector3 blockGlobalPosition, int blockId)
        {
            bool isRemoving = blockId == 0;
            if (blockGlobalPosition == null) return;

            Vector3 chunkPosition = (blockGlobalPosition / Chunk.CHUNK_SIZE).Floor();
            if (!chunks.Contains(chunkPosition)) return;

            Chunk chunk = (Chunk) chunks[chunkPosition];
            Vector3 subPosition = blockGlobalPosition.PosMod(Chunk.CHUNK_SIZE);

            if (isRemoving) {
                GD.Print(chunk.data[subPosition]);
                chunk.RemoveBlock(subPosition);
            } else {
                chunk.UpdateBlock(blockId, subPosition);
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
            } else {
                chunk.Regenerate(); // TODO: temporario, preciso remover apenas o unique block para performance
            }

            if (!isRemoving) {
                await Shockwave(blockGlobalPosition + Vector3.One / 2);
            }

            BakeNavmesh();
        }

        public async Task Shockwave(Vector3 shockwaveOrigin) 
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