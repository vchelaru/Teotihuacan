using FlatRedBall.Input;
using FlatRedBall.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using Teotihuacan.Entities;

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

        List<PlayerHUDRuntime> playerHuds = new List<PlayerHUDRuntime>();
        List<PlayerHUDJoinRuntime> playerJoinHuds = new List<PlayerHUDJoinRuntime>();

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
        }

        public void SetNumberOfPlayers(int numberOfPlayers)
        {
            this.PlayerHUDInstance.Visible = numberOfPlayers > 0;
            this.PlayerHUDInstance1.Visible = numberOfPlayers > 1;
            this.PlayerHUDInstance2.Visible = numberOfPlayers > 2;
            this.PlayerHUDInstance3.Visible = numberOfPlayers > 3;

            playerHuds.Add(PlayerHUDInstance);
            playerHuds.Add(PlayerHUDInstance1);
            playerHuds.Add(PlayerHUDInstance2);
            playerHuds.Add(PlayerHUDInstance3);

            playerJoinHuds.Add(PlayerHUDJoinInstance);
            playerJoinHuds.Add(PlayerHUDJoinInstance1);
            playerJoinHuds.Add(PlayerHUDJoinInstance2);
            playerJoinHuds.Add(PlayerHUDJoinInstance3);
        }



        public void CustomActivity(PositionedObjectList<Player> players, PlayerBase playerBase, List<IInputDevice> deadPlayerInputDevices)
        {
            // make all huds invisible, will be made visible below:
            foreach(var hud in playerHuds)
            {
                hud.Visible = false;
            }

            foreach(var player in players)
            {
                var index = player.CurrentColorCategoryState.ToInt();
                var hud = playerHuds[index];

                hud.Visible = true;
                switch(player.EquippedWeapon)
                {
                    case Animation.Weapon.ShootingFire:
                        hud.CurrentWeaponCategoryState = PlayerHUDRuntime.WeaponCategory.Fireball;
                        break;
                    case Animation.Weapon.ShootingLightning:
                        hud.CurrentWeaponCategoryState = PlayerHUDRuntime.WeaponCategory.Lightning;
                        break;
                    case Animation.Weapon.ShootingSkulls:
                        hud.CurrentWeaponCategoryState = PlayerHUDRuntime.WeaponCategory.Skull;

                        break;
                }
                hud.UpdateStatusBars(player);
            }

            for(int i = 0; i < 4; i++)
            {
                if (!deadPlayerInputDevices.Contains(InputManager.Xbox360GamePads[i]))
                {
                    playerJoinHuds[i].Visible = !playerHuds[i].Visible &&
                        FlatRedBall.Input.InputManager.Xbox360GamePads[i].IsConnected;
                }
                else
                {
                    playerJoinHuds[i].Visible = false;
                    playerHuds[i].Visible = false;
                }
            }

            if(deadPlayerInputDevices.Contains(InputManager.Keyboard))
            {
                //Assume the keyboad player is in slot 0;
                playerJoinHuds[0].Visible = false;
                playerHuds[0].Visible = false;
            }

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

        public void SetPauseMenuVisibility(bool isVisible)
        {
            if(isVisible)
            {
                // no animations?
                this.PauseMenuInstance.Visible = true;
            }
            else
            {
                this.PauseMenuInstance.Visible = false;
            }
        }

        public void RefreshExperienceBar(Player player, UpdateType updateType, bool isLevelUp)
        {
            var index = player.CurrentColorCategoryState.ToInt();
            var hud = playerHuds[index];

            hud.RefreshExperienceBar(player, updateType, isLevelUp);
        }
    }
}
