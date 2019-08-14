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
    public static class PlayerManager
    {
        public const int MaxNumberOfPlayers = 4;

        public static PlayerData[] PlayersSlots = new PlayerData[MaxNumberOfPlayers];
        //public static List<IInputDevice> ConnectedDevices = new List<IInputDevice>();
        //public static List<PlayerData> DeadPlayers = new List<PlayerData>(MaxNumberOfPlayers);

        public static List<PlayerData> ActivePlayers = new List<PlayerData>(MaxNumberOfPlayers);


        public static bool TryAssignSlotToPlayer(InputControls inputControls, out PlayerData playerData)
        {
            // pass 1 - try find existing slot reserved for my InputControls
            for (int slotIndex = 0; slotIndex < MaxNumberOfPlayers; slotIndex++)
            {
                var slotPlayerData = PlayersSlots[slotIndex];
                if (slotPlayerData != null
                    &&
                    (slotPlayerData.SlotState == PlayerData.eSlotState.Reserved_Disconnect
                     ||
                     slotPlayerData.SlotState == PlayerData.eSlotState.Reserved_Left)
                    &&
                    slotPlayerData.InputControls.ControlsID == inputControls.ControlsID)
                {
                    playerData = slotPlayerData;
                    playerData.InputControls = inputControls;
                    playerData.SlotState = PlayerData.eSlotState.Full;
                    ActivePlayers.Add(playerData);
                    return true;
                }
            }

            // pass 2 - try find free slot 
            for (int slotIndex = 0; slotIndex < MaxNumberOfPlayers; slotIndex++)
            {
                /*var slotPlayerData = PlayersSlots[slotIndex];

                if (slotPlayerData == null)*/
                if (PlayersSlots[slotIndex] == null)
                {
                    // Try load player stats
                    playerData = DataLoader.TryLoadData(slotIndex);

                    // Create new player stats
                    if (playerData == null)
                    {
                        playerData = new PlayerData(slotIndex, inputControls);
                        //playerData.SlotIndex = slotIndex;
                    }
                    else
                    {
                        playerData.InputControls = inputControls;
                    }
                    
                    playerData.SlotState = PlayerData.eSlotState.Full;

                    PlayersSlots[slotIndex] = playerData;
                    ActivePlayers.Add(playerData);

                    return true;
                }
                /*else if (slotPlayerData.SlotState == PlayerData.eSlotState.Free)
                {
                    playerData = slotPlayerData;
                    playerData.InputControls = inputControls;
                    playerData.SlotState = PlayerData.eSlotState.Full;
                    return true;
                }*/
            }

            // pass 3-1 - try find any reserved-abandoned slot to take over
            for (int slotIndex = 0; slotIndex < MaxNumberOfPlayers; slotIndex++)
            {
                var slotPlayerData = PlayersSlots[slotIndex];
                if (slotPlayerData.SlotState == PlayerData.eSlotState.Reserved_Left)
                {
                    playerData = slotPlayerData;
                    playerData.InputControls = inputControls;
                    playerData.SlotState = PlayerData.eSlotState.Full;
                    ActivePlayers.Add(playerData);
                    return true;
                }
            }

            // pass 3-2 - try find reserved-disconnected slot to take over
            for (int slotIndex = 0; slotIndex < MaxNumberOfPlayers; slotIndex++)
            {
                var slotPlayerData = PlayersSlots[slotIndex];
                if (slotPlayerData.SlotState == PlayerData.eSlotState.Reserved_Disconnect)
                {
                    playerData = slotPlayerData;
                    playerData.InputControls = inputControls;
                    playerData.SlotState = PlayerData.eSlotState.Full;
                    ActivePlayers.Add(playerData);
                    return true;
                }
            }

            playerData = null;
            return false;
        }

        //public static void AddDeadPlayer(int slotIndex)
        public static void SetPlayerDead(PlayerData deadPlayerData)
        {
            //DeadPlayers.Add(deadPlayerData);
            ActivePlayers.Remove(deadPlayerData);
            deadPlayerData.SlotState = PlayerData.eSlotState.Full_PlayerDead;
        }

        public static void SetPlayerInactive(PlayerData playerData)
        {
            ActivePlayers.Remove(playerData);
        }

        /*public static IEnumerable<PlayerData> FindDisconnectedPlayers()
        {
            foreach (var playerSlot in PlayersSlots)
            {
                if (playerSlot != null && playerSlot.InputControls.AreConnected == false)
                {
                    yield return playerSlot;
                }
            }
        }*/

        // TODO: check who is using this !
        /*public static void CreateNewWeaponLevel(Player playerEntity)
        {
            var playerData = new PlayerData(playerEntity);
            playerData.InitializeAllWeapons();

            PlayersSlots[playerEntity.PlayerData.SlotIndex] = playerData;
        }*/

        /*public static void AddUniqueInputDevice(IInputDevice input)
        {
            if(!ConnectedDevices.Contains(input))
            {
                ConnectedDevices.Add(input);
            }
        }*/

        public static void SaveAll()
        {
            for (int slotIndex = 0; slotIndex < MaxNumberOfPlayers; slotIndex++)
            {
                var playerData = PlayersSlots[slotIndex];
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

            Array.Clear(PlayersSlots, 0, PlayersSlots.Length);
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

        /*public static PlayerData TryLoadPlayerData(int slotIndex)
        {
            return DataLoader.TryLoadData(slotIndex);
        }*/
    }
}
