using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Localization;
using Teotihuacan.GumRuntimes;

namespace Teotihuacan.Screens
{
	public partial class UiTestScreen
	{

		void CustomInitialize()
        {
            //this.GameOverInstance.Visible = false;

            //this.GameOverInstance.PopupAppearAnimation.EndReached += () =>
            //{
            //    GameOverInstance.FadeToBlackAnimation.PlayAfter(3);
            //};

		}

		void CustomActivity(bool firstTimeCalled)
		{
            if(InputManager.Keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                this.GameOverInstance.Visible = true;

                this.GameOverInstance.CurrentVisualsAlphaState = GameOverRuntime.VisualsAlpha.Transparent;
                this.GameOverInstance.PopupAppearAnimation.Play();
            }
		}

		void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
