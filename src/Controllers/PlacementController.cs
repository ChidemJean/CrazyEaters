namespace CrazyEaters.Controllers
{
    using Godot;
    using System;
    using CrazyEaters.Managers;
    using CrazyEaters.Entities;
    using CrazyEaters.Resources;
    using CrazyEaters.DependencyInjection;

    public class PlacementController : Spatial
    {

        [Export]
        public bool removeBlockFlag = false;

        [Export]
        public PackedScene blockPref;

        [Export]
        public PackedScene dustParticlesPref = null;

        [Signal]
        public delegate void OnChangeEditMode(bool editMode);

        [Inject]
        private GameManager gameManager;
        [Inject]
        private SceneSwitcher sceneSwitcher;
        private Block currentBlock = null;

        private HabitatScene scene = null;
        public float blockSize = 2;

        [Export(PropertyHint.Layers3dPhysics)]
        public uint rayCollisionMask;

        public override void _Ready()
        {
            this.ResolveDependencies();
            scene = (HabitatScene) sceneSwitcher.currentScene;
        }

        public override void _Input(InputEvent @event)
        {
            if (gameManager.inputMode == GameManager.InputMode.SCENE) {
                if (scene.inEditMode) {
                    // if (@event is InputEventMouseMotion) {
                    //     InputEventMouseMotion _event = (InputEventMouseMotion) @event;
                    //     Vector2 mousePos = _event.Position * gameManager.hud.currentScale;
                    //     MoveBlock(mousePos);
                    //     return;
                    // }
                    if (@event is InputEventMouseButton) {
                        InputEventMouseButton _event = (InputEventMouseButton) @event;
                        Vector2 mousePos = _event.Position * gameManager.hud.currentScale;

                        if (_event.ButtonIndex == 1 && !_event.IsPressed()) {
                            if (removeBlockFlag) {
                                RemoveBlock(mousePos);
                            } else if (currentBlock != null) {
                                MoveBlock(mousePos);
                            } else {
                                PlaceBlock(mousePos);
                            }
                            return;
                        }

                        // if (_event.IsPressed()) {
                        //     if (_event.ButtonIndex == (int) ButtonList.WheelUp) {
                        //         currentBlockId ++;
                        //     }
                        //     if (_event.ButtonIndex == (int) ButtonList.WheelDown) {
                        //         currentBlockId --;
                        //     }
                        // }
                        return;
                    }
                }
            }
        }

        private void MoveBlock(Vector2 mousePos) 
        {
            if (removeBlockFlag) return;

            var intersect = ProjectRay(mousePos);

            if (intersect != null && intersect.Count > 0) {
                Vector3 pos = (Vector3) intersect["position"];
                Vector3 normal = (Vector3) intersect["normal"];

                Vector3 blockGlobalPos = (pos + normal / 2).Floor();
                BlockData blockData = scene.RetrieveBlockDataFromId(scene.currentBlockId);
                currentBlock.Translation = gameManager.world.GetProjectedBlockGlobalPosition(blockGlobalPos);
                bool canPlace = gameManager.world.CheckMultiBlocksFree(blockGlobalPos, blockData.gridSize, Vector3.Up);
                currentBlock.UpdateOverlap(canPlace);
            }
        }

        private void RemoveBlock(Vector2 mousePos) 
        {
            var intersect = ProjectRay(mousePos);

            if (intersect != null && intersect.Count > 0) {
                // CollisionObject collider = (CollisionObject) intersect["collider"];
                Vector3 pos = (Vector3) intersect["position"];
                Vector3 normal = (Vector3) intersect["normal"];

                Vector3 blockGlobalPos = (pos - normal / 2).Floor();
                gameManager.world.SetBlockGlobalPosition(blockGlobalPos, 0);

                return;
                // Block box = collider.GetParentOrNull<Block>();
                // if (box != null && !box.Name.ToLower().Contains("floor")) {
                //     box.QueueFree();
                // }
            }
        }

        private void PlaceBlock(Vector2 mousePos) 
        {
            var intersect = ProjectRay(mousePos);
            if (intersect != null && intersect.Count > 0) {
                Vector3 pos = (Vector3) intersect["position"];
                Vector3 normal = (Vector3) intersect["normal"];

                int currentBlockId = scene.currentBlockId;
                Vector3 blockGlobalPos = (pos + normal / 2).Floor();
                BlockData blockData = scene.RetrieveBlockDataFromId(currentBlockId);

                if (blockData.blockType == BlockData.BlockType.Group) {
                    gameManager.world.SetBlockGlobalPosition(blockGlobalPos, currentBlockId);
                } else {
                    currentBlock = blockData.prefab.Instance<Block>();
                    AddChild(currentBlock);
                    currentBlock.Translate(gameManager.world.GetProjectedBlockGlobalPosition(blockGlobalPos));
                    bool canPlace = gameManager.world.CheckMultiBlocksFree(blockGlobalPos, blockData.gridSize, Vector3.Up);
                    currentBlock.UpdateOverlap(canPlace);
                }

                return;
                //
                CollisionObject collider = (CollisionObject) intersect["collider"];
                Block targetBlock = collider.GetParentOrNull<Block>();
                pos = GetSnappedPos(pos, normal, currentBlock, targetBlock);
                //
                if (!currentBlock.overlaping) {
                    Block newBlock = InstanceNewBlock(pos);
                    newBlock.SetPlaced(true);
                    newBlock.GetNode<StaticBody>("StaticBody").CollisionLayer = 2^1;
                    if (dustParticlesPref != null) {
                        Particles dustParticles = dustParticlesPref.Instance<Particles>();
                        dustParticles.Translation = pos;
                        scene.AddChild(dustParticles);
                        dustParticles.Emitting = true;
                    }
                }
            }
        }

        private Godot.Collections.Dictionary ProjectRay(Vector2 mousePos)
        {
            PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;
            Vector3 rayOrigin = ((HabitatScene)sceneSwitcher.currentScene).camera.ProjectRayOrigin(mousePos);
            Vector3 rayEnd = rayOrigin + ((HabitatScene)sceneSwitcher.currentScene).camera.ProjectRayNormal(mousePos) * 2000;
            return spaceState.IntersectRay(rayOrigin, rayEnd, null, rayCollisionMask);
        }

        private Block InstanceNewBlock(Vector3 pos) 
        {
            Block newBlock = (Block) blockPref.Instance();
            newBlock.Translation = pos;
            AddChild(newBlock);
            return newBlock;
        }

        public Vector3 GetSnappedPos(Vector3 pos, Vector3 normal, Block block, Block targetBlock = null)
        {
            Vector3 blockGrid = block.data.gridSize;

            if (targetBlock != null) {
                Vector3 targetCenter = targetBlock.GlobalTransform.origin;
                Vector3 blockGridTarget = targetBlock.data.gridSize;

                return targetCenter + (normal * blockGridTarget) + (normal * blockGrid);
            }

            float x = Mathf.Round(pos.x);
            float y = Mathf.Round(pos.y);
            float z = Mathf.Round(pos.z);

            x = x % blockSize == 0 ? x : x + 1 * blockGrid.x;
            y = y % blockSize == 0 ? y : y + 1 * blockGrid.y;
            z = z % blockSize == 0 ? z : z + 1 * blockGrid.z;

            return new Vector3(x, y, z);
        }

        public void ChangeEditMode(bool editMode)
        {
            scene.inEditMode = editMode;
            if (!scene.inEditMode) {
                if (currentBlock != null && IsInstanceValid(currentBlock)) currentBlock.QueueFree();
            }
            EmitSignal(nameof(OnChangeEditMode), scene.inEditMode);
        }

        public void ChangeRemoveBlockFlag(bool removeBlockFlag)
        {
            this.removeBlockFlag = removeBlockFlag;
            // if (this.removeBlockFlag) {
            //     currentBlock.QueueFree();
            // } else {
                ChangeEditMode(true);
            // }
        }

    }
}