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
        NodePath meshPath;

        [Export]
        NodePath skeletonPath;

        [Export]
        NodePath animationPlayerPath;

        [Export(PropertyHint.Layers3dPhysics)]
        public uint spatulaMask;

        Spatial bulletSlot;
        Skeleton skeleton;
        MeshInstance meshInstance;
        AnimationPlayer animationPlayer;

        [Inject]
        GameManager gameManager;

        Vector2? initialPos = null;

        float currentDistance = 0f;

        public override void _Ready()
        {
            this.ResolveDependencies();

            bulletSlot = GetNode<Spatial>(bullerSlotPath);
            skeleton = GetNode<Skeleton>(skeletonPath);
            meshInstance = GetNode<MeshInstance>(meshPath);
            animationPlayer = GetNode<AnimationPlayer>(animationPlayerPath);
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
                        animationPlayer.Play("hold");
                        float animLength = animationPlayer.GetAnimation("hold").Length / animationPlayer.PlaybackSpeed;
                        animationPlayer.Advance(animLength - animLength * currentDistance);
                        currentDistance = 0f;
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
                    currentDistance = Mathf.Clamp(mousePos.DistanceTo(initial) / 200f, 0f, 1f);

                    float angle = Mathf.Rad2Deg((initial - mousePos).AngleTo(initial - aux));
                    skeleton.RotationDegrees = new Vector3(0, angle, 0);
                    animationPlayer.Play("pull");
                    float animLength = animationPlayer.GetAnimation("pull").Length / animationPlayer.PlaybackSpeed;
                    animationPlayer.Advance(animLength * currentDistance);
                    animationPlayer.Stop();
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
            Transform rest = skeleton.GetBoneRest(boneIndex);
            Transform newPose = rest.Rotated(Vector3.Right, angle.x);
            newPose = newPose.Rotated(Vector3.Up, angle.y);
            newPose = newPose.Rotated(Vector3.Back, angle.z);
            skeleton.SetBonePose(boneIndex, newPose);
        }
   }
}