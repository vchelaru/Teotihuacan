using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Teotihuacan.Entities.Enemy;
using static Teotihuacan.Entities.Enemy.DataCategory;

namespace Teotihuacan.GameData
{
    public class Level1Spawns : LevelSpawnsBase
    {
        public Level1Spawns()
        {
            var wave = AddWave();
            wave.AddMiniWave
            (
                Shooter
            );
            wave.AddMiniWave
            (
                DataCategory.Suicider,
                DataCategory.Suicider,
                DataCategory.Suicider
            );

            wave = AddWave();
            wave.AddMiniWave
            (
                Shooter,
                Shooter,
                Shooter
            );
            wave.AddMiniWave
            (
                Shooter,
                Shooter,
                Shooter,
                Shooter
            );

            wave = AddWave();
            wave.AddMiniWave
            (
                Shooter,
                Shooter,
                Shooter
            );
            wave.AddMiniWave
            (
                DataCategory.Suicider,
                DataCategory.Suicider,
                DataCategory.Suicider
            );
            wave.AddMiniWave
            (
                Shooter,
                Shooter,
                Shooter
            );
            wave.AddMiniWave
            (
                Shooter,
                Shooter,
                Shooter,
                Shooter
            );

        }
    }
}
