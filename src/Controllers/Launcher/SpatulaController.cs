using Godot;
using System;
using System.Collections.Generic;
using CrazyEaters.Managers;
using CrazyEaters.Tools;
using CrazyEaters.DependencyInjection;

namespace CrazyEaters.Controllers.Launcher
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
        NodePath skeletonBasePath;

        [Export]
        NodePath animationPlayerPath;

        [Export]
        float throwPower = 10;
        float curThrowPower = 0;

        [Export(PropertyHint.Layers3dPhysics)]
        public uint spatulaMask;

        Spatial bulletSlot;
        Skeleton skeleton;
        Skeleton skeletonBase;
        MeshInstance meshInstance;
        AnimationPlayer animationPlayer;

        [Inject]
        GameManager gameManager;

        Vector2? initialPos = null;

        float currentDistance = 0f;

        SceneTreeTween tween = null;

        [Export]
        NodePath lineRendererPath;
        LineRenderer lineRenderer;
        [Export]
        int numPoints = 8;
        [Export] 
        public float timeBetweenPoints = 1f;

        [Export]
        NodePath throwablePath;

        IThrowable throwable;

        public override void _Ready()
        {
            this.ResolveDependencies();

            bulletSlot = GetNode<Spatial>(bullerSlotPath);
            skeleton = GetNode<Skeleton>(skeletonPath);
            skeletonBase = GetNode<Skeleton>(skeletonBasePath);
            meshInstance = GetNode<MeshInstance>(meshPath);
            animationPlayer = GetNode<AnimationPlayer>(animationPlayerPath);
            lineRenderer = GetNode<LineRenderer>(lineRendererPath);
            if (throwablePath != null) {
                throwable = GetNode<IThrowable>(throwablePath);
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            if (throwable != null) {
                ((Spatial) throwable).GlobalTranslation = bulletSlot.GlobalTransform.origin;
                ((Spatial) throwable).GlobalRotation = bulletSlot.GlobalRotation;
            }

            Transform targetGlobalPose = LocalPoseToGlobalPose(skeleton.FindBone("Bone3"), skeleton.GetBoneGlobalPose(skeleton.FindBone("Bone3")), skeleton);
            // Transform globalPoseBase = GlobalPoseToLocalPose(skeletonBase.FindBone("Center"), skeletonBase.GetBoneGlobalPose(skeletonBase.FindBone("Center")), skeletonBase);
            Transform globalPoseBase = skeletonBase.GetBoneGlobalPose(skeletonBase.FindBone("Center"));
            globalPoseBase.origin = targetGlobalPose.origin + new Vector3(.7f,5.6f,0);
            skeletonBase.SetBoneGlobalPoseOverride(skeletonBase.FindBone("Center"), globalPoseBase, 1.0f, true);
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
                            StopTween();
                        } else {
                            initialPos = null;
                            if (gameManager.inputMode == GameManager.InputMode.LAUNCHER) gameManager.inputMode = GameManager.InputMode.SCENE;
                        }
                    } else {
                        initialPos = null;
                        if (gameManager.inputMode == GameManager.InputMode.LAUNCHER) gameManager.inputMode = GameManager.InputMode.SCENE;
                        animationPlayer.Play("hold");
                        float animLength = animationPlayer.GetAnimation("hold").Length / animationPlayer.PlaybackSpeed;
                        animationPlayer.Advance(animLength - animLength * currentDistance);
                        currentDistance = 0f;
                        lineRenderer.SetProcess(false);
                        lineRenderer.Clear();
                        if (curThrowPower != 0 && throwable != null) {
                            throwable.Drop(-bulletSlot.GlobalTransform.basis.x * (throwPower * 6f * curThrowPower), bulletSlot.GlobalTransform.origin);
                            throwable = null;
                        }
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
                    currentDistance = Mathf.Clamp(mousePos.DistanceTo(initial) / 300f, 0f, 1f);

                    float angle = Mathf.Rad2Deg((initial - mousePos).AngleTo(initial - aux));
                    skeleton.RotationDegrees = new Vector3(0, angle, 0);
                    animationPlayer.Play("pull");
                    float animLength = animationPlayer.GetAnimation("pull").Length / animationPlayer.PlaybackSpeed;
                    animationPlayer.Advance(animLength * currentDistance);
                    animationPlayer.Stop();

                    if (throwable == null) {
                        throwable = GetNode<IThrowable>(throwablePath);
                        throwable.Hold(bulletSlot);
                    }

                    if (currentDistance > 0.01f) {
                        curThrowPower = currentDistance;
                        lineRenderer.SetProcess(true);
                        lineRenderer.UpdatePoints(GetTrajectoryPoints(currentDistance));
                    } else {
                        curThrowPower = 0;
                    }
                }
                return;
            }
        }

        public Vector3 GetThrowVec(float force)
        {
            return -bulletSlot.GlobalTransform.basis.x * (throwPower * force);
        }

        public List<Vector3> GetTrajectoryPoints(float force)
        {
            List<Vector3> points = new List<Vector3>();

            Vector3 startingPosition = bulletSlot.GlobalTransform.origin;
            Vector3 startingVelocity = GetThrowVec(force);

            for (float t = 0; t < numPoints; t += timeBetweenPoints)
            {
                Vector3 newPoint = startingPosition + t * startingVelocity;
                newPoint.y = startingPosition.y + startingVelocity.y * t + gameManager.gravityVector.y/4f * t * t;
                points.Add(newPoint);
            }

            return points;
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

        public async void HoldAnimationFinished()
        {
            tween = GetTree().CreateTween();
            tween.TweenProperty(skeleton, "rotation_degrees", Vector3.Zero, .9f).SetTrans(Tween.TransitionType.Elastic).SetEase(Tween.EaseType.Out);
            await ToSignal(tween, "finished");
            tween = null;
        }

        public void StopTween()
        {
            if (tween != null) { 
                tween.Stop();
                // tween.Free();
            }
        }

        public void OnAnimationFinished(string name)
        {
            if (name == "hold") {
                HoldAnimationFinished();
            }
        }
   
        public Transform GlobalPoseToLocalPose(int bondeIndex, Transform globalPose, Skeleton skeleton)
        {
            if (skeleton.GetBoneParent(bondeIndex) >= 0) {
                int parentBoneIdx = skeleton.GetBoneParent(bondeIndex);
                Transform conversionTransform = skeleton.GetBoneGlobalPose(parentBoneIdx) * skeleton.GetBoneRest(bondeIndex);
                return conversionTransform.AffineInverse() * globalPose;
            }
            return globalPose;
        }

        public Transform LocalPoseToGlobalPose(int bondeIndex, Transform localPose, Skeleton skeleton)
        {
            if (skeleton.GetBoneParent(bondeIndex) >= 0) {
                int parentBoneIdx = skeleton.GetBoneParent(bondeIndex);
                Transform conversionTransform = skeleton.GetBoneGlobalPose(parentBoneIdx) * skeleton.GetBoneRest(bondeIndex);
                return conversionTransform * localPose;
            }
            return localPose;
        }
   }
}