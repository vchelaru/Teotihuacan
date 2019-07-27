using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teotihuacan.Models
{
    public static class DataLoader
    {
        public static void SaveData(PlayerData playerData, string playerName)
        {
            var serialized = JsonConvert.SerializeObject(playerData);

            System.IO.File.WriteAllText(playerName + ".data", serialized);
        }

        public static PlayerData LoadData(string playerName)
        {
            PlayerData playerData = null;

            var path = playerName + ".data";

            if (System.IO.File.Exists(path))
            {
                var serialized = System.IO.File.ReadAllText(path);

                playerData = JsonConvert.DeserializeObject<PlayerData>(serialized);
            }
            return playerData;
        }
    }
}
