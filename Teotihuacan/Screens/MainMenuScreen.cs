using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Localization;
using Teotihuacan.Managers;
using Teotihuacan.Models;
using Teotihuacan.GumRuntimes;

namespace Teotihuacan.Screens
{
	public partial class MainMenuScreen
	{
        MainMenuScreenGumRuntime MainMenuScreenGumRuntime;

        void CustomInitialize()
		{
             MainMenuScreenGumRuntime = MainMenuScreenGum as MainMenuScreenGumRuntime;

            int numberOfControlsConnected = 1; // 1 for keyboard+mouse
            foreach (var gamePad in InputManager.Xbox360GamePads)
            {
                if (gamePad.IsConnected)
                    numberOfControlsConnected++;
            }
            MainMenuScreenGumRuntime.SetJoinHUDsVisibility(numberOfControlsConnected);
        }

		void CustomActivity(bool firstTimeCalled)
		{
            PlayersJoinLeaveActivity();
        }

        private void PlayersJoinLeaveActivity()
        {
            // -- Joining
            if (PlayerManager.ActivePlayers.Count < PlayerManager.MaxNumberOfPlayers)
            {
                Xbox360GamePad gamePad;
                for (int gamepadIndex = 0; gamepadIndex < InputManager.Xbox360GamePads.Length; gamepadIndex++)
                {
                    gamePad = InputManager.Xbox360GamePads[gamepadIndex];
                    if (gamePad.ButtonPushed(InputControls.Xbox360GamePad_Button_Join)
                        &&
                        CanPlayerJoin(gamepadIndex))
                    {
                        PlayerData slotPlayerData;
                        if (PlayerManager.TryAssignSlotToPlayer(new Xbox360GamePadControls(gamePad, gamepadIndex), out slotPlayerData))
                        {
                            // join success
                            SetPlayerHudOnJoin(slotPlayerData);
                        }
                    }
                }

                if (InputManager.Keyboard.KeyPushed(InputControls.KeyboardAndMouse_Button_Join)
                    &&
                    CanPlayerJoin(InputControls.KeyboardAndMouse_ControlsID))
                {
                    PlayerData slotPlayerData;
                    if (PlayerManager.TryAssignSlotToPlayer(new KeyboardMouseControls(), out slotPlayerData))
                    { 
                        // join success
                        SetPlayerHudOnJoin(slotPlayerData);
                    }
                }
            }

            // -- Leaving & Disconnects
            //foreach (var playerSlotData in PlayerManager.ActivePlayers)
            PlayerData playerSlotData;
            for (int i = PlayerManager.ActivePlayers.Count - 1; i > -1; i--)
            {
                playerSlotData = PlayerManager.ActivePlayers[i];

                if (playerSlotData.InputControls.AreConnected == false)
                {
                    // player disconnected, so pause ? and drop the player:
                    playerSlotData.SlotState = PlayerData.eSlotState.Reserved_Disconnect;
                    DropPlayer(playerSlotData);
                }
                else if (playerSlotData.InputControls.WasLeaveJustPressed)
                {
                    // player wants to leave
                    playerSlotData.SlotState = PlayerData.eSlotState.Reserved_Left;
                    DropPlayer(playerSlotData);
                }
            }
        }

        private bool CanPlayerJoin(int controlsID)
        {
            foreach (var slotData in PlayerManager.ActivePlayers)
            {
                if (slotData.InputControls.ControlsID == controlsID)
                    return false;
            }

            return true;
        }

        private void SetPlayerHudOnJoin(PlayerData playerSlotData)
        {
            //var playerSlotIndex = 
            var mainMenuScreenGumRuntime = MainMenuScreenGum as MainMenuScreenGumRuntime;

            mainMenuScreenGumRuntime.PlayerJoinHuds[playerSlotData.SlotIndex].Visible = false;
            mainMenuScreenGumRuntime.PlayerHuds[playerSlotData.SlotIndex].Visible = true;

            mainMenuScreenGumRuntime.RefreshExperienceBar(playerSlotData);
        }

        private void DropPlayer(PlayerData playerSlotData)
        {
            PlayerManager.SetPlayerInactive(playerSlotData);

            var mainMenuScreenGumRuntime = MainMenuScreenGum as MainMenuScreenGumRuntime;

            mainMenuScreenGumRuntime.PlayerHuds[playerSlotData.SlotIndex].Visible = false;
            mainMenuScreenGumRuntime.PlayerJoinHuds[playerSlotData.SlotIndex].Visible = true;
        }



        void CustomDestroy() { }

        static void CustomLoadStaticContent(string contentManagerName) { }

	}
}
