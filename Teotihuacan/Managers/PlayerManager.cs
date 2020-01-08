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

        /// <summary>
        /// Different behaviour than standard TryAssignSlotToPlayer() used for in game joining.
        /// Puts player in first free slot.
        /// Made so players can rearange themselfs as they want before the game starts.
        /// </summary>

        /// <returns>True if free slot was found and assigned to player.</returns>
        public static bool TryAssignSlotToPlayerInMainMenu(InputControls inputControls, out PlayerData playerData)
        {
            // pass 2 - try find free slot 
            for (int slotIndex = 0; slotIndex < MaxNumberOfPlayers; slotIndex++)
            {
                if (PlayersSlots[slotIndex] == null)
                {
                    // Try load player stats
                    playerData = DataLoader.TryLoadData(slotIndex);

                    // or Create new player stats
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
                else if (PlayersSlots[slotIndex].SlotState != PlayerData.eSlotState.Full)
                {
                    playerData = PlayersSlots[slotIndex];
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

        public static void SetPlayerRemoved(PlayerData playerData, PlayerData.eSlotState reason)
        {
            ActivePlayers.Remove(playerData);
            playerData.SlotState = reason;
        }
        

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
    }
}
