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
	public partial class Level4
	{

		void CustomInitialize()
		{
            this.Spawns = new Level4Spawns();
            CurrentMultipliers.LevelHealthMultiplier = 1.65f;
            CurrentMultipliers.LevelDamageMultiplier = 1.11f;
            CurrentMultipliers.LevelRangeMultiplier = 1.3f;

            this.NextScreen = nameof(Level5);

        }

        protected override void InitializeMusic()
        {
            if (FlatRedBall.Audio.AudioManager.CurrentSong != null)
            {
                FlatRedBall.Audio.AudioManager.StopSong();
            }
            FlatRedBall.Audio.AudioManager.PlaySong(Level4Music_tafi_maradi_no_voice_by_kevin_macleod, true, false);
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
