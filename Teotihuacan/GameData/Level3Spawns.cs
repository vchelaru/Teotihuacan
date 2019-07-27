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

            // introduce the suicider fast
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

            // Single miniwave in this, dangerous if you're not prepared.
            wave = AddWave();

            wave.AddMiniWave
            (
                SuiciderFast, SuiciderFast, SuiciderFast,
                SuiciderFast, SuiciderFast, SuiciderFast,
                SuiciderFast, SuiciderFast, SuiciderFast
            )
            .CanSpawnAtMultipleSpots = true;

            /////////////////////////////////////

            // filler wave
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

            wave = AddWave();

            wave.AddMiniWave
            (
                SuiciderFast, SuiciderFast, SuiciderFast,

                Shooter, Shooter, Shooter
            );

            // guys from everywhere
            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter,
                Shooter, Shooter, Shooter
            )
            .CanSpawnAtMultipleSpots = true;

            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter, Shooter, 
                Suicider, Suicider, SuiciderFast, SuiciderFast
            )
            .CanSpawnAtMultipleSpots = true;


            ///////////////////////////////////////

            // 2 bosses, but maybe you can use the explosions:
            wave = AddWave();

            wave.AddMiniWave
            (
                Boss, Suicider, Suicider,
                Boss, Suicider, Suicider
            )
            .CanSpawnAtMultipleSpots = true;

        }
    }
}
