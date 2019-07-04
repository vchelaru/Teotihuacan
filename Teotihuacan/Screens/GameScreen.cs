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
using Teotihuacan.Managers;
using FlatRedBall.TileEntities;

namespace Teotihuacan.Screens
{
	public partial class GameScreen
	{
        protected LevelSpawnsBase Spawns;

        SpawnManager spawnManager;


        void CustomInitialize()
		{

            TileEntityInstantiator.CreateEntitiesFrom(Map);

            spawnManager = new SpawnManager();

        }

		void CustomActivity(bool firstTimeCalled)
		{
            spawnManager.DoActivity(EnemyList, SpawnPointList, Spawns);


        }

		void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
