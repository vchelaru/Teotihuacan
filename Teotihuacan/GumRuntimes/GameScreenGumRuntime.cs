using FlatRedBall.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using Teotihuacan.Entities;

namespace Teotihuacan.GumRuntimes
{
    public partial class GameScreenGumRuntime
    {
        partial void CustomInitialize()
        {
        }

        public void SetNumberOfPlayers(int numberOfPlayers)
        {
            this.PlayerHUDInstance.Visible = numberOfPlayers > 0;
            this.PlayerHUDInstance1.Visible = numberOfPlayers > 1;
            this.PlayerHUDInstance2.Visible = numberOfPlayers > 2;
            this.PlayerHUDInstance3.Visible = numberOfPlayers > 3;
        }

        public void SetHUDOwners(PositionedObjectList<Player> players)
        {
            this.PlayerHUDInstance.SetOwningPlayer(players.Count > 0 ? players[0] : null);
            this.PlayerHUDInstance1.SetOwningPlayer(players.Count > 1 ? players[1] : null);
            this.PlayerHUDInstance2.SetOwningPlayer(players.Count > 2 ? players[2] : null);
            this.PlayerHUDInstance3.SetOwningPlayer(players.Count > 3 ? players[3] : null);
        }
    }
}
