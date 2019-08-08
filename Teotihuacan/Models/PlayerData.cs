using FlatRedBall.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teotihuacan.Animation;
using Teotihuacan.Entities;
using Teotihuacan.GameData;

namespace Teotihuacan.Models
{

    public class PlayerData
    {
        public readonly int PlayerIndex;

        public IInputControls InputControls;

        public bool IsPlayerDead = false;

        public List<WeaponLevelBase> WeaponLevels = new List<WeaponLevelBase>();

        public Weapon EquippedWeapon { get; set; } = Weapon.ShootingFire;



        public PlayerData(Player playerCharEntity)
        {
            PlayerIndex = playerCharEntity.Index;
            InputControls = playerCharEntity.InputControls;
        }
        public PlayerData(int playerIndex, IInputControls inputControls)
        {
            PlayerIndex = playerIndex;
            InputControls = inputControls;
        }



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
            var weaponLevel = WeaponLevels.First(item => item.WeaponType == weapon);

            weaponLevel.AddWeaponExperience();
        }

    }
}
