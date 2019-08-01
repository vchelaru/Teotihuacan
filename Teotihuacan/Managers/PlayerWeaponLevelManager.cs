using FlatRedBall.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teotihuacan.GameData;
using Teotihuacan.Models;

namespace Teotihuacan.Managers
{
    public static class PlayerWeaponLevelManager
    {
        public static Dictionary<IInputDevice, PlayerData> PlayerWeaponLevels = new Dictionary<IInputDevice, PlayerData>();
        public static List<IInputDevice> ConnectedDevices = new List<IInputDevice>();

        public static void CreateNewWeaponLevelFromInputDevice(IInputDevice inputDevice)
        {
            var playerData = new PlayerData();
            playerData.InitializeAllWeapons();

            PlayerWeaponLevels.Add(inputDevice, playerData);
        }

        public static void AddUniqueInputDevice(IInputDevice input)
        {
            if(!ConnectedDevices.Contains(input))
            {
                ConnectedDevices.Add(input);
            }
        }

        public static void SaveAll()
        {
            foreach(var kvp in PlayerWeaponLevels)
            {
                var inputDevice = kvp.Key;
                string name = GetNameFromInputDevice(inputDevice);
                DataLoader.SaveData(kvp.Value, name);
            }
        }

        public static void ClearAll()
        {
            DataLoader.Delete("Keyboard");
            DataLoader.Delete("Gamepad0");
            DataLoader.Delete("Gamepad1");
            DataLoader.Delete("Gamepad2");
            DataLoader.Delete("Gamepad3");

            PlayerWeaponLevels.Clear();
        }

        private static string GetNameFromInputDevice(IInputDevice inputDevice)
        {
            string name = null;

            if (inputDevice is Keyboard)
            {
                name = "Keyboard";
            }
            else if (inputDevice is Xbox360GamePad gamePad)
            {
                name = "Gamepad" +
                    Array.IndexOf(InputManager.Xbox360GamePads, gamePad);
            }

            return name;
        }

        public static PlayerData LoadForInputDevice(IInputDevice inputDevice)
        {
            var name = GetNameFromInputDevice(inputDevice);

            return DataLoader.LoadData(name);
        }
    }
}
