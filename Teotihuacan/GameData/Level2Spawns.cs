using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Teotihuacan.Entities.Enemy.DataCategory;

namespace Teotihuacan.GameData
{
    public class Level2Spawns : LevelSpawnsBase
    {
        public Level2Spawns()
        {
            var wave = AddWave();
            wave.AddMiniWave
            (
                Shooter, Shooter
            );
            wave.AddMiniWave
            (
                Shooter, Shooter
            );

            wave = AddWave();
            wave.AddMiniWave
            (
                Shooter, Shooter
            )
            .CanSpawnAtMultipleSpots = true;
            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter
            )
            .CanSpawnAtMultipleSpots=true;

            ///////////////////////////////////

            wave = AddWave();
            wave.AddMiniWave
            (
                Shooter, Suicider, Shooter,
                Suicider, Shooter, Suicider
            )
            .CanSpawnAtMultipleSpots = true;

            ///////////////////////////////////

            wave = AddWave();

            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter,
                Shooter
            );

            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter,
                Shooter
            );

            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter,
                Shooter
            ).CanSpawnAtMultipleSpots = true;


            wave.AddMiniWave
            (
                Shooter, Shooter, Shooter,
                Suicider, Shooter
            ).CanSpawnAtMultipleSpots = true;

            ///////////////////////////////////

            wave = AddWave();

            wave.AddMiniWave
            (
                Suicider, Suicider, Suicider,
                Suicider, Suicider, Suicider
            );

            wave.AddMiniWave
            (
                Suicider, Suicider, Suicider,
                Suicider, Suicider, Suicider
            ).CanSpawnAtMultipleSpots = true;

            ///////////////////////////////////

            wave = AddWave();

            wave.AddMiniWave
            (
                Boss
            );
        }
    }
}
