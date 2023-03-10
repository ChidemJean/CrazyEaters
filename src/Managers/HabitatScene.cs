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
        [Export] private NodePath characterPointPath;
        [Export] private NodePath habitatCtnPath;
        [Export] private NodePath navigationPath;
        private Spatial characterCtn;
        private Position3D characterPoint;
        private Spatial habitatCtn;
        private Navigation navigation;
        private NavigationMeshInstance navigationMesh;
        private string saveUUID;

        public override void _Ready()
        {
            base._Ready();
            
            camera = GetNode<Camera>(cameraPath);
            gameViewport = GetNode<Viewport>(gameViewportPath);
            main3DNode = GetNode<Spatial>(main3DNodePath);
            placementController = GetNode<PlacementController>(placementControllerPath);
            characterCtn = GetNode<Spatial>(characterCtnPath);
            habitatCtn = GetNode<Spatial>(habitatCtnPath);
            navigation = GetNode<Navigation>(navigationPath);
            characterPoint = GetNode<Position3D>(characterPointPath);
            navigationMesh = navigation.GetNode<NavigationMeshInstance>("Navmesh");

            saveUUID = (string) args[1];
            gameManager.currentMainNode3D = main3DNode;
            saveSystemNode.LoadHabitats(OnLoadHabitats);
        }

        public void OnLoadHabitats(HabitatsGameData habitatsGameData)
        {
            foreach (var habitat in habitatsGameData.habitats) {
                if (habitat.uuid == saveUUID) {
                    InitHabitat(habitat);
                    InitCharacter(habitat.character, habitat.statusesEater);
                    return;
                }
            }
        }

        public void InitCharacter(CrazyEaters.Save.CharacterData _characterData, List<CrazyEaters.Save.StatusCharacter> statusesEater)
        {
            var characterData = itemsManager.FindByKey(_characterData.id) as CrazyEaters.Resources.CharacterData;
            GD.Print(characterData.prefab);
            var characterNode = characterData.prefab.Instance<Character>();
            characterCtn.AddChild(characterNode);
            characterNode.GlobalTranslation = characterPoint.GlobalTransform.origin;
        }
        public void InitHabitat(HabitatGameData habitatGameData)
        {
            var habitatData = itemsManager.FindByKey(habitatGameData.habitatID) as CrazyEaters.Resources.HabitatData;
            var habitatNode = habitatData.prefab.Instance<Habitat>();
            habitatNode.onReady = () => {
                navigationMesh.BakeNavigationMesh(true);
            };
            habitatCtn.AddChild(habitatNode);
        }

        public BlockData RetrieveBlockDataFromId(int blockId)
        {
            BlockData data = null;
            // this.blocksData.blocks.TryGetValue(blockId + "", out data);
            return data;
        }
    }
}
