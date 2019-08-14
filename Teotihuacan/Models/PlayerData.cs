using FlatRedBall.Input;
using Newtonsoft.Json;
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
            Reserved_Left = 1,
            Reserved_Disconnect = 2,

            Full_PlayerDead = 3,

            Full = 4,
        }



        public int SlotIndex;

        [JsonIgnore]
        public eSlotState SlotState; // = eSlotState.Free;

        [JsonIgnore]
        public InputControls InputControls;

        public List<WeaponLevelBase> WeaponLevels = new List<WeaponLevelBase>();

        public Weapon EquippedWeapon { get; set; } = Weapon.ShootingFire;




        public PlayerData() { }

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
