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
        float xpGaugeMax;

        partial void CustomInitialize () 
        {
            healthGageMax = HealthGauge.Height;
            energyGaugeMax = EnergyGauge.Height;
            xpGaugeMax = XPGauge.Height;
        }


        public void UpdateStatusBars(Player owningPlayer)
        {
            HealthGauge.Height = healthGageMax * ((float)(owningPlayer.CurrentHP) / (float)(owningPlayer.MaxHP));
            // below 1 and things overlap. We could use clipping but...this works
            HealthGauge.Visible = HealthGauge.Height > 1;
            EnergyGauge.Height = energyGaugeMax * owningPlayer.CurrentEnergy / owningPlayer.MaxEnergy;
            // below 1 and things overlap. We could use clipping but...this works
            EnergyGauge.Visible = EnergyGauge.Height > 1;

            XPGauge.Height = xpGaugeMax * owningPlayer.ProgressToNextLevel;
            XPGauge.Visible = XPGauge.Height > 1;
            WeaponLevelText = $"{owningPlayer.CurrentWeaponLevel}";
        }
    }
}
