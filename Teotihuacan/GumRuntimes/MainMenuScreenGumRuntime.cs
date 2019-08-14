    using System;
    using System.Collections.Generic;
    using System.Linq;
using Teotihuacan.Managers;
using Teotihuacan.Models;

namespace Teotihuacan.GumRuntimes
    {
        public partial class MainMenuScreenGumRuntime
        {
            public List<PlayerHUDRuntime> PlayerHuds = new List<PlayerHUDRuntime>();
            public List<PlayerHUDJoinRuntime> PlayerJoinHuds = new List<PlayerHUDJoinRuntime>();

            partial void CustomInitialize() 
            {
                //PauseMenuInstance.ResumeClicked += (not, used) => ResumeClicked(this, null);
                //PauseMenuInstance.QuitClicked += (not, used) => QuitClicked(this, null);
                //PauseMenuInstance.ClearDataClicked += (not, used) => ClearDataClicked(this, null);

                PlayerHuds.Add(PlayerHUDInstance);
                PlayerHuds.Add(PlayerHUDInstance1);
                PlayerHuds.Add(PlayerHUDInstance2);
                PlayerHuds.Add(PlayerHUDInstance3);
                PlayerHUDInstance.Visible = false;
                PlayerHUDInstance1.Visible = false;
                PlayerHUDInstance2.Visible = false;
                PlayerHUDInstance3.Visible = false;

                PlayerJoinHuds.Add(PlayerHUDJoinInstance);
                PlayerJoinHuds.Add(PlayerHUDJoinInstance1);
                PlayerJoinHuds.Add(PlayerHUDJoinInstance2);
                PlayerJoinHuds.Add(PlayerHUDJoinInstance3);
                PlayerHUDJoinInstance.Visible = false;
                PlayerHUDJoinInstance1.Visible = false;
                PlayerHUDJoinInstance2.Visible = false;
                PlayerHUDJoinInstance3.Visible = false;
            }

            public void SetJoinHUDsVisibility(int numberOfControlsConnected)
            {
                int numberOfJoingHUDs = Math.Min(PlayerManager.MaxNumberOfPlayers, numberOfControlsConnected);

                int slotIndex = 0;
                for (; slotIndex < numberOfJoingHUDs; slotIndex++)
                {
                    PlayerJoinHuds[slotIndex].Visible = true;
                }
                for (; slotIndex < PlayerManager.MaxNumberOfPlayers; slotIndex++)
                {
                    PlayerJoinHuds[slotIndex].Visible = false;
                }
            }

            public void RefreshExperienceBar(PlayerData playerSlotData)
            {
                var hud = PlayerHuds[playerSlotData.SlotIndex];

                hud.RefreshExperienceBar(playerSlotData);
            }


        }
    }
