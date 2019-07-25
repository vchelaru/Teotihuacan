using System;
using System.Collections.Generic;
using System.Text;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;
using FlatRedBall.Math.Geometry;
using Teotihuacan.Animation;
using FlatRedBall.Screens;

namespace Teotihuacan.Entities
{
	public partial class WeaponDrop
	{

        public Weapon WeaponType { get; private set; }
        double spawnTime = 0;

        double currentBlinkFrequency => BlinkFrequency * (1 - ((currentScreen.PauseAdjustedSecondsSince(spawnTime) / LifeTime) - .5));
        bool hasBlinkBegan = false;

        private Screen currentScreen;
        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
		private void CustomInitialize()
		{
            switch(FlatRedBallServices.Random.Next(3))
            {
                case 0:
                    CurrentDataCategoryState = DataCategory.FireBallDrop;
                    WeaponType = Weapon.ShootingFire;
                    break;
                case 1:
                    CurrentDataCategoryState = DataCategory.LightningDrop;
                    WeaponType = Weapon.ShootingLightning;
                    break;
                case 2:
                    CurrentDataCategoryState = DataCategory.SkullDrop;
                    WeaponType = Weapon.ShootingSkulls;
                    break;
            }
            currentScreen = ScreenManager.CurrentScreen;
            spawnTime = currentScreen.PauseAdjustedCurrentTime;
            this.Call(Destroy).After(LifeTime);
            //Debug remove this
            CurrentDataCategoryState = DataCategory.FireBallDrop;
            WeaponType = Weapon.ShootingFire;

        }

		private void CustomActivity()
		{
            if(!hasBlinkBegan && currentScreen.PauseAdjustedSecondsSince(spawnTime) >= LifeTime *.5)
            {
                hasBlinkBegan = true;
                PerformBlink();
            }

		}

        void PerformBlink()
        {
            SpriteInstance.Visible = !SpriteInstance.Visible;
            this.Call(PerformBlink).After(currentBlinkFrequency);
        }

		private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
