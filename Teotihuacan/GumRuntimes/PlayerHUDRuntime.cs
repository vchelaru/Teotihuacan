    using System;
    using System.Collections.Generic;
    using System.Linq;
using Teotihuacan.Entities;

namespace Teotihuacan.GumRuntimes
{
    public partial class PlayerHUDRuntime
    {
        float healthGageMax;
        partial void CustomInitialize () 
        {
            healthGageMax = HealthGauge.Height;
        }


        public void UpdateHealth(Player owningPlayer)
        {
            HealthGauge.Height = healthGageMax * ((float)(owningPlayer.CurrentHP) / (float)(owningPlayer.MaxHP));
            
        }
    }
}
