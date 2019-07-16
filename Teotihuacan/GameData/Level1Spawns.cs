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
            this.Waves.Add(
            new Wave
            {
                MiniWaves = new List<MiniWave>
                {
                    new MiniWave
                    {
                        Spawns = new List<Entities.Enemy.DataCategory>
                        {
                            Entities.Enemy.DataCategory.Shooter,
                        }
                    },
                    new MiniWave
                    {
                        Spawns = new List<Entities.Enemy.DataCategory>
                        {
                            Entities.Enemy.DataCategory.Shooter,
                        }
                    },
                }
            });

            this.Waves.Add(
            new Wave
            {
                MiniWaves = new List<MiniWave>
                {
                    new MiniWave
                    {
                        Spawns = new List<Entities.Enemy.DataCategory>
                        {
                            Entities.Enemy.DataCategory.Shooter,
                            Entities.Enemy.DataCategory.Shooter,
                            Entities.Enemy.DataCategory.Shooter,
                        }
                    },
                    new MiniWave
                    {
                        Spawns = new List<Entities.Enemy.DataCategory>
                        {
                            Entities.Enemy.DataCategory.Shooter,
                            Entities.Enemy.DataCategory.Shooter,
                            Entities.Enemy.DataCategory.Shooter,
                            Entities.Enemy.DataCategory.Shooter,
                        }
                    },
                }
            });

            this.Waves.Add(
            new Wave
            {
                MiniWaves = new List<MiniWave>
                {
                    new MiniWave
                    {
                        Spawns = new List<Entities.Enemy.DataCategory>
                        {
                            Entities.Enemy.DataCategory.Shooter,
                            Entities.Enemy.DataCategory.Shooter,
                            Entities.Enemy.DataCategory.Shooter,
                            Entities.Enemy.DataCategory.Shooter,
                        }
                    },
                    new MiniWave
                    {
                        Spawns = new List<Entities.Enemy.DataCategory>
                        {
                            Entities.Enemy.DataCategory.Shooter,
                            Entities.Enemy.DataCategory.Shooter,
                            Entities.Enemy.DataCategory.Shooter,
                            Entities.Enemy.DataCategory.Shooter,
                            Entities.Enemy.DataCategory.Shooter,
                            Entities.Enemy.DataCategory.Shooter,
                        }
                    },
                }
            });
        }
    }
}
