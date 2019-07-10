using FlatRedBall.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using Teotihuacan.Entities;

namespace Teotihuacan.GumRuntimes
{
    public partial class GameScreenGumRuntime
    {
        List<PlayerHUDRuntime> playerHuds = new List<PlayerHUDRuntime>();
        List<PlayerHUDJoinRuntime> playerJoinHuds =
            new List<PlayerHUDJoinRuntime>();

        partial void CustomInitialize()
        {
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



        public void CustomActivity(PositionedObjectList<Player> players, PlayerBase playerBase)
        {
            foreach(var player in players)
            {
                var index = player.CurrentColorCategoryState.ToInt();

                playerHuds[index].Visible = true;
                playerHuds[index].UpdateHealth(player);
            }

            for(int i = 0; i < 4; i++)
            {
                playerJoinHuds[i].Visible = !playerHuds[i].Visible;
            }

            BaseHUDInstance.UpdateHealth(playerBase);

        }

        public void ShowGameOver(Screens.GameScreen currentScreen)
        {
            this.GameOverInstance.Visible = true;
            GameOverInstance.FadeToBlackAnimation.Play();
            GameOverInstance.FadeToBlackAnimation.EndReached += () =>
            {
                currentScreen.RestartScreen(false);
            };
        }

        public void SetPauseScreenVisibility(bool isVisible)
        {
            this.PauseInstance.Visible = isVisible;
        }
    }
}
