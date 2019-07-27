using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Teotihuacan.Entities.Enemy.DataCategory;


namespace Teotihuacan.GameData
{
    public class Level3Spawns : LevelSpawnsBase
    {
        public Level3Spawns()
        {
            var wave = AddWave();

            wave.AddMiniWave
            (
                SuiciderFast, SuiciderFast
            );

            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter
            );

            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter
            );

            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter
            );

            /////////////////////////////////////

            wave = AddWave();

            wave.AddMiniWave
            (
                SuiciderFast, SuiciderFast, SuiciderFast,
                SuiciderFast, SuiciderFast, SuiciderFast,
                SuiciderFast, SuiciderFast, SuiciderFast
            )
            .CanSpawnAtMultipleSpots = true;

            /////////////////////////////////////


            wave = AddWave();

            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter,
                Shooter, Shooter, Shooter
            )
            .CanSpawnAtMultipleSpots = true;

            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter,
                Shooter, Shooter, Shooter
            )
            .CanSpawnAtMultipleSpots = true;

            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter,
                Shooter, Shooter, Shooter
            )
            .CanSpawnAtMultipleSpots = true;

            /////////////////////////////////////
        }
    }
}
