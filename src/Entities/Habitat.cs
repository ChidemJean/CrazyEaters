using Godot;
using System;
using System.Collections.Generic;
using CrazyEaters.Resources;
using CrazyEaters.Enemies;
using CrazyEaters.Utils;

namespace CrazyEaters.Entities
{
    public class Habitat : Spatial
    {
        [Export] private NodePath characterPointPath;
        [Export] private NodePath[] enemyPointsPaths;
        private List<Position3D> enemyPoints;
        private Position3D characterPoint;
        public Vector3 CharacterPoint {
            get {
                return characterPoint != null ? characterPoint.GlobalTransform.origin : Vector3.Zero;
            }
        }

        public Action onReady;

        public override void _Ready()
        {
            if (onReady != null) onReady.Invoke();
            characterPoint = GetNode<Position3D>(characterPointPath);
            if (enemyPointsPaths != null && enemyPointsPaths.Length > 0) {
                enemyPoints = new List<Position3D>();
                for (var i = 0; i < enemyPointsPaths.Length; i++) {
                    var enemyPoint = enemyPointsPaths[i];
                    enemyPoints.Add(GetNode<Position3D>(enemyPoint));
                }
            }
        }

        public Position3D GetRandomEnemySpawnPoint()
        {
            if (enemyPoints == null || enemyPoints.Count == 0) return null;
            int idx = MathUtils.RandiRange(0, enemyPoints.Count - 1);
            return enemyPoints[idx];
        }

    }
}
