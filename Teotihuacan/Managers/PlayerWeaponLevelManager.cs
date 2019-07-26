using FlatRedBall.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teotihuacan.GameData;

namespace Teotihuacan.Managers
{
    public static class PlayerWeaponLevelManager
    {
        public static Dictionary<IInputDevice, WeaponLevelBase> PlayerWeaponLevels = new Dictionary<IInputDevice, WeaponLevelBase>();

        public static void CreateNewWeaponLevelFromInputDevice(IInputDevice inputDevice)
        {
            var weaponLevel = new WeaponLevelBase();
            weaponLevel.ChangeWeaponType(Animation.Weapon.ShootingFire);

            PlayerWeaponLevels.Add(inputDevice, weaponLevel);
        }
    }
}
