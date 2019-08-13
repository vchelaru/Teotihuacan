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
        public enum eSlotState
        {
            Free = 0,
            ReservedLeft = 1,
            ReservedDisconnect = 2,
            FullPlayerDead = 3,
            Full = 4,
        }



        public int SlotIndex;

        public eSlotState SlotState; // = eSlotState.Free;

        public InputControls InputControls;

        public List<WeaponLevelBase> WeaponLevels = new List<WeaponLevelBase>();

        public Weapon EquippedWeapon { get; set; } = Weapon.ShootingFire;




        public PlayerData() { }

        /*public PlayerData(Player playerCharEntity)
        {
            SlotIndex = playerCharEntity.Index; // DO NOT USE Player.Index !! It's for Pleyr Entity pooling only !
            InputControls = playerCharEntity.InputControls;
        }*/

        public PlayerData(int slotIndex, InputControls inputControls)
        {
            SlotIndex = slotIndex;
            InputControls = inputControls;

            InitializeAllWeapons();
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

        /*public void SetStateFree()
        {
            SlotState = eSlotState.Free;
            InputControls = null;
        }*/

    }
}
