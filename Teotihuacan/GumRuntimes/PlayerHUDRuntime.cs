    using System;
    using System.Collections.Generic;
    using System.Linq;
using Teotihuacan.Entities;
using Teotihuacan.UiControllers;

namespace Teotihuacan.GumRuntimes
{
    public partial class PlayerHUDRuntime
    {
        float healthGageMax;
        float energyGaugeMax;
        float xpGaugeMax;

        BarController BarController;

        partial void CustomInitialize () 
        {
            BarController = new BarController();
            BarController.MaxHeight = XPGauge.Height;
            BarController.Bar = XPGauge;

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


            //XPGauge.Height = xpGaugeMax * owningPlayer.ProgressToNextLevel;
            XPGauge.Visible = XPGauge.Height > 1;
            WeaponLevelText = $"{owningPlayer.CurrentWeaponLevel}";
        }

        public void RefreshExperienceBar(Player owningPlayer, UpdateType updateType)
        {
            if(updateType == UpdateType.Instant)
            {
                XPGauge.Height = xpGaugeMax * owningPlayer.ProgressToNextLevel;
            }
            else
            {
                BarController.InterpolateToRatio(owningPlayer.ProgressToNextLevel);
            }
        }
    }
}
