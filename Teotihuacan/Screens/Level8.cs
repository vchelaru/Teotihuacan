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
using Teotihuacan.GameData;

namespace Teotihuacan.Screens
{
	public partial class Level8
	{

		void CustomInitialize()
		{
            this.Spawns = new Level3Spawns();
            CurrentMultipliers.LevelHealthMultiplier = 2.5f;
            CurrentMultipliers.LevelDamageMultiplier = 1.15f;
            CurrentMultipliers.LevelRangeMultiplier = 1.35f;

            this.NextScreen = nameof(Level9);

        }

		void CustomActivity(bool firstTimeCalled)
		{


		}

		void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}