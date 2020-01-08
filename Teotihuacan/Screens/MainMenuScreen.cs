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

            UpdateStartButton();

            MainMenuScreenGumRuntime.IntroFadeInAnimation.EndReached += IntroFadeInAnimation_EndReached;
            MainMenuScreenGumRuntime.IntroFadeInAnimation.Play();
        }

        private void IntroFadeInAnimation_EndReached()
        {
            MainMenuScreenGumRuntime.IntroFadeInAnimation.EndReached -= IntroFadeInAnimation_EndReached;

            MainMenuScreenGumRuntime.QuitClicked = FlatRedBallServices.Game.Exit;
            MainMenuScreenGumRuntime.ClearDataClicked = () =>
            {
                PlayerManager.ClearAll();

                // TODO: reset HUDs
            };
            MainMenuScreenGumRuntime.StartGameClicked = () => MoveToScreen(typeof(Level1));

            //LevelStartInstance.Visible = false;
            MainMenuScreenGumRuntime.QuitButtonInstance.FormsControl.IsEnabled = true;
            MainMenuScreenGumRuntime.QuitButtonInstance.Visible = true;
            MainMenuScreenGumRuntime.ClearDataButtonInstance.FormsControl.IsEnabled = true;
            MainMenuScreenGumRuntime.ClearDataButtonInstance.Visible = true;
            MainMenuScreenGumRuntime.StartButtonInstance.Visible = true;

            // Fix for button showing as enabled even when it's IsEnabled = false:
            //bool startButtonInstanceIsEnabled = MainMenuScreenGumRuntime.StartButtonInstance.FormsControl.IsEnabled;
            //MainMenuScreenGumRuntime.StartButtonInstance.FormsControl.IsEnabled = !startButtonInstanceIsEnabled;
            //MainMenuScreenGumRuntime.StartButtonInstance.FormsControl.IsEnabled = startButtonInstanceIsEnabled;
            if (!MainMenuScreenGumRuntime.StartButtonInstance.FormsControl.IsEnabled)
            {
                MainMenuScreenGumRuntime.StartButtonInstance.FormsControl.IsEnabled = true;
                MainMenuScreenGumRuntime.StartButtonInstance.FormsControl.IsEnabled = false;
            }

            int numberOfControlsConnected = 1; // 1 for keyboard+mouse always connected
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
                        if (PlayerManager.TryAssignSlotToPlayerInMainMenu(new Xbox360GamePadControls(gamePad, gamepadIndex), out slotPlayerData))
                        {
                            // join success
                            JoinPlayer(slotPlayerData);
                        }
                    }
                }

                if (InputManager.Keyboard.KeyPushed(InputControls.KeyboardAndMouse_Button_Join)
                    &&
                    CanPlayerJoin(InputControls.KeyboardAndMouse_ControlsID))
                {
                    PlayerData slotPlayerData;
                    if (PlayerManager.TryAssignSlotToPlayerInMainMenu(new KeyboardMouseControls(), out slotPlayerData))
                    {
                        // join success
                        JoinPlayer(slotPlayerData);
                    }
                }
            }

            // -- Leaving & Disconnects
            //foreach (var playerSlotData in PlayerManager.ActivePlayers)
            PlayerData playerSlotData;
            for (int i = PlayerManager.ActivePlayers.Count - 1; i > -1; i--)
            {
                playerSlotData = PlayerManager.ActivePlayers[i];

                if (playerSlotData.InputControls.AreConnected == false || playerSlotData.InputControls.WasLeaveJustPressed)
                    DropPlayer(playerSlotData, PlayerData.eSlotState.Free);
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
            //var mainMenuScreenGumRuntime = MainMenuScreenGum as MainMenuScreenGumRuntime;

            MainMenuScreenGumRuntime.PlayerJoinHuds[playerSlotData.SlotIndex].Visible = false;
            
            MainMenuScreenGumRuntime.PlayerHuds[playerSlotData.SlotIndex].Visible = true;
            MainMenuScreenGumRuntime.RefreshExperienceBar(playerSlotData);
            
            ControlsIconRuntime playerControlsIcon = MainMenuScreenGumRuntime.PlayerControlsIcons[playerSlotData.SlotIndex];
            if (playerSlotData.InputControls.ControlsID == InputControls.KeyboardAndMouse_ControlsID)
            {
                playerControlsIcon.CurrentTypeCathegoryState = ControlsIconRuntime.TypeCathegory.KBM;
            }
            else
            {
                playerControlsIcon.CurrentTypeCathegoryState = ControlsIconRuntime.TypeCathegory.Controler;
                playerControlsIcon.ControlerIndexText = (playerSlotData.InputControls.ControlsID + 1).ToString();
            }
            playerControlsIcon.Visible = true;
        }

        private void JoinPlayer(PlayerData playerSlotData)
        {
            UpdateStartButton();

            SetPlayerHudOnJoin(playerSlotData);
        }

        private void DropPlayer(PlayerData playerSlotData, PlayerData.eSlotState reason)
        {
            PlayerManager.SetPlayerRemoved(playerSlotData, reason);

            MainMenuScreenGumRuntime.PlayerHuds[playerSlotData.SlotIndex].Visible = false;
            MainMenuScreenGumRuntime.PlayerJoinHuds[playerSlotData.SlotIndex].Visible = true;
            MainMenuScreenGumRuntime.PlayerControlsIcons[playerSlotData.SlotIndex].Visible = false;

            UpdateStartButton();
        }

        void UpdateStartButton()
        {
            // TODO: how to disable button control

            //if (PlayerManager.ActivePlayers.Count == 0)
            //{
            //    //MainMenuScreenGumRuntime.StartButtonInstance
            //    //    .CurrentButtonCategoryState =
            //    //        GumRuntimes.Menus.MenuParts.AztecMenuButtonRuntime.ButtonCategory.Disabled;

            //    //MainMenuScreenGumRuntime.StartButtonInstance.FormsControl.IsEnabled = false;

            //    //MainMenuScreenGumRuntime.StartButtonInstance.
            //}

            bool canStartGame = PlayerManager.ActivePlayers.Count > 0;
            MainMenuScreenGumRuntime.StartButtonInstance.FormsControl.IsEnabled = canStartGame;
            MainMenuScreenGumRuntime.StartButtonInstance.Enabled = canStartGame;
        }


        void CustomDestroy() { }

        static void CustomLoadStaticContent(string contentManagerName) { }
	}
}
