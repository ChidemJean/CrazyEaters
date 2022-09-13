using Godot;
using System;
using CrazyEaters.Managers;
using CrazyEaters.DependencyInjection;

namespace CrazyEaters.Controllers
{
    public class SpatulaController : Spatial
    {

        [Export]
        NodePath bullerSlotPath;

        [Export]
        NodePath skeletonPath;

        [Export(PropertyHint.Layers3dPhysics)]
        public uint spatulaMask;

        Spatial bulletSlot;
        Skeleton skeleton;

        [Inject]
        GameManager gameManager;

        Vector2? initialPos = null;

        public override void _Ready()
        {
            this.ResolveDependencies();

            bulletSlot = GetNode<Spatial>(bullerSlotPath);
            skeleton = GetNode<Skeleton>(skeletonPath);
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton)
            {
                InputEventMouseButton _event = (InputEventMouseButton)@event;
                if (_event.ButtonIndex == (uint)ButtonList.Left) {
                    if (_event.IsPressed()) {
                        Vector2 mousePos = _event.Position * gameManager.hud.currentScale;
                        PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;
                        Vector3 rayOrigin = GetViewport().GetCamera().ProjectRayOrigin(mousePos);
                        Vector3 rayEnd = rayOrigin + GetViewport().GetCamera().ProjectRayNormal(mousePos) * 1000;
                        Godot.Collections.Dictionary rayCol = spaceState.IntersectRay(rayOrigin, rayEnd, null, spatulaMask, false, true);
                        if (rayCol != null && rayCol.Count > 0)
                        {
                            initialPos = mousePos;
                            gameManager.inputMode = GameManager.InputMode.LAUNCHER;
                        } else {
                            initialPos = null;
                            gameManager.inputMode = GameManager.InputMode.SCENE;
                        }
                    } else {
                        initialPos = null;
                        skeleton.RotationDegrees = Vector3.Zero;
                        gameManager.inputMode = GameManager.InputMode.SCENE;
                    }
                }
                return;
            }
            if (@event is InputEventMouseMotion) {
                InputEventMouseMotion _event = (InputEventMouseMotion) @event;
                if (initialPos != null) {
                    Vector2 mousePos = _event.Position * gameManager.hud.currentScale;
                    Vector2 initial = (Vector2) initialPos;
                    Vector2 aux = new Vector2(initial.x, initial.y + 20f);
                    float angle = Mathf.Rad2Deg((initial - mousePos).AngleTo(initial - aux));
                    skeleton.RotationDegrees = new Vector3(0, angle, 0);
                }
            }
        }

        private Vector3 GetBoneRotation(string boneName)
        {
            int boneIndex = skeleton.FindBone(boneName);
            Transform rest = skeleton.GetBoneRest(boneIndex);
            return rest.basis.GetEuler();
        }

        private void SetBoneRotation(string boneName, Vector3 angle)
        {
            int boneIndex = skeleton.FindBone(boneName);
            Transform rest = skeleton.GetBoneCustomPose(boneIndex);
            Transform newPose = rest.Rotated(Vector3.Right, angle.x);
            newPose = newPose.Rotated(Vector3.Up, angle.y);
            newPose = newPose.Rotated(Vector3.Back, angle.z);
            skeleton.SetBoneCustomPose(boneIndex, newPose);
        }
   }
}