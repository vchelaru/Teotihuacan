using System;
using System.Collections.Generic;
using System.Linq;

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
    }
}
