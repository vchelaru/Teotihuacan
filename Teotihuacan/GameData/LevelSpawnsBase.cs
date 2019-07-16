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
    }

    public class LevelSpawnsBase
    {
        public List<Wave> Waves { get; private set; } = new List<Wave>();
    }
}
