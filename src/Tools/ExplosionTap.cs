namespace CrazyEaters.Tools 
{
    using Godot;
    using Godot.Collections;
    using System;
    using CrazyEaters.Managers;
    using CrazyEaters.Optimization;

    public class ExplosionTap : Spatial
    {
        GameManager gameManager;
        Hud hud;

        [Export]
        Resource shapeExplosion;

        [Export]
        float impulseForce = 30;

        public override void _Ready()
        {
            gameManager = GetNode<GameManager>("/root/GameManager");
            hud = GetNode<Hud>("/root/MainNode");
        }

        public override void _Input(InputEvent @event)
        {
            if (gameManager.inputMode == GameManager.InputMode.SCENE) {
                if (@event is InputEventMouseButton) {
                    InputEventMouseButton _event = (InputEventMouseButton) @event;
                    Vector2 mousePos = _event.Position * hud.currentScale;

                    if (_event.ButtonIndex == 1 && _event.IsPressed()) {
                        PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;
                        Vector3 rayOrigin = gameManager.camera.ProjectRayOrigin(mousePos);
                        Vector3 rayEnd = rayOrigin + gameManager.camera.ProjectRayNormal(mousePos) * 2000;
                        var intersect = spaceState.IntersectRay(rayOrigin, rayEnd);

                        if (intersect != null && intersect.Count > 0) {
                            Vector3 pos = (Vector3) intersect["position"];
                            PhysicsShapeQueryParameters _params = new PhysicsShapeQueryParameters();
                            _params.SetShape(shapeExplosion);
                            _params.Transform = new Transform(Quat.Identity, pos);
                            var intersectExplosion = spaceState.IntersectShape(_params);
                            
                            if (intersectExplosion != null && intersectExplosion.Count > 0) {
                                foreach (Dictionary item in intersectExplosion) {
                                    if (item["collider"] is RigidBody) {
                                        RigidBody collider = (RigidBody) item["collider"];
                                        Vector3 impulse = (collider.GlobalTransform.origin - pos).Normalized();
                                        collider.ApplyCentralImpulse(impulse * impulseForce);
                                    }
                                }
                            }
                        }
                    }

                } else if (@event is InputEventMouseMotion) {
                    InputEventMouseMotion _event = (InputEventMouseMotion) @event;
                }
            }
        }

    }
}
