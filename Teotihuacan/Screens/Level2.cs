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
	public partial class Level2
	{

		void CustomInitialize()
		{
            Spawns = new Level2Spawns();
            CurrentMultipliers.LevelHealthMultiplier = 1.25f;

            NextScreen = nameof(Level3);
        }

        protected override void InitializeMusic()
        {
            LoopedBackgroundMusic = Level2LoopedMusic.CreateInstance();
            LoopedBackgroundMusic.Volume = 0.3f;
            LoopedBackgroundMusic.IsLooped = true;
            LoopedBackgroundMusic.Play();
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
