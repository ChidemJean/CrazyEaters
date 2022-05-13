namespace CrazyEaters.Controllers
{
    using Godot;
    using System;
    using CrazyEaters.Managers;

    public class PlacementController : CSGCombiner
    {
        [Export]
        public bool inEditMode = false;

        [Export]
        public bool removeBlockFlag = false;

        [Export]
        public PackedScene blockPref;

        [Signal]
        public delegate void OnChangeEditMode(bool editMode);

        private GameManager gameManager;

        private Spatial currentBlock = null;

        public override void _Ready()
        {
            gameManager = GetNode<GameManager>("/root/GameManager");
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
                currentBlock.Translation = GetSnappedPos(pos);
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
                GD.Print("PLACE BLOCK");
                Vector3 pos = (Vector3) intersect["position"];
                Spatial newBlock = InstaceNewBlock(pos);
                newBlock.GetNode<StaticBody>("StaticBody").CollisionLayer = 2^1;
            }
        }

        private Godot.Collections.Dictionary ProjectRay(Vector2 mousePos)
        {
            PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;
            Vector3 rayOrigin = gameManager.camera.ProjectRayOrigin(mousePos);
            Vector3 rayEnd = rayOrigin + gameManager.camera.ProjectRayNormal(mousePos) * 2000;
            return spaceState.IntersectRay(rayOrigin, rayEnd, null, 2^1);
        }

        private Spatial InstaceNewBlock(Vector3 pos) 
        {
            Spatial newBlock = (Spatial) blockPref.Instance();
            newBlock.Translation = GetSnappedPos(pos);
            AddChild(newBlock);
            return newBlock;
        }

        public Vector3 GetSnappedPos(Vector3 pos)
        {
            float x = Mathf.Floor(pos.x);
            float y = Mathf.Floor(pos.y);
            float z = Mathf.Floor(pos.z);

            x = x % 2 == 0 ? x : x + 1;
            y = y % 2 == 0 ? y : y + 1;
            z = z % 2 == 0 ? z : z + 1;
            return new Vector3(x, y, z);
        }

        public void ChangeEditMode(bool editMode)
        {
            inEditMode = editMode;
            if (inEditMode) {
                GD.Print("In Edit");
                currentBlock = InstaceNewBlock(Vector3.Zero);
            } else {
                GD.Print("No Edit");
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