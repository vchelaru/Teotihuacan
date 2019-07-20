using FlatRedBall;
using FlatRedBall.Instructions;
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
        public int CurrentWaveIndex { get; private set; }
        public bool CanSpawn { get; private set; }

        public void EnableSpawning()
        {
            CanSpawn = true;
        }
        
        public void DisableSpawning()
        {
            CanSpawn = false;
        }

        public void DoActivity(PositionedObjectList<Enemy> enemies, PositionedObjectList<SpawnPoint> spawnPoints, LevelSpawnsBase spawns)
        {
            if (spawns != null && CanSpawn)
            {
                var hasSpawnsLeft = CurrentWaveIndex < spawns.Waves.Count || 
                                    CurrentSpawnIndex < spawns.Waves[CurrentWaveIndex].MiniWaves.Count;

                var shouldSpawn = DetermineIfShouldSpawn(enemies);

                if (shouldSpawn && hasSpawnsLeft)
                {
                    var miniWave = spawns.Waves[CurrentWaveIndex].MiniWaves[CurrentSpawnIndex];

                    SpawnMiniWave(spawnPoints, miniWave);

                    CurrentSpawnIndex++;

                    if (CurrentSpawnIndex >= spawns.Waves[CurrentWaveIndex].MiniWaves.Count)
                    {
                        CurrentWaveIndex++;
                        CanSpawn = false;
                        CurrentSpawnIndex = 0;
                    }
                }
            }
        }

        private static void SpawnMiniWave(PositionedObjectList<SpawnPoint> spawnPoints, MiniWave miniWave)
        {
            // make the same spawn point used for all enemies on this subwave
            var spawnPoint = FlatRedBallServices.Random.In(spawnPoints);

            for (int i = 0; i < miniWave.Spawns.Count; i++)
            {
                var data = miniWave.Spawns[i];
                if (i == 0)
                {
                    // do it now!
                    var enemy = Factories.EnemyFactory.CreateNew(spawnPoint.X, spawnPoint.Y);
                    enemy.CurrentDataCategoryState = data;
                }
                else
                {
                    spawnPoint.Call(() =>
                    {
                        var enemy = Factories.EnemyFactory.CreateNew(spawnPoint.X, spawnPoint.Y);
                        enemy.CurrentDataCategoryState = data;
                    }).After(i * SpawnPoint.SecondsBetweenEachEnemyInMiniWave);
                }
            }
        }

        private bool DetermineIfShouldSpawn(PositionedObjectList<Enemy> enemies)
        {
            return enemies.Count == 0;
        }
    }
}
