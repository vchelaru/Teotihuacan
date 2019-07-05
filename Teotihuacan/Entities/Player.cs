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

namespace Teotihuacan.Entities
{
	public partial class Player
	{
        TopDownDirection directionAiming;
        I2DInput rightStick;
        Vector3 aimingVector;
        AnimationController spriteAnimationController;
        bool wasPrimaryInputPressed => InputDevice.DefaultPrimaryActionInput.WasJustPressed || InputManager.Mouse.ButtonPushed(Mouse.MouseButtons.LeftButton);

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
                if(MovementInput.Magnitude > .01)
                {
                    return MakeAnimationChainName("walk");
                }
                return MakeAnimationChainName("idle");
            };

            spriteAnimationController.Layers.Add(walkAnimationLayer);
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
            DoAimingActivity();
            DoShootingActivity();
            spriteAnimationController.Activity();
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
            }
        }

        private string MakeAnimationChainName(string action)
        {
            switch(TopDownDirectionExtensions.FromDirection(new Vector2(aimingVector.X, aimingVector.Y), PossibleDirections.EightWay))
            {
                case TopDownDirection.Down:
                    return $"{action}_{nameof(TopDownDirection.Down)}";
                case TopDownDirection.DownLeft:
                    return $"{action}_{nameof(TopDownDirection.DownLeft)}";
                case TopDownDirection.DownRight:
                    return $"{action}_{nameof(TopDownDirection.DownRight)}";
                case TopDownDirection.Left:
                    return $"{action}_{nameof(TopDownDirection.Left)}";
                case TopDownDirection.Right:
                    return $"{action}_{nameof(TopDownDirection.Right)}";
                case TopDownDirection.Up:
                    return $"{action}_{nameof(TopDownDirection.Up)}";
                case TopDownDirection.UpLeft:
                    return $"{action}_{nameof(TopDownDirection.UpLeft)}";
                case TopDownDirection.UpRight:
                    return $"{action}_{nameof(TopDownDirection.UpRight)}";
            }

            return $"{action}_{nameof(TopDownDirection.Down)}";
        }

        private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
