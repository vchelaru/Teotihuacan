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
using Microsoft.Xna.Framework.Audio;

namespace Teotihuacan.Screens
{
	public partial class Level10
	{
        

		void CustomInitialize()
		{
            LoopedBackgroundMusic = Level10LoopedMusic.CreateInstance();
            LoopedBackgroundMusic.Volume = 0.3f;
            LoopedBackgroundMusic.IsLooped = true;
            LoopedBackgroundMusic.Play();

            this.Spawns = new Level5Spawns();
            CurrentMultipliers.LevelHealthMultiplier = 2.7f;
            CurrentMultipliers.LevelDamageMultiplier = 1.15f;
            CurrentMultipliers.LevelRangeMultiplier = 1.35f;

            this.NextScreen = nameof(WinScreen);
        }

        protected override void InitializeMusic()
        {
            LoopedBackgroundMusic = Level10LoopedMusic.CreateInstance();
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
