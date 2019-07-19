using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teotihuacan.GameData
{
    public class MiniWave
    {
        public List<Entities.Enemy.DataCategory> Spawns { get; set; }
    }

    public class Wave
    {
        public List<MiniWave> MiniWaves { get; set; }
        public void AddMiniWave(params Entities.Enemy.DataCategory[] categories)
        {
            var miniWave = new MiniWave();
            miniWave.Spawns = categories.ToList();
            if(this.MiniWaves == null)
            {
                this.MiniWaves = new List<MiniWave>();
            }
            this.MiniWaves.Add(miniWave);
        }
    }

    public class LevelSpawnsBase
    {
        public List<Wave> Waves { get; private set; } = new List<Wave>();

        public Wave AddWave()
        {
            var wave = new Wave();
            Waves.Add(wave);
            return wave;
        }
    }
}
