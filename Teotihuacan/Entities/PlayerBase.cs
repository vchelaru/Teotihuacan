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

namespace Teotihuacan.Entities
{
	public partial class PlayerBase
	{

        public int CurrentHP { get; private set; }
        bool canTakeDamage => CurrentHP > 0;


        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
		private void CustomInitialize()
		{
            CurrentHP = MaxHp;

		}

		private void CustomActivity()
		{


		}

        public bool TakeDamage(int damageToTake)
        {
            bool tookDamage = false;
            if (canTakeDamage)
            {
                tookDamage = true;

                if(!DebuggingVariables.IsBaseInvincible)
                {
                    CurrentHP -= damageToTake;
                }

                if (CurrentHP <= 0)
                {
                    //PerformDeath();
                }
                else
                {
                    FlashWhite();
                }
            }

            return tookDamage;
        }

        private void FlashWhite()
        {
            this.SpriteInstance.ColorOperation = FlatRedBall.Graphics.ColorOperation.ColorTextureAlpha;
            this.SpriteInstance.Red = 1;
            this.SpriteInstance.Green = 1;
            this.SpriteInstance.Blue = 1;

            this.Call(ReturnFromFlash).After(FlashDuration);
        }

        private void ReturnFromFlash()
        {
            this.SpriteInstance.ColorOperation = FlatRedBall.Graphics.ColorOperation.Texture;
        }

        private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
