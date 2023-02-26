namespace CrazyEaters.Managers
{
    using Godot;
    using System;
    using CrazyEaters.Managers;
    using CrazyEaters.Save;

    public class CEScene : Control
    {
        [Export]
        public NodePath viewport3dPath;

        public Viewport viewport3d = null;

        public GameManager gameManager;
        public SaveSystemNode saveSystemNode;

        public override void _EnterTree()
        {
            if (viewport3dPath != null) viewport3d = GetNodeOrNull<Viewport>(viewport3dPath);
            gameManager = GetNode<GameManager>("/root/GameManager");
            saveSystemNode = GetNode<SaveSystemNode>("/root/MainNode/SaveSystem");
        }

        // TODO:
        public void Save(GameData gameData, Action OnSave = null)
        {
            // saveSystemNode.SaveGame(OnSave);
        }

        public void Load(Action<GameData> OnLoad)
        {
            // saveSystemNode.LoadGame((GameData gameData) => {
            //     OnLoad.Invoke(gameData);
            // });
        }
    }
}
