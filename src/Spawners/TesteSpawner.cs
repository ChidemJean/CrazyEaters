namespace CrazyEaters.Spawners 
{

    using Godot;
    using System;
    using System.Collections.Generic;
    using CrazyEaters.Managers;
    using System.Threading.Tasks;

    public class TesteSpawner : Spatial
    {
        [Export]
        public PackedScene[] prefabs = null;
        public Queue<PackedScene> prefabsQueue = new Queue<PackedScene>();
        public List<PackedScene> prefabsLoaded = new List<PackedScene>();

        [Export]
        public int qtdInstances = 2;

        [Export]
        public bool spawn = true;

        public Random random;
        GameManager gameManager;
        public int loaded = 0;
        Godot.Object resourceQueueGD;
        Thread thread;

        public override void _Ready()
        {
            random = new Random();

            foreach (PackedScene item in prefabs) {
                prefabsQueue.Enqueue(item);
            }

            // resourceQueueGD = GetNode("/root/ResourceQueue");
            // resourceQueueGD.Call("start");

            thread = new Thread();

            if (spawn) {
                Start();
            }
        }


        public void item_load(Resource resource)
        {
            GD.Print("item load!");
            prefabsLoaded.Add((PackedScene)resource);
        }

        public async void Start()
        {
            if (prefabsQueue.Count > 0) {
                PackedScene item = prefabsQueue.Dequeue();
                ResourceInteractiveLoader interactive_ldr = ResourceLoader.LoadInteractive(item.ResourcePath);
                PackedScene res = await Task.Run(async () => {
                                    while(true) {
                                        Error err = interactive_ldr.Poll();
                                        GD.Print(interactive_ldr.GetStage() / interactive_ldr.GetStageCount());
                                        if (err == Error.FileEof) {
                                            return interactive_ldr.GetResource();
                                        }
                                    }
                                }) as PackedScene;
                item_load(res);
                Start();
            } else {
                if (prefabsLoaded != null && prefabsLoaded.Count > 0) {
                    for (int i = 0; i < qtdInstances; i++) {
                        int index = random.Next(prefabsLoaded.Count);
                        PackedScene prefab = prefabsLoaded[index];
                        Spatial node = prefab.Instance() as Spatial;
                        node.Translate(new Vector3((float)GD.RandRange(-20f, 20f), 0, (float)GD.RandRange(-20f, 20f)));
                        AddChild(node);
                        await Task.Delay(TimeSpan.FromMilliseconds(GD.Randf() * 300));
                    }
                }
            }
        }
    }
}