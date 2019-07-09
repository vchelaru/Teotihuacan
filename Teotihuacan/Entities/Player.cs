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
using Microsoft.Xna.Framework;
using FlatRedBall.Gui;
using Teotihuacan.Animation;

namespace Teotihuacan.Entities
{
	public partial class Player
	{
        #region Fields/Properties

        I2DInput rightStick;
        Vector3 aimingVector;
        AnimationController spriteAnimationController;
        bool isPrimaryInputDown
        {
            get
            {
                if(InputDevice == InputManager.Keyboard)
                {
                    return InputDevice.DefaultPrimaryActionInput.IsDown || 
                        InputManager.Mouse.ButtonDown(Mouse.MouseButtons.LeftButton);

                }
                else if(InputDevice is Xbox360GamePad gamePad)
                {
                    return gamePad.RightTrigger.IsDown;
                }
                else
                {
                    return false;
                }
            }

        }

        PrimaryActions currentPrimaryAction = PrimaryActions.idle;
        SecondaryActions currentSecondaryAction = SecondaryActions.None;
        AnimationLayer shootingLayer;
        double lastFireShotTime;

        public int CurrentHP { get; private set; }

        bool canTakeDamage => CurrentHP > 0;

        public Action UpdateHud;

        public bool PauseInputPressed => InputDevice.DefaultPauseInput.WasJustPressed;
        #endregion

        #region Initialize

        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
        private void CustomInitialize()
		{
            this.PossibleDirections = PossibleDirections.EightWay;
            CurrentHP = MaxHP;

            InitializeAnimationLayers();
		}

        private void InitializeAnimationLayers()
        {
            spriteAnimationController = new AnimationController(SpriteInstance);
            AnimationLayer walkAnimationLayer = new AnimationLayer();
            walkAnimationLayer.EveryFrameAction  = () =>
            {
                return GetChainName(currentPrimaryAction);
            };
            spriteAnimationController.Layers.Add(walkAnimationLayer);

            shootingLayer = new AnimationLayer();
            spriteAnimationController.Layers.Add(shootingLayer);
        }

        public void SetTwinStickInput(Xbox360GamePad gamePad)
        {
            InitializeTopDownInput(gamePad);
            rightStick = gamePad.RightStick;
        }

        #endregion

        #region Activity

        private void CustomActivity()
		{
            DoPrimaryActionActivity();
            DoAimingActivity();
            DoShootingActivity();
            DoMovementValueUpdate();
            spriteAnimationController.Activity();
		}

        private void DoPrimaryActionActivity()
        {
            const float movementThreashHold = 0.01f;
            currentPrimaryAction = MovementInput.Magnitude > movementThreashHold ? PrimaryActions.walk : PrimaryActions.idle;
        }

        private void DoAimingActivity()
        {
            Vector3 newAimingVector = aimingVector;
            if (rightStick != null)
            {
                if (rightStick.Magnitude > 0)
                {
                    newAimingVector = new Vector3(rightStick.X, rightStick.Y, 0);
                }
            }
            else
            {

                Vector3 cursorPosition = new Vector3(GuiManager.Cursor.WorldXAt(Z), GuiManager.Cursor.WorldYAt(Z), 0);

                newAimingVector = cursorPosition - new Vector3(X, Y, 0);
            }

            // in case the stick happens to report 0:
            if(newAimingVector.X == 0 && newAimingVector.Y == 0)
            {
                newAimingVector.X = 1;
            }

            if(newAimingVector.X != 0 || newAimingVector.Y != 0)
            {
                //Normalize at the end in case the right stick input is not at max magnitude
                newAimingVector.Normalize();
                aimingVector = newAimingVector;
            }
        }

        private void DoShootingActivity()
        {
            Bullet.DataCategory bulletData = null;
            if (isPrimaryInputDown)
            {
                // todo - support different weapons:
                currentSecondaryAction = SecondaryActions.ShootingFire;
                bulletData = Bullet.DataCategory.PlayerFire;
            }
            else
            {
                currentSecondaryAction = SecondaryActions.None;
            }

            if(currentSecondaryAction == SecondaryActions.ShootingFire &&
                FlatRedBall.Screens.ScreenManager.CurrentScreen.PauseAdjustedSecondsSince(lastFireShotTime) > 
                1/FireShotsPerSecond
                )
            {
                var direction = aimingVector;

                var bullet = Factories.BulletFactory.CreateNew(this.X, this.Y);
                bullet.Owner = this;
                bullet.CurrentDataCategoryState = bulletData;
                bullet.Velocity = bullet.BulletSpeed * direction;
                bullet.SetAnimationChainFromVelocity(TopDownDirectionExtensions.FromDirection(aimingVector, PossibleDirections));

                shootingLayer.PlayOnce(GetChainName(currentPrimaryAction, SecondaryActions.ShootingFire));

                lastFireShotTime = FlatRedBall.Screens.ScreenManager.CurrentScreen.PauseAdjustedCurrentTime;
            }
        }

        private void DoMovementValueUpdate()
        {
            switch(currentSecondaryAction)
            {
                case SecondaryActions.None:
                    mCurrentMovement = TopDownValues[DataTypes.TopDownValues.DefaultValues];
                    break;
                case SecondaryActions.ShootingFire:
                case SecondaryActions.ShootingLightning:
                    mCurrentMovement = TopDownValues[DataTypes.TopDownValues.WhileShooting];
                    break;
            }
        }

        private string GetChainName(PrimaryActions primaryAction, SecondaryActions secondaryAction = SecondaryActions.None)
        {
            if(aimingVector.X != 0 || aimingVector.Y != 0)
            {
                var direction = TopDownDirectionExtensions.FromDirection(new Vector2(aimingVector.X, aimingVector.Y), PossibleDirections.EightWay);

                return ChainNameHelperMethods.GenerateChainName(primaryAction, secondaryAction, direction);
            }
            else
            {
                return ChainNameHelperMethods.GenerateChainName(primaryAction, secondaryAction, TopDownDirection.Right);
            }
        }

        public bool TakeDamage(int damageToTake)
        {
            bool didTakeDamage = false;

            if(canTakeDamage)
            {
                didTakeDamage = true;

                if(CurrentHP > 0)
                {
                    CurrentHP -= damageToTake;

                    CurrentHP = Math.Max(CurrentHP, 0);
                    UpdateHud?.Invoke();

                    if (CurrentHP > 0)
                    {
                        FlashWhite();
                    }
                    else
                    {
                        PerformDeath();
                    }

                }
            }

            return didTakeDamage;
        }

        private void PerformDeath()
        {
            var death = SpriteManager.AddParticleSprite(Death_1_SpriteSheet);
            death.AnimationChains = Death_1;
            death.CurrentChainName = nameof(PrimaryActions.Death);
            death.Position = SpriteInstance.Position;
            death.TextureScale = 1;
            death.Animate = true;
            SpriteManager.RemoveSpriteAtTime(death, death.CurrentChain.TotalLength);
            Destroy();
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

        #endregion


        private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}

    public static class ColorStateExtensions
    {
        public static int ToInt(this Player.ColorCategory category)
        {
            if (category == Player.ColorCategory.Blue)
                return 0;
            if (category == Player.ColorCategory.Red)
                return 1;
            if (category == Player.ColorCategory.Yellow)
                return 2;
            if (category == Player.ColorCategory.Green)
                return 3;

            return -1;
        }

        public static Player.ColorCategory ToPlayerColorCategory(this int value)
        {
            switch(value)
            {
                case 0: return Player.ColorCategory.Blue;
                case 1: return Player.ColorCategory.Red;
                case 2: return Player.ColorCategory.Yellow;
                case 3: return Player.ColorCategory.Green;
            }

            return Player.ColorCategory.Blue;
        }
    }
}
