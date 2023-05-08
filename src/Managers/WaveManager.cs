using Godot;
using System;
using System.Collections.Generic;
using CrazyEaters.DependencyInjection;
using CrazyEaters.Entities;
using CrazyEaters.Utils;
using CrazyEaters.Resources;
using CrazyEaters.Enemies;

namespace CrazyEaters.Managers
{    
    public class WaveManager : Node
    {
        [Inject] private AccountManager accountManager;
        [Inject] private SceneSwitcher sceneSwitcher;
        private HabitatScene habitatScene;
        private Habitat habitat;
        private Spatial enemiesCtn;
        private SceneTreeTimer stTimer;

        public override void _Ready()
        {
            this.ResolveDependencies();
            habitatScene = (sceneSwitcher.currentScene as HabitatScene);
            habitatScene.Connect("HabitatInit", this, nameof(OnHabitatInit));
        }

        public void OnHabitatInit() 
        {
            enemiesCtn = habitatScene.EnemiesCtn;
            habitat = habitatScene.CurrentHabitat;
            CreateWave();
        }

        public void CreateWave() 
        {
            var enemies = RequestUnblockedEnemies(2);
            int timeBetweenSpawns = 5;
            StartWave(enemies, timeBetweenSpawns);
        }

        public async void StartWave(List<EnemyData> enemies, int timeBetweenSpawns)
        {
            foreach (var enemyData in enemies) 
            {
                SpawnEnemy(enemyData);
                stTimer = GetTree().CreateTimer(timeBetweenSpawns);
                await ToSignal(stTimer, "timeout");
            }
        }

        public Enemy SpawnEnemy(EnemyData enemyData)
        {
            var spawnPoint = habitat.GetRandomEnemySpawnPoint();
            if (spawnPoint == null) return null;
            var enemyNode = enemyData.prefab.Instance<Enemy>();
            enemiesCtn.AddChild(enemyNode);
            enemyNode.GlobalTranslation = spawnPoint.GlobalTransform.origin;
            return enemyNode;
        }
        
        public List<EnemyData> RequestUnblockedEnemies(int waveLenght = 1) 
        {
            if (waveLenght == 0) return null;

            var unblockedEnemies = accountManager.UnblockedEnemies;
            List<EnemyData> waveEnemies = new List<EnemyData>();

            for (int i = 0; i < waveLenght; i++) {
                int idx = MathUtils.RandiRange(0, unblockedEnemies.Count - 1);
                waveEnemies.Add(unblockedEnemies[idx]);
            }

            return waveEnemies;
        }

    }
}