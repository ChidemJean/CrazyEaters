namespace CrazyEaters.Controllers
{
    using Godot;
    using System;
    using CrazyEaters.Managers;
    using CrazyEaters.Entities;

    public class PlacementController : CSGCombiner
    {
        [Export]
        public bool inEditMode = false;

        [Export]
        public bool removeBlockFlag = false;

        [Export]
        public PackedScene blockPref;

        [Export]
        public PackedScene dustParticlesPref = null;

        [Export]
        public NodePath scenePath = null;

        [Signal]
        public delegate void OnChangeEditMode(bool editMode);

        private GameManager gameManager;

        private Block currentBlock = null;
        private Spatial scene = null;

        public override void _Ready()
        {
            gameManager = GetNode<GameManager>("/root/GameManager");
            scene = GetNode<Spatial>(scenePath);
        }

        public override void _Input(InputEvent @event)
        {
            if (inEditMode) {
                if (@event is InputEventMouseMotion) {
                    InputEventMouseMotion _event = (InputEventMouseMotion) @event;
                    Vector2 mousePos = _event.Position * gameManager.hud.currentScale;
                    MoveBlock(mousePos);
                    return;
                }
                if (@event is InputEventMouseButton) {
                    InputEventMouseButton _event = (InputEventMouseButton) @event;
                    Vector2 mousePos = _event.Position * gameManager.hud.currentScale;

                    if (_event.ButtonIndex == 1 && _event.IsPressed()) {
                        if (removeBlockFlag) {
                            RemoveBlock(mousePos);
                        } else {
                            PlaceBlock(mousePos);
                        }
                    }
                    return;
                }
            }
        }

        private void MoveBlock(Vector2 mousePos) 
        {
            if (removeBlockFlag) return;

            var intersect = ProjectRay(mousePos);

            if (intersect != null && intersect.Count > 0) {
                Vector3 pos = (Vector3) intersect["position"];
                CollisionObject collider = (CollisionObject) intersect["collider"];
                Spatial target = collider.GetParentOrNull<Spatial>();
                
                if (target != null && target is Block) {
                    currentBlock.Translation = GetSnappedPos(pos, currentBlock, target as Block);
                } else {
                    currentBlock.Translation = GetSnappedPos(pos, currentBlock, null);
                }
            }
        }

        private void RemoveBlock(Vector2 mousePos) 
        {
            var intersect = ProjectRay(mousePos);

            if (intersect != null && intersect.Count > 0) {
                CollisionObject collider = (CollisionObject) intersect["collider"];
                CSGBox box = collider.GetParentOrNull<CSGBox>();
                if (box != null && !box.Name.ToLower().Contains("floor")) {
                    box.QueueFree();
                }
            }
        }

        private void PlaceBlock(Vector2 mousePos) 
        {
            var intersect = ProjectRay(mousePos);
            if (intersect != null && intersect.Count > 0) {
                Vector3 pos = (Vector3) intersect["position"];
                Spatial newBlock = InstanceNewBlock(pos);
                newBlock.GetNode<StaticBody>("StaticBody").CollisionLayer = 2^1;
                if (dustParticlesPref != null) {
                    Particles dustParticles = dustParticlesPref.Instance<Particles>();
                    dustParticles.Translation = pos;
                    scene.AddChild(dustParticles);
                    dustParticles.Emitting = true;
                }
            }
        }

        private Godot.Collections.Dictionary ProjectRay(Vector2 mousePos)
        {
            PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;
            Vector3 rayOrigin = gameManager.camera.ProjectRayOrigin(mousePos);
            Vector3 rayEnd = rayOrigin + gameManager.camera.ProjectRayNormal(mousePos) * 2000;
            return spaceState.IntersectRay(rayOrigin, rayEnd, null, 2^1);
        }

        private Block InstanceNewBlock(Vector3 pos) 
        {
            Block newBlock = (Block) blockPref.Instance();
            newBlock.Translation = GetSnappedPos(pos, newBlock);
            AddChild(newBlock);
            return newBlock;
        }

        public Vector3 GetSnappedPos(Vector3 pos, Block block, Block targetBlock = null)
        {
            if (targetBlock != null) {
                Vector3 dir = (pos - targetBlock.GlobalTransform.origin).Normalized();
                GD.Print("DIR BLOCK: " + dir);
            }

            Vector3 blockGrid = block.data.gridSize;

            float x = Mathf.Floor(pos.x);
            float y = Mathf.Floor(pos.y);
            float z = Mathf.Floor(pos.z);

            x = x % 2 == 0 ? x : x + 1 * blockGrid.x;
            y = y % 2 == 0 ? y : y + 1 * blockGrid.y;
            z = z % 2 == 0 ? z : z + 1 * blockGrid.z;

            return new Vector3(x, y, z);
        }

        public void ChangeEditMode(bool editMode)
        {
            inEditMode = editMode;
            if (inEditMode) {
                currentBlock = InstanceNewBlock(Vector3.Zero);
            } else {
                currentBlock.QueueFree();
            }
            EmitSignal(nameof(OnChangeEditMode), inEditMode);
        }

        public void ChangeRemoveBlockFlag(bool removeBlockFlag)
        {
            this.removeBlockFlag = removeBlockFlag;
            if (this.removeBlockFlag) {
                currentBlock.QueueFree();
            } else {
                ChangeEditMode(true);
            }
        }

    }
}