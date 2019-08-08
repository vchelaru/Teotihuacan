using FlatRedBall.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teotihuacan.Entities;
using Teotihuacan.GameData;
using Teotihuacan.Models;

namespace Teotihuacan.Managers
{
    public static class PlayerWeaponLevelManager
    {
        public static PlayerData[] PlayersData = new PlayerData[4];
        public static List<IInputDevice> ConnectedDevices = new List<IInputDevice>();

        // TODO: check who is using this !
        public static void CreateNewWeaponLevel(Player playerEntity)
        {
            var playerData = new PlayerData(playerEntity);
            playerData.InitializeAllWeapons();

            PlayersData[playerEntity.Index] = playerData;
        }

        /*public static void AddUniqueInputDevice(IInputDevice input)
        {
            if(!ConnectedDevices.Contains(input))
            {
                ConnectedDevices.Add(input);
            }
        }*/

        public static void SaveAll()
        {
            for (int i = 0; i < Screens.GameScreen.MaxNumberOfPlayers; i++)
            {
                var playerData = PlayersData[i];
                if (playerData != null)
                {
                    DataLoader.SaveData(playerData);
                }
            }
        }

        public static void ClearAll()
        {
            DataLoader.Delete(0);
            DataLoader.Delete(1);
            DataLoader.Delete(2);
            DataLoader.Delete(3);

            Array.Clear(PlayersData, 0, PlayersData.Length);
        }

        /*private static string GetNameFromInputDevice(IInputDevice inputDevice)
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
        }*/

        /*public static PlayerData LoadForInputDevice(IInputDevice inputDevice)
        {
            var name = GetNameFromInputDevice(inputDevice);

            return DataLoader.LoadData(name);
        }*/
        public static PlayerData LoadPlayerData(int playerIndex)
        {
            return DataLoader.LoadData(playerIndex);
        }
    }
}
