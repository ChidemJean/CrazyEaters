using Godot;
using System;
using CrazyEaters.DependencyInjection;
using CrazyEaters.Managers;

namespace CrazyEaters.Scenario
{
    public class NightLight : Spatial
    {
        [Inject] GameManager gameManager;
        [Export] NodePath lightPath;
        [Export] NodePath meshConePath;

        private SpotLight light;
        private MeshInstance meshCone;

        public override void _Ready()
        {
            this.ResolveDependencies();
            light = GetNode<SpotLight>(lightPath);
            meshCone = GetNode<MeshInstance>(meshConePath);
            gameManager.StartListening(GameEvent.InDay, OnDay);
            gameManager.StartListening(GameEvent.InNight, OnNight);
        }

        public void OnDay(object param) 
        {
            light.LightEnergy = 0f;
            meshCone.Visible = false;
        }

        public void OnNight(object param) 
        {
            light.LightEnergy = 15f;
            meshCone.Visible = true;
        }

        public override void _ExitTree()
        {
            gameManager.StopListening(GameEvent.InDay, OnDay);
            gameManager.StopListening(GameEvent.InNight, OnNight);
        }
    }
}