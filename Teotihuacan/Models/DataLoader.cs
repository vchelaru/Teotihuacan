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
        public static void SaveData(PlayerData playerData)
        {
            var serializedData = JsonConvert.SerializeObject(playerData);

            var path = PathFromName(playerData.PlayerIndex);

            var directory = FileManager.GetDirectory(path);
            System.IO.Directory.CreateDirectory(directory);

            System.IO.File.WriteAllText(path, serializedData);
        }

        public static PlayerData LoadData(int playerIndex)
        {
            PlayerData playerData = null;

            var path = PathFromName(playerIndex);

            if (System.IO.File.Exists(path))
            {
                var serializedData = System.IO.File.ReadAllText(path);

                playerData = JsonConvert.DeserializeObject<PlayerData>(serializedData);
            }
            return playerData;
        }

        public static void Delete(int playerIndex)
        {
            var path = PathFromName(playerIndex);

            if(System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }


        static string PathFromName(int playerIndex)
        {
            return "Data/" + playerIndex + ".data";
        }
    }
}
