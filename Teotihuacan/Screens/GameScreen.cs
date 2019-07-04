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
using Teotihuacan.Entities;

namespace Teotihuacan.Screens
{
	public partial class GameScreen
	{
        #region Fields/Properties

        protected LevelSpawnsBase Spawns;

        SpawnManager spawnManager;

        #endregion

        #region Initialize

        void CustomInitialize()
		{

            TileEntityInstantiator.CreateEntitiesFrom(Map);

            spawnManager = new SpawnManager();

            InitializeCollisions();

            CameraControllerInstance.Targets.AddRange(PlayerList);

            Factories.EnemyFactory.EntitySpawned = HandleEnemySpawn;

        }

        private void InitializeCollisions()
        {
            // add border around the tile map
            int borderSizeWide = Map.NumberTilesWide.Value + 2;

            var leftX = Map.X - Map.WidthPerTile.Value / 2.0f;
            var rightX = Map.X + Map.Width + Map.WidthPerTile.Value / 2.0f;
            
            float worldX = leftX;
            float topY = Map.Y + Map.HeightPerTile.Value / 2.0f;

            float bottomY = Map.Y - Map.Height- Map.HeightPerTile.Value / 2.0f;

            for(int x = 0; x < borderSizeWide; x++)
            {
                SolidCollisions.AddCollisionAtWorld(worldX, topY);
                SolidCollisions.AddCollisionAtWorld(worldX, bottomY);
                worldX += Map.WidthPerTile.Value;

            }

            float worldY = 0;
            for (int y = 0; y < Map.NumberTilesTall.Value; y++)
            {
                SolidCollisions.AddCollisionAtWorld(leftX, worldY);
                SolidCollisions.AddCollisionAtWorld(rightX, worldY);

                worldY -= Map.HeightPerTile.Value;
            }
        }

        #endregion

        #region Activity

        void CustomActivity(bool firstTimeCalled)
		{
            spawnManager.DoActivity(EnemyList, SpawnPointList, Spawns);

            DoAi();
        }

        private void DoAi()
        {
            foreach(var enemy in EnemyList)
            {
                var ai = enemy.InputDevice as TopDown.TopDownAiInput<Enemy>;
                ai.Activity();
                ai.Target = PlayerList[0].Position;
            }
        }

        private void HandleEnemySpawn(Enemy enemy)
        {
            var input = new TopDown.TopDownAiInput<Enemy>(enemy);
            enemy.InitializeTopDownInput(input);
        }

        #endregion

        void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
