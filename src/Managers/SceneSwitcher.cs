namespace CrazyEaters.Managers
{
    using Godot;
    using Godot.Collections;
    using System;

    public class SceneSwitcher : Node
    {
        [Export]
        public Dictionary<string, PackedScene> scenes = new Dictionary<string, PackedScene>();
        
        [Export]
        public NodePath loadingPath;

        Control loading;

        GameManager gameManager;

        public CEScene currentScene = null;
        private PackedScene currentSceneResource = null;

        Godot.Object resourceQueueObj;

        bool isLoading = false;

        public override void _Ready()
        {
            gameManager = GetNode<GameManager>("/root/GameManager");
            loading = GetNode<Control>(loadingPath);

            GDScript resourceQueue = (GDScript) GD.Load("res://resource_queue.gd");
            resourceQueueObj = (Godot.Object) resourceQueue.New();
            resourceQueueObj.Call("start");

            ChangeScene("habitat_scene");
        }

        public void ChangeScene(string key) {
            currentSceneResource = scenes[key];
            if (currentSceneResource != null) {
                isLoading = true;
                loading.Visible = true;
                loading.SetProcess(true);
                resourceQueueObj.Call("queue_resource", currentSceneResource.ResourcePath, true);
            }
        }

        public override void _Process(float delta)
        {
            if (isLoading) {
                if ((bool)resourceQueueObj.Call("is_ready", currentSceneResource.ResourcePath)) {
                    ShowScene((PackedScene) resourceQueueObj.Call("get_resource", currentSceneResource.ResourcePath));
                } else {
                    UpdateProgress(resourceQueueObj.Call("get_progress", currentSceneResource.ResourcePath));
                }
            }
        }

        public void ShowScene(PackedScene scene) 
        {
            if (scene == null) return;
            Clean();
            
            currentScene = scene.Instance<CEScene>();
            AddChild(currentScene);
            gameManager.hud.ChangeViewport3d(currentScene.viewport3d);
            isLoading = false;

            loading.Visible = false;
            loading.SetProcess(false);
        }

        public void UpdateProgress(object progress) 
        {
        }

        public void Clean() {
            if (GetChildren().Count > 0) {
                Node child = GetChild(0);
                RemoveChild(child);
                child.QueueFree();
            }
        }

    }
}
