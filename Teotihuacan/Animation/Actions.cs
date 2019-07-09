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
        Fireball = 4,
        Skull = 5,
        Death = 6,
        ProjectileExplosion = 7
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
                case PrimaryActions.Fireball:
                    return $"{nameof(PrimaryActions.Fireball)}_";
                case PrimaryActions.Skull:
                    return $"{nameof(PrimaryActions.Skull)}_";
                case PrimaryActions.Death:
                    return $"{nameof(PrimaryActions.Death)}_";
                case PrimaryActions.ProjectileExplosion:
                    return $"{nameof(PrimaryActions.ProjectileExplosion)}_";
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
