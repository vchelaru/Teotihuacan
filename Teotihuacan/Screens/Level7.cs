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
	public partial class Level7
	{

		void CustomInitialize()
		{
            this.Spawns = new Level2Spawns();
            CurrentMultipliers.LevelHealthMultiplier = 2.4f;
            CurrentMultipliers.LevelDamageMultiplier = 1.15f;
            CurrentMultipliers.LevelRangeMultiplier = 1.35f;

            this.NextScreen = nameof(Level8);

        }

        protected override void InitializeMusic()
        {
            if (FlatRedBall.Audio.AudioManager.CurrentSong != null)
            {
                FlatRedBall.Audio.AudioManager.StopSong();
            }
            FlatRedBall.Audio.AudioManager.PlaySong(Level7Music_anglozulu_by_kevin_macleod, true, false);
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
