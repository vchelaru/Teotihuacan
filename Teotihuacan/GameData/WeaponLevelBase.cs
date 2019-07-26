using FlatRedBall.Input;
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
        public int toNextLevel;
        public int previousLevel;
        public Weapon WeaponType { get; private set; }

        public void ChangeWeaponType(Weapon newType)
        {
            WeaponType = newType;
            CurrentWeaponLevel = 0;
            WeaponExperience = 0;
            toNextLevel = (int)(Math.Pow(CurrentWeaponLevel + 1, 1 / ExperienceCurveRoot));
            previousLevel = 0;
        }

        public void AddWeaponExperience( int experienceToAdd = 1)
        {
            WeaponExperience += experienceToAdd;
            TryToUpgradeLevel();
        }

        private void TryToUpgradeLevel()
        {
            if(WeaponExperience >= toNextLevel)
            {
                CurrentWeaponLevel++;
                toNextLevel = (int)(Math.Pow(CurrentWeaponLevel + 1, 1 / ExperienceCurveRoot));
                previousLevel = (int)(Math.Pow(CurrentWeaponLevel, 1 / ExperienceCurveRoot));
            }
        }

        public float ProgressToNextLevel()
        {
            float adjustedCurrentXP = WeaponExperience - previousLevel;
            float adjustedNextXP = toNextLevel - previousLevel;
            return adjustedCurrentXP / adjustedNextXP;
        }
    }
}
