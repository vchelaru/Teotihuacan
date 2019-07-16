    using System;
    using System.Collections.Generic;
    using System.Linq;
using Teotihuacan.Entities;

namespace Teotihuacan.GumRuntimes
{
    public partial class PlayerHUDRuntime
    {
        float healthGageMax;
        float energyGaugeMax;

        partial void CustomInitialize () 
        {
            healthGageMax = HealthGauge.Height;
            energyGaugeMax = EnergyGauge.Height;
        }


        public void UpdateHealth(Player owningPlayer)
        {
            HealthGauge.Height = healthGageMax * ((float)(owningPlayer.CurrentHP) / (float)(owningPlayer.MaxHP));
            // below 1 and things overlap. We could use clipping but...this works
            HealthGauge.Visible = HealthGauge.Height > 1;
            EnergyGauge.Height = energyGaugeMax * owningPlayer.CurrentEnergy / owningPlayer.MaxEnergy;
            // below 1 and things overlap. We could use clipping but...this works
            EnergyGauge.Visible = EnergyGauge.Height > 1;

        }
    }
}
