using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teotihuacan.Animation;

namespace Teotihuacan.GameData
{
    public class WeaponLevelBase
    {
        public static readonly int LevelCap = 5;
        public static readonly float ExperienceCurveRoot = .6f;

        public int CurrentWeaponLevel { get; private set; }
        public int WeaponExperience { get; private set; }
        public Weapon WeaponType { get; private set; }

        public void ChangeWeaponType(Weapon newType)
        {
            WeaponType = newType;
            CurrentWeaponLevel = 0;
            WeaponExperience = 0;
        }

        public void AddWeaponExperience( int experienceToAdd = 1)
        {
            WeaponExperience += experienceToAdd;
            TryToUpgradeLevel();
        }

        private void TryToUpgradeLevel()
        {
            CurrentWeaponLevel = (int)(Math.Pow(WeaponExperience, ExperienceCurveRoot));

            //For now we will not have a level cap.
            //I think the current level function will keep everything pretty balanced.
            //Math.Max(CurrentWeaponLevel, LevelCap);
        }
    }
}
