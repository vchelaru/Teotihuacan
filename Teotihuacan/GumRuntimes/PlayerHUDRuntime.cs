    using System;
    using System.Collections.Generic;
    using System.Linq;
using Teotihuacan.Entities;

namespace Teotihuacan.GumRuntimes
{
    public partial class PlayerHUDRuntime
    {
        Player owningPlayer;
        float healthGageMax;
        partial void CustomInitialize () 
        {
            healthGageMax = HealthGauge.Height;
        }

        public void SetOwningPlayer(Player owner)
        {
            owningPlayer = owner;
            if(owningPlayer != null)
            {
                owningPlayer.UpdateHud = OnUpdateHud;
            }
        }

        public void OnUpdateHud()
        {
            HealthGauge.Height = healthGageMax * ((float)(owningPlayer.CurrentHP) / (float)(owningPlayer.MaxHP));
        }
    }
}
