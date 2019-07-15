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



namespace Teotihuacan.Screens
{
	public partial class RivaTestScreen
	{
        const float _Slow = 0.5f;
        const float _Fast2x = 2f;
        const float _Fast4x = 4f;

        const float _CriticalHP_PositiveAlphaRate = 3f;
        const float _CriticalMP_PositiveAlphaRate = 3f;
        const float _Healed_PositiveAlphaRate = 3f;

        void CustomInitialize()
		{
            Steppe_test_02.X = -266f; // 533f;
            Steppe_test_02.Y = 150f; // 300f;

            CriticalHP_ColorSpriteInstance.Alpha = 0f;
            CriticalMP_ColorSpriteInstance.Alpha = 0f;
            Healed_ColorSpriteInstance.Alpha = 0f;

            CriticalHP_ColorSpriteInstance.AlphaRate = _CriticalHP_PositiveAlphaRate;
            CriticalMP_ColorSpriteInstance.AlphaRate = _CriticalMP_PositiveAlphaRate;
            Healed_ColorSpriteInstance.AlphaRate = _Healed_PositiveAlphaRate;
        }

		void CustomActivity(bool firstTimeCalled)
		{
            CheckFlipAlphaRate(CriticalHP_ColorSpriteInstance, _CriticalHP_PositiveAlphaRate, 0.75f);
            CheckFlipAlphaRate(CriticalMP_ColorSpriteInstance, _CriticalMP_PositiveAlphaRate, 0.75f);
            CheckFlipAlphaRate(Healed_ColorSpriteInstance, _Healed_PositiveAlphaRate, 0.75f);
		}

		void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }


        void CheckFlipAlphaRate(Sprite sprite, float alphaRate, float maxAlpha)
        {
            if (sprite.Alpha >= maxAlpha)
            {
                sprite.AlphaRate = -alphaRate;
            }
            else if(sprite.Alpha <= 0f)
            {
                sprite.AlphaRate = alphaRate;
            }
        }
	}
}
