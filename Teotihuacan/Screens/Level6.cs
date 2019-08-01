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
	public partial class Level6
	{

		void CustomInitialize()
		{

            this.Spawns = new Level1Spawns();
            CurrentMultipliers.LevelHealthMultiplier = 2.2f;
            CurrentMultipliers.LevelDamageMultiplier = 1.15f;
            CurrentMultipliers.LevelRangeMultiplier = 1.35f;

            this.NextScreen = nameof(Level7);

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
