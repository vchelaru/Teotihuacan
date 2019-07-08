using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teotihuacan.Animation
{
    public enum PrimaryActions
    {
        idle = 0,
        walk = 1,
        shoot = 2,
        Death = 3
    }

    public enum SecondaryActions
    {
        None = 0,
        ShootingFire = 1,
        ShootingLightning = 2
    }

    public static class ActionsExtentions
    {
        public static string ToFriendlyString(this PrimaryActions action)
        {
            switch (action)
            {
                case PrimaryActions.shoot:
                    return $"{nameof(PrimaryActions.shoot)}_";
                case PrimaryActions.walk:
                    return $"{nameof(PrimaryActions.walk)}_";
            }

            return $"{nameof(PrimaryActions.idle)}_";
        }

        public static string ToFriendlyString(this SecondaryActions action)
        {
            switch(action)
            {
                case SecondaryActions.ShootingFire:
                    return $"{nameof(SecondaryActions.ShootingFire)}_";
                case SecondaryActions.ShootingLightning:
                    return $"{nameof(SecondaryActions.ShootingLightning)}_";
            }

            return string.Empty;
        }
    }
}
