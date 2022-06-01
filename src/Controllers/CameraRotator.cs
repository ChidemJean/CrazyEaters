namespace CrazyEaters.Controllers
{
    using Godot;
    using System;
    using CrazyEaters.Managers;
    using CrazyEaters.Optimization;

    public class CameraRotator : Spatial
    {
        [Export]
        public float speed;
        [Export]
        public float zoomSpeed = 2f;
        [Export]
        public NodePath targetNode;
        private bool tapHolding = false;
        private Vector2 initialCursorPos = Vector2.Zero;
        private Godot.Object zoomAnimation = null;
        Hud hud;
        Camera camera;
        GameManager gameManager;
        
        public override void _Ready()
        {
            gameManager = GetNode<GameManager>("/root/GameManager");
            hud = gameManager.hud;
            camera = GetNode<Camera>(targetNode);
        }

        public override void _Input(InputEvent @event)
        {
            if (gameManager.inputMode == GameManager.InputMode.SCENE) {
                if (tapHolding) {
                    if (@event is InputEventMouseMotion) {
                        InputEventMouseMotion _event = (InputEventMouseMotion) @event;
                        Vector2 mousePos = _event.Position * (hud != null ? hud.currentScale : 1);
                        OnPosition(mousePos);
                        return;
                    }
                }
                if (@event is InputEventMouseButton) {
                    InputEventMouseButton _event = (InputEventMouseButton) @event;

                    if (_event.ButtonIndex == (int) ButtonList.Left) {
                        Vector2 mousePos = _event.Position * (hud != null ? hud.currentScale : 1);
                        if (_event.IsPressed()) {
                            OnTap(mousePos);
                        } else {
                            OnRelease();
                        }
                        return;
                    }

                    if (_event.IsPressed() && _event.ButtonIndex == (int) ButtonList.WheelDown) {
                        OnZoom(1);
                    }
                    if (_event.IsPressed() && _event.ButtonIndex == (int) ButtonList.WheelUp) {
                        OnZoom(-1);
                    }
                }
            }
        }

        public void OnZoom(int direction)
        {
            float zoomAmount = direction * zoomSpeed;
            float movement = Mathf.Clamp(camera.Translation.z + zoomAmount, 8f, 80f);
            camera.Translation = new Vector3(camera.Translation.x, camera.Translation.y, movement);
        }

        public void OnTap(Vector2 position)
        {
            tapHolding = true;
            initialCursorPos = position;
        }

        public void OnRelease()
        {
            tapHolding = false;
            initialCursorPos = Vector2.Zero;
        }

        void OnPosition(Vector2 position)
        {
            if (tapHolding) {

                float angleY = 0;
                float angleX = 0;

                if (initialCursorPos != Vector2.Zero) {
                    Vector2 moveDir = (position - initialCursorPos);

                    Vector3 rotation = RotationDegrees;

                    angleY = (rotation.y) + (moveDir.x * speed * GetProcessDeltaTime()) * -1;
                    angleX = (rotation.x) + (moveDir.y * speed * GetProcessDeltaTime()) * -1;

                    angleX = Mathf.Clamp(angleX, -90f, 5f);

                    RotationDegrees = new Vector3(angleX, angleY, 0);

                    initialCursorPos = position;
                }

            }
        }
    }
}
