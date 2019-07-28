using FlatRedBall.IO;
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

            var path = PathFromName(playerName);

            var directory = FileManager.GetDirectory(path);
            System.IO.Directory.CreateDirectory(directory);

            System.IO.File.WriteAllText(path, serialized);
        }

        public static PlayerData LoadData(string playerName)
        {
            PlayerData playerData = null;

            var path = PathFromName(playerName);

            if (System.IO.File.Exists(path))
            {
                var serialized = System.IO.File.ReadAllText(path);

                playerData = JsonConvert.DeserializeObject<PlayerData>(serialized);
            }
            return playerData;
        }

        static string PathFromName(string playerName)
        {
            return "Data/" + playerName + ".data";
        }
    }
}
