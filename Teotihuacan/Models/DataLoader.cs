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

            var path = PathFromName(playerData.SlotIndex);

            var directory = FileManager.GetDirectory(path);
            System.IO.Directory.CreateDirectory(directory);

            System.IO.File.WriteAllText(path, serializedData);
        }

        /// <summary>Returns PlayerData for given slot index or null if data do not exist on disk.</summary>
        /// <param name="slotIndex"></param>
        /// <returns>PlayerData for given slot index or null if data do not exist on disk.</returns>
        public static PlayerData TryLoadData(int slotIndex)
        {
            PlayerData playerData = null;

            var path = PathFromName(slotIndex);

            if (System.IO.File.Exists(path))
            {
                var serializedData = System.IO.File.ReadAllText(path);

                playerData = JsonConvert.DeserializeObject<PlayerData>(serializedData);
            }
            return playerData;
        }

        public static void Delete(int slotIndex)
        {
            var path = PathFromName(slotIndex);

            if(System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }


        static string PathFromName(int slotIndex)
        {
            return "Data/" + slotIndex + ".data";
        }
    }
}
