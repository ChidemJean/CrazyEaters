namespace CrazyEaters.Managers
{
    using Godot;
    using Godot.Collections;
    using System;

    public class SceneSwitcher : Node
    {
        [Export]
        public string initialScene = "";

        [Export]
        public Dictionary<string, PackedScene> scenes = new Dictionary<string, PackedScene>();
        
        [Export]
        public NodePath loadingPath;

        Control loading;

        GameManager gameManager;

        public CEScene currentScene = null;
        private PackedScene currentSceneResource = null;

        Godot.Object resourceQueueObj;
        ProgressBar loadingBar;

        bool isLoading = false;

        public override void _Ready()
        {
            gameManager = GetNode<GameManager>("/root/GameManager");
            loading = GetNode<Control>(loadingPath);
            loadingBar = loading.GetNode<ProgressBar>("VBox/Progress");

            gameManager.StartListening(GameEvent.ChangeScene, OnChangeScene);

            // GDScript resourceQueue = (GDScript) GD.Load("res://resource_queue.gd");
            // resourceQueueObj = (Godot.Object) resourceQueue.New();
            // resourceQueueObj.Call("start");

            ChangeScene(initialScene);
        }

        public void OnChangeScene(object scene)
        {
            string sceneStr = (string) scene;
            if (!sceneStr.Contains("_scene")) {
                sceneStr += "_scene";
            }
            ChangeScene(sceneStr);
        }

        public void ChangeScene(string key) {
            if (!scenes.ContainsKey(key)) return;
            currentSceneResource = scenes[key];
            if (currentSceneResource != null) {
                isLoading = true;
                loading.Visible = true;
                LoadScene(currentSceneResource);
                // loading.SetProcess(true);
                // resourceQueueObj.Call("queue_resource", currentSceneResource.ResourcePath, true);
            }
        }

        public async void LoadScene(PackedScene scene)
        {
            if (ResourceLoader.HasCached(scene.ResourcePath)){
                if (currentScene != null) {
                    currentScene.QueueFree();
                }
                ShowScene(ResourceLoader.Load<PackedScene>(scene.ResourcePath));
            } else {
                var loader = ResourceLoader.LoadInteractive(scene.ResourcePath);
                if (loader == null) {
                    GD.Print("Ocorreu um erro ao carregar a cena");
                    return;
                }
                while (true) {
                    Error error = loader.Poll();
                    if (error == Error.Ok) {
                        if (currentScene != null) {
                            currentScene.QueueFree();
                        }
                        loadingBar.Value = ((float)loader.GetStage())/loader.GetStageCount() * 100;
                    } else if (error == Error.FileEof) {
                        ShowScene(loader.GetResource() as PackedScene);
                        return;
                    }
                }
            }
        }

        // public override void _Process(float delta)
        // {
        //     if (isLoading) {
        //         if ((bool)resourceQueueObj.Call("is_ready", currentSceneResource.ResourcePath)) {
        //             ShowScene((PackedScene) resourceQueueObj.Call("get_resource", currentSceneResource.ResourcePath));
        //         } else {
        //             UpdateProgress(resourceQueueObj.Call("get_progress", currentSceneResource.ResourcePath));
        //         }
        //     }
        // }

        public void ShowScene(PackedScene scene) 
        {
            if (scene == null) return;
            Clean();
            
            currentScene = scene.Instance<CEScene>();
            AddChild(currentScene);
            if (currentScene.viewport3d != null) {
                gameManager.hud.ChangeViewport3d(currentScene.viewport3d);
            }
            isLoading = false;

            loading.Visible = false;
            // loading.SetProcess(false);
        }

        public void Clean() {
            if (GetChildren().Count > 0) {
                Node child = GetChild(0);
                RemoveChild(child);
                child.QueueFree();
            }
        }

        public override void _ExitTree()
        {
            gameManager.StopListening(GameEvent.ChangeScene, OnChangeScene);
        }

    }
}
