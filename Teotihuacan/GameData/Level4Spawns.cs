using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Teotihuacan.Entities.Enemy.DataCategory;


namespace Teotihuacan.GameData
{
    public class Level4Spawns : LevelSpawnsBase
    {
        public Level4Spawns()
        {
            var wave = AddWave();

            // reminder of what you have:

            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter
            );

            wave.AddMiniWave
            (
                Suicider, Suicider, Suicider,
                SuiciderFast, SuiciderFast, SuiciderFast
            )
            .CanSpawnAtMultipleSpots = true;

            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter,
                Shooter, Shooter
            )
            .CanSpawnAtMultipleSpots = true;


            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter,
                Shooter, Shooter
            )
            .CanSpawnAtMultipleSpots = true;

            ///////////////////////////////////////

            wave = AddWave();

            // Introduce the new shooter:
            wave.AddMiniWave
            (
                ShooterSpread
            );

            wave.AddMiniWave
            (
                ShooterSpread, ShooterSpread
            );

            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter,
                ShooterSpread, ShooterSpread
            );

            ///////////////////////////////////////

            wave = AddWave();

            wave.AddMiniWave
            (
                Suicider, SuiciderFast, Suicider, SuiciderFast
            )
            .CanSpawnAtMultipleSpots = true;

            wave.AddMiniWave
            (
                ShooterSpread, ShooterSpread, SuiciderFast, SuiciderFast
            );

            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter, 
                Shooter, ShooterSpread, ShooterSpread
            );

            wave.AddMiniWave
            (
                Shooter, Shooter, 
                Suicider, Shooter,
                Shooter, Suicider, 
                Shooter, Shooter,
                Suicider
            );


            ///////////////////////////////////////

            wave = AddWave();

            wave.AddMiniWave
            (
                SuiciderFast, SuiciderFast, SuiciderFast,
                SuiciderFast, SuiciderFast, SuiciderFast
            )
            .CanSpawnAtMultipleSpots = true;

            wave.AddMiniWave
            (
                SuiciderFast, SuiciderFast, SuiciderFast,
                SuiciderFast, SuiciderFast, SuiciderFast,
                SuiciderFast, SuiciderFast, SuiciderFast
            )
            .CanSpawnAtMultipleSpots = true;


            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter,
                Shooter, Shooter, Shooter
            )
            .CanSpawnAtMultipleSpots = true;

            ///////////////////////////////////////

            wave.AddMiniWave
            (
                ShooterSpread, ShooterSpread, ShooterSpread
            )
            .CanSpawnAtMultipleSpots = true;

            wave.AddMiniWave
            (
                ShooterSpread, ShooterSpread,
                Suicider, Suicider,      Suicider, Suicider
            )
            .CanSpawnAtMultipleSpots = true;

            wave.AddMiniWave
            (
                ShooterSpread, ShooterSpread, ShooterSpread,
                Boss
            )
            .CanSpawnAtMultipleSpots = true;



        }
    }
}
