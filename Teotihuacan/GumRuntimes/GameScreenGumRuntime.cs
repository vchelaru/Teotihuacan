using FlatRedBall.Input;
using FlatRedBall.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using Teotihuacan.Entities;
using Teotihuacan.Managers;

namespace Teotihuacan.GumRuntimes
{
    public enum UpdateType
    {
        Interpolate,
        Instant
    }
    public partial class GameScreenGumRuntime
    {

        #region Fields/Properties

        public List<PlayerHUDRuntime> PlayerHuds = new List<PlayerHUDRuntime>();
        public List<PlayerHUDJoinRuntime> PlayerJoinHuds = new List<PlayerHUDJoinRuntime>();
        public List<PlayerHUDDeadRuntime> PlayerDeadHuds = new List<PlayerHUDDeadRuntime>();

        #endregion

        #region Events/Actions

        public event EventHandler ResumeClicked;
        public event EventHandler QuitClicked;
        public event EventHandler ClearDataClicked;

        public event Action StartLevel;

        #endregion

        partial void CustomInitialize()
        {
            PauseMenuInstance.ResumeClicked += (not, used) => ResumeClicked(this, null);
            PauseMenuInstance.QuitClicked += (not, used) => QuitClicked(this, null);
            PauseMenuInstance.ClearDataClicked += (not, used) => ClearDataClicked(this, null);

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

            PlayerDeadHuds.Add(PlayerHUDDeadInstance);
            PlayerDeadHuds.Add(PlayerHUDDeadInstance1);
            PlayerDeadHuds.Add(PlayerHUDDeadInstance2);
            PlayerDeadHuds.Add(PlayerHUDDeadInstance3);
            PlayerHUDDeadInstance.Visible = false;
            PlayerHUDDeadInstance1.Visible = false;
            PlayerHUDDeadInstance2.Visible = false;
            PlayerHUDDeadInstance3.Visible = false;
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



        public void CustomActivity(PositionedObjectList<Player> players, PlayerBase playerBase/*, List<IInputDevice> deadPlayerInputDevices*/)
        {
            // Update players' HUDs
            for (int playerSlotIndex = 0; playerSlotIndex < PlayerManager.MaxNumberOfPlayers; playerSlotIndex++)
            {
                var slotData = PlayerManager.PlayersSlots[playerSlotIndex];

                if (slotData != null && slotData.SlotState == Models.PlayerData.eSlotState.Full)
                {
                    // active player slot
                    var hud = PlayerHuds[playerSlotIndex];

                    switch (slotData.EquippedWeapon)
                    {
                        case Animation.Weapon.ShootingFire:
                            hud.CurrentWeaponCategoryState = PlayerHUDRuntime.WeaponCategory.Fireball;
                            break;
                        case Animation.Weapon.ShootingLightning:
                            hud.CurrentWeaponCategoryState = PlayerHUDRuntime.WeaponCategory.Lightning;
                            break;
                        case Animation.Weapon.ShootingSkull:
                            hud.CurrentWeaponCategoryState = PlayerHUDRuntime.WeaponCategory.Skull;
                            break;
                    }

                    hud.UpdateStatusBars(players.First(p => p.PlayerData.SlotIndex == playerSlotIndex));
                }
            }

            /*// Update join HUDs' visibility
            for (int i = 0; i < 4; i++)
            {
                if (!deadPlayerInputDevices.Contains(InputManager.Xbox360GamePads[i]))
                {
                    playerJoinHuds[i].Visible = !playerHuds[i].Visible &&
                        FlatRedBall.Input.InputManager.Xbox360GamePads[i].IsConnected;
                }
            }*/

            // Update Base's HUD
            BaseHUDInstance.UpdateHealth(playerBase);
        }

        public void ShowGameOverOverlay(Screens.GameScreen currentScreen)
        {
            this.GameOverInstance.Visible = true;

            //this.GameOverInstance.CurrentVisualsAlphaState = GameOverRuntime.VisualsAlpha.Transparent;
            this.GameOverInstance.PopupAppearAnimation.Play();
            this.GameOverInstance.PopupAppearAnimation.EndReached += () =>
            {
                GameOverInstance.FadeToBlackAnimation.PlayAfter(3);
                GameOverInstance.FadeToBlackAnimation.EndReached += () =>
                {
                    currentScreen.RestartScreen(false);
                };
            };
        }

        public void ShowLevelStartOverlay(string levelName)
        {
            LevelStartInstance.Visible = true;
            LevelStartInstance.LevelNameText = levelName;
            LevelStartInstance.FadeInAndCountDownAnimation.EndReached += () =>
            {
                LevelStartInstance.Visible = false;
                StartLevel();
            };
            LevelStartInstance.FadeInAndCountDownAnimation.Play();
        }

        public void ShowWaveStateOverlay(string message)
        {
            WaveStateInstance.WaveMessageText = message;
            WaveStateInstance.Visible = true;
        }

        public void HideWaveStateOverlay()
        {
            WaveStateInstance.Visible = false;
        }

        public void ShowPauseMenu()
        {
            this.PauseMenuInstance.Visible = true;

            // BUG: this seem to get the game stuck (create endless loop ?)
            // Also GodFloats animation doesn't play
            //this.PauseMenuInstance.GodFloatsAnimation.Play();
        }

        public void HidePauseMenu()
        {
            this.PauseMenuInstance.Visible = false;
            //this.PauseMenuInstance.GodFloatsAnimation.Stop();
        }

        public void RefreshExperienceBar(Player player, UpdateType updateType, bool isLevelUp)
        {
            var hud = PlayerHuds[player.PlayerData.SlotIndex];

            hud.RefreshExperienceBar(player, updateType, isLevelUp);
        }
    }
}
