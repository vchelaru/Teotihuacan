using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Teotihuacan.Entities.Enemy.DataCategory;


namespace Teotihuacan.GameData
{
    public class Level5Spawns : LevelSpawnsBase
    {
        public Level5Spawns()
        {
            var wave = AddWave();

            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter,
                Shooter
            );

            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter,
                Shooter, Suicider, Suicider
            );

            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter,
                Shooter, Suicider, Suicider
            );

            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter,
                Shooter, Shooter, Shooter,
                Shooter, Suicider, Suicider
            )
            .CanSpawnAtMultipleSpots = true ;


            ////////////////////////////////////////

            wave = AddWave();

            wave.AddMiniWave
            (
                ShooterSpread, ShooterSpread
            );

            wave.AddMiniWave
            (
                ShooterSpread, ShooterSpread, ShooterSpread,
                Suicider, Suicider, Suicider
            )
            .CanSpawnAtMultipleSpots = true;

            wave.AddMiniWave
            (
                SuiciderFast, SuiciderFast, SuiciderFast,
                SuiciderFast, SuiciderFast, SuiciderFast
            );


            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter,
                ShooterSpread, ShooterSpread, ShooterSpread
            );

            wave.AddMiniWave
            (
                Boss,
                ShooterSpread, ShooterSpread, ShooterSpread
            );


            ////////////////////////////////////////

            wave = AddWave();


            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter
            );

            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter,
                Shooter, Shooter, Shooter
            );

            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter,
                Shooter, Shooter, Shooter,
                Shooter, Shooter, Shooter

            )
            .CanSpawnAtMultipleSpots = true;


            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter,
                Suicider, Suicider, Suicider,
                Suicider, Suicider, Suicider

            )
            .CanSpawnAtMultipleSpots = true;

            ////////////////////////////////////////

            wave = AddWave();

            wave.AddMiniWave
            (
                ShooterSpread, ShooterSpread, ShooterSpread
            )
            .CanSpawnAtMultipleSpots = true;

            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter,
                Suicider, Suicider
            )
            .CanSpawnAtMultipleSpots = true;

            wave.AddMiniWave
            (
                SuiciderFast, SuiciderFast, SuiciderFast, SuiciderFast,
                ShooterSpread, ShooterSpread, ShooterSpread,
                Shooter, Shooter
            )
            .CanSpawnAtMultipleSpots = true;

            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter, Shooter, Shooter, 
                SuiciderFast, SuiciderFast, SuiciderFast, SuiciderFast,
                ShooterSpread, ShooterSpread, ShooterSpread,
                Suicider, Suicider
            )
            .CanSpawnAtMultipleSpots = true;

            ////////////////////////////////////////

            wave = AddWave();


            wave.AddMiniWave
            (
                ShooterSpread, ShooterSpread, ShooterSpread, ShooterSpread,
                Boss, Boss
            )
            .CanSpawnAtMultipleSpots = true;

        }
    }
}
