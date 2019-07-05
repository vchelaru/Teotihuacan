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
        I2DInput rightStick;
        Vector3 aimingVector;
        AnimationController spriteAnimationController;
        bool wasPrimaryInputPressed => InputDevice.DefaultPrimaryActionInput.WasJustPressed || InputManager.Mouse.ButtonPushed(Mouse.MouseButtons.LeftButton);
        PrimaryActions currentPrimaryAction = PrimaryActions.idle;
        AnimationLayer shootingLayer;


        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
		private void CustomInitialize()
		{
            this.PossibleDirections = PossibleDirections.EightWay;

            InitializeTwinStickInput();
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

        private void InitializeTwinStickInput()
        {
            var inputAsGamepad = InputDevice as Xbox360GamePad;
            if (inputAsGamepad != null)
            {
                rightStick = inputAsGamepad.RightStick;
            }
        }

        private void CustomActivity()
		{
            DoPrimaryActionActivity();
            DoAimingActivity();
            DoShootingActivity();
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

            //Normalize at the end in case the right stick input is not at max magnitude
            newAimingVector.Normalize();
            aimingVector = newAimingVector;
        }

        private void DoShootingActivity()
        {
            var didShoot = wasPrimaryInputPressed;

            if(didShoot)
            {
                var direction = aimingVector;

                var bullet = Factories.BulletFactory.CreateNew(this.X, this.Y);
                bullet.Velocity = bullet.BulletSpeed * direction;

                shootingLayer.PlayOnce(GetChainName(currentPrimaryAction, SecondaryActions.ShootingFire));
            }
        }

        private string GetChainName(PrimaryActions primaryAction, SecondaryActions secondaryAction = SecondaryActions.None)
        {
            var direction = TopDownDirectionExtensions.FromDirection(new Vector2(aimingVector.X, aimingVector.Y), PossibleDirections.EightWay);

            return ChainNameHelperMethods.GenerateChainName(primaryAction, secondaryAction, direction);
        }

        private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
