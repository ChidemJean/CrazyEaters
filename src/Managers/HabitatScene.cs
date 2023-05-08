namespace CrazyEaters.Managers
{
    using Godot;
    using System;
    using System.Collections.Generic;
    using CrazyEaters.Controllers;
    using CrazyEaters.Save;
    using CrazyEaters.Characters;
    using CrazyEaters.Entities;
    using CrazyEaters.Resources;
    using CrazyEaters.DependencyInjection;


    public class HabitatScene : CEScene
    {
        [Export]
        NodePath main3DNodePath;
        [Export]
        NodePath cameraPath;
        [Export]
        NodePath gameViewportPath;
        [Export]
        NodePath placementControllerPath;
        [Export]
        public bool inEditMode = false;

        public Spatial main3DNode;
        public Camera camera;
        public Viewport gameViewport;
        public PlacementController placementController;
        [Export]
        public int currentBlockId = 3;
        [Export] private NodePath characterCtnPath;
        [Export] private NodePath enemiesCtnPath;
        [Export] private NodePath habitatCtnPath;
        [Export] private NodePath navigationPath;
        private Spatial characterCtn;
        private Spatial enemiesCtn;
        private Spatial habitatCtn;
        private Navigation navigation;
        private NavigationMeshInstance navigationMesh;
        private string saveUUID;
        private Habitat currentHabitat;
        private Character character;
        public Habitat CurrentHabitat {
            get {
                return currentHabitat;
            }
        }
        public Spatial EnemiesCtn {
            get {
                return enemiesCtn;
            }
        }
        public Character Character {
            get {
                return character;
            }
        }
        
        [Signal] public delegate void HabitatInit();

        public override void _Ready()
        {
            base._Ready();
            
            camera = GetNode<Camera>(cameraPath);
            gameViewport = GetNode<Viewport>(gameViewportPath);
            main3DNode = GetNode<Spatial>(main3DNodePath);
            placementController = GetNode<PlacementController>(placementControllerPath);
            characterCtn = GetNode<Spatial>(characterCtnPath);
            enemiesCtn = GetNode<Spatial>(enemiesCtnPath);
            habitatCtn = GetNode<Spatial>(habitatCtnPath);
            navigation = GetNode<Navigation>(navigationPath);
            navigationMesh = navigation.GetNode<NavigationMeshInstance>("Navmesh");

            saveUUID = (string) args[1];
            gameManager.currentMainNode3D = main3DNode;
            saveSystemNode.LoadHabitats(OnLoadHabitats);
        }

        public void OnLoadHabitats(HabitatsGameData habitatsGameData)
        {
            foreach (var habitat in habitatsGameData.habitats) {
                if (habitat.uuid == saveUUID) {
                    var habitatNode = InitHabitat(habitat);
                    InitCharacter(habitat.character, habitat.statusesEater, habitatNode.CharacterPoint);
                    return;
                }
            }
        }

        public void InitCharacter(CrazyEaters.Save.CharacterData _characterData, List<CrazyEaters.Save.StatusCharacter> statusesEater, Vector3 spawnPoint)
        {
            var characterData = itemsManager.FindByKey(_characterData.id) as CrazyEaters.Resources.CharacterData;
            var characterNode = characterData.prefab.Instance<Character>();
            characterCtn.AddChild(characterNode);
            character = characterNode;
            characterNode.GlobalTranslation = spawnPoint;
        }
        public Habitat InitHabitat(HabitatGameData habitatGameData)
        {
            var habitatData = itemsManager.FindByKey(habitatGameData.habitatID) as CrazyEaters.Resources.HabitatData;
            currentHabitat = habitatData.prefab.Instance<Habitat>();
            currentHabitat.onReady = () => {
                navigationMesh.BakeNavigationMesh(true);
            };
            habitatCtn.AddChild(currentHabitat);
            EmitSignal("HabitatInit");
            return currentHabitat;
        }

        public BlockData RetrieveBlockDataFromId(int blockId)
        {
            BlockData data = null;
            // this.blocksData.blocks.TryGetValue(blockId + "", out data);
            return data;
        }
    }
}
