using System;
using System.Collections.Generic;
using System.Linq;
using Teotihuacan.Entities;

namespace Teotihuacan.GumRuntimes
{
    public partial class BaseHUDRuntime
    {
        float healthGageMax;
        partial void CustomInitialize()
        {
            healthGageMax = HealthGauge.Height;

        }

        public void UpdateHealth(PlayerBase playerBase)
        {
            HealthGauge.Height = healthGageMax * 
                ((float)(playerBase.CurrentHP) / (float)(playerBase.MaxHp));

        }
    }
}
