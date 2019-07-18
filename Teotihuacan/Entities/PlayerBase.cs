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

using StateInterpolationPlugin;
using FlatRedBall.Glue.StateInterpolation;

namespace Teotihuacan.Entities
{
	public partial class PlayerBase
	{
        #region Fields/Properties

        public int CurrentHP { get; private set; }
        bool canTakeDamage => CurrentHP > 0;

        Tweener pedestalPulseTweener;

        #endregion

        #region Initialize

        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
        private void CustomInitialize()
		{
            CurrentHP = MaxHp;


            pedestalPulseTweener = this.PedestalPulsingFxSpriteInstance.Tween(
                "Alpha", 0.8f, 3.6f, 
                FlatRedBall.Glue.StateInterpolation.InterpolationType.Linear, 
                FlatRedBall.Glue.StateInterpolation.Easing.Out);

            pedestalPulseTweener.Ended += () => 
                {
                    pedestalPulseTweener.Reverse();
                    pedestalPulseTweener.Start();
                };
        }

        #endregion

        private void CustomActivity()
		{


		}

        public void Heal(int ammountToHeal)
        {
            CurrentHP = Math.Min(CurrentHP + ammountToHeal, MaxHp);
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
                    UpdateVisualsToCurrentHealth();
                }

                if (CurrentHP <= 0)
                {
                    PerformDeath();

                }
                else
                {
                    FlashWhite();
                }
            }

            return tookDamage;
        }

        private void PerformDeath()
        {
            int numberOfExplosions = 13;

            void PlayExplosions()
            {
                var maxExplosionDistance = 32;

                var sprite = SpriteManager.AddSprite(BigExplosionBase);
                sprite.TextureScale = 1;
                sprite.X = FlatRedBallServices.Random.Between(X - maxExplosionDistance, X + maxExplosionDistance); ;
                sprite.Y = FlatRedBallServices.Random.Between(Y - maxExplosionDistance, Y + maxExplosionDistance); ;
                sprite.Z = this.Z + 1f;

                this.Call(() => SpriteManager.RemoveSprite(sprite)).After(BigExplosionBase.TotalLength - TimeManager.SecondDifference);
            }

            for(int i = 0; i < numberOfExplosions; i++)
            {
                if(i == 0)
                {
                    PlayExplosions();
                }
                else
                {
                    this.Call(PlayExplosions).After(i * .13);
                }
            }
        }

        private void UpdateVisualsToCurrentHealth()
        {
            var ratio = (float)CurrentHP / MaxHp;

            if(ratio > .5)
            {
                StatueSpriteInstance.CurrentFrameIndex = 0;
            }
            else if(ratio > .25)
            {
                StatueSpriteInstance.CurrentFrameIndex = 1;
            }
            else // ratio <= .25
            {
                StatueSpriteInstance.CurrentFrameIndex = 2;
            }
        }

        private void FlashWhite()
        {
            this.StatueSpriteInstance.ColorOperation = FlatRedBall.Graphics.ColorOperation.ColorTextureAlpha;
            this.StatueSpriteInstance.Red = 1;
            this.StatueSpriteInstance.Green = 1;
            this.StatueSpriteInstance.Blue = 1;

            this.Call(ReturnFromFlash).After(FlashDuration);
        }

        private void ReturnFromFlash()
        {
            this.StatueSpriteInstance.ColorOperation = FlatRedBall.Graphics.ColorOperation.Texture;
        }

        private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
