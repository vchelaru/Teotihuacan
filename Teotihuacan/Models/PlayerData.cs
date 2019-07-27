using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teotihuacan.Animation;
using Teotihuacan.GameData;

namespace Teotihuacan.Models
{

    public class PlayerData
    {
        public List<WeaponLevelBase> WeaponLevels = new List<WeaponLevelBase>();

        public Weapon EquippedWeapon { get; set; } = Weapon.ShootingFire;

        public void InitializeAllWeapons()
        {
            AddWeapon(Weapon.ShootingFire);
            AddWeapon(Weapon.ShootingLightning);
            AddWeapon(Weapon.ShootingSkulls);
        }

        private void AddWeapon(Weapon weaponType)
        {
            var levelBase = new WeaponLevelBase();
            levelBase.ChangeWeaponType(weaponType);
            WeaponLevels.Add(levelBase);
        }

        public void AddWeaponExperience(Weapon weapon)
        {
            var weaponLevel = WeaponLevels.Single(item => item.WeaponType == weapon);

            weaponLevel.AddWeaponExperience();
        }

    }
}
