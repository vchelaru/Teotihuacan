using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teotihuacan.GameData
{
    public class Level1Spawns : LevelSpawnsBase
    {
        public Level1Spawns()
        {
            var wave = AddWave();
            wave.AddMiniWave
            (
                Entities.Enemy.DataCategory.Shooter
            );
            wave.AddMiniWave
            (
                Entities.Enemy.DataCategory.Shooter
            );

            wave = AddWave();
            wave.AddMiniWave
            (
                Entities.Enemy.DataCategory.Shooter,
                Entities.Enemy.DataCategory.Shooter,
                Entities.Enemy.DataCategory.Shooter
            );
            wave.AddMiniWave
            (
                Entities.Enemy.DataCategory.Shooter,
                Entities.Enemy.DataCategory.Shooter,
                Entities.Enemy.DataCategory.Shooter,
                Entities.Enemy.DataCategory.Shooter
            );

            wave = AddWave();
            wave.AddMiniWave
            (
                Entities.Enemy.DataCategory.Shooter,
                Entities.Enemy.DataCategory.Shooter,
                Entities.Enemy.DataCategory.Shooter
            );
            wave.AddMiniWave
            (
                Entities.Enemy.DataCategory.Shooter,
                Entities.Enemy.DataCategory.Shooter
            );
            wave.AddMiniWave
            (
                Entities.Enemy.DataCategory.Shooter,
                Entities.Enemy.DataCategory.Shooter,
                Entities.Enemy.DataCategory.Shooter
            );
            wave.AddMiniWave
            (
                Entities.Enemy.DataCategory.Shooter,
                Entities.Enemy.DataCategory.Shooter,
                Entities.Enemy.DataCategory.Shooter,
                Entities.Enemy.DataCategory.Shooter
            );

        }
    }
}
