﻿using FlatRedBall;
using FlatRedBall.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teotihuacan.Entities;
using Teotihuacan.GameData;

namespace Teotihuacan.Managers
{
    public class SpawnManager
    {
        int CurrentSpawnIndex;
        
        public void DoActivity(PositionedObjectList<Enemy> enemies, PositionedObjectList<SpawnPoint> spawnPoints, LevelSpawnsBase spawns)
        {
            if (spawns != null)
            {
                var hasSpawnsLeft = CurrentSpawnIndex < spawns.MiniWaves.Count;

                var shouldSpawn = DetermineIfShouldSpawn(enemies);

                if (shouldSpawn && hasSpawnsLeft)
                {
                    var miniWave = spawns.MiniWaves[CurrentSpawnIndex];

                    foreach (var data in miniWave.Spawns)
                    {
                        // todo - determine spawn locations...
                        var x = 100;
                        var y = -100;

                        var spawnPoint = FlatRedBallServices.Random.In(spawnPoints);

                        var enemy = Factories.EnemyFactory.CreateNew(spawnPoint.X, spawnPoint.Y);
                        enemy.CurrentDataCategoryState = data;

                    }

                    CurrentSpawnIndex++;
                }
            }
        }

        private bool DetermineIfShouldSpawn(PositionedObjectList<Enemy> enemies)
        {
            return enemies.Count == 0;
        }
    }
}
