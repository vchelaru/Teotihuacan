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
using FlatRedBall.TileCollisions;
using Teotihuacan.GumRuntimes;

namespace Teotihuacan.Screens
{
	public partial class GameScreen
	{
        #region Fields/Properties

        protected LevelSpawnsBase Spawns;

        SpawnManager spawnManager;

        TileNodeNetwork nodeNetwork;

        const int AiFrameSkip = 10;

        int currentFrameSkipIndex;

        bool hasGameOverBeenTriggered = false;

        #endregion

        #region Initialize

        void CustomInitialize()
		{
            CreatePlayers();

            TileEntityInstantiator.CreateEntitiesFrom(Map);

            spawnManager = new SpawnManager();

            InitializeCollisions();

            CameraControllerInstance.Targets.AddRange(PlayerList);

            Factories.EnemyFactory.EntitySpawned = HandleEnemySpawn;

            InitializeNodeNetworks();

            InitializeUi();
        }

        private void CreatePlayers()
        {
            var numberOfControllers = InputManager.NumberOfConnectedGamePads;

            if(numberOfControllers == 0)
            {
                var player = new Player();
                player.CurrentColorCategoryState =
                    Player.ColorCategory.Blue;
                PlayerList.Add(player);
                player.InitializeTopDownInput(InputManager.Keyboard);
            }
            else
            {
                foreach(var controller in InputManager.Xbox360GamePads)
                {
                    if(controller.IsConnected)
                    {
                        var player = new Player();
                        player.CurrentColorCategoryState =
                            PlayerList.Count.ToPlayerColorCategory();

                        PlayerList.Add(player);
                        player.InitializeTopDownInput(controller);
                        player.SetTwinStickInput(controller);
                    }
                }
            }

            for(int i = 0; i < PlayerList.Count; i++)
            {
                var player = PlayerList[i];
                player.Y = -40;
                player.X = 40 + i * 40;
            }
        }

        private void InitializeNodeNetworks()
        {
            nodeNetwork = TileNodeNetworkCreator.CreateFromTypes(
                Map, DirectionalType.Four, new string[] { "Ground" });
            // todo - add ground:
            //nodeNetwork = new TileNodeNetwork(Map.X, Map.Y - Map.Height, Map.WidthPerTile.Value,
            //    Map.NumberTilesWide.Value, Map.NumberTilesTall.Value, DirectionalType.Eight);
            //nodeNetwork.LinkColor = Microsoft.Xna.Framework.Color.Gray;
            //nodeNetwork.NodeColor = Microsoft.Xna.Framework.Color.White;

            var names = Map.TileProperties
                .Where(item => item.Value.Any(subItem => 
                    subItem.Name == "Type" && (subItem.Value as string) == "Wall"))
                .Select(item => item.Key)    
                .ToArray();

            var mapHafSize = Map.WidthPerTile.Value/2.0f;

            foreach (var layer in Map.MapLayers)
            {
                foreach(var name in names)
                {
                    if(layer.NamedTileOrderedIndexes.ContainsKey(name))
                    {
                      var indexes = layer.NamedTileOrderedIndexes[name];

                        foreach(var index in indexes)
                        {
                            var bottomLeftPosition = layer.Vertices[index * 4].Position;

                            var node = nodeNetwork.TiledNodeAtWorld(
                                bottomLeftPosition.X + mapHafSize,
                                bottomLeftPosition.Y + mapHafSize);
                            if(node != null)
                            {
                                nodeNetwork.Remove(node);
                            }
                        }
                    }
                }
            }

            if(DebuggingVariables.ShowNodeNetwork)
            {
                nodeNetwork.Visible = true;
                nodeNetwork.UpdateShapes();

            }
            else
            {
                nodeNetwork.Visible = false;

            }
        }

        private void InitializeCollisions()
        {
            SolidCollisions.Visible = DebuggingVariables.ShowSolidCollision;


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

        private void InitializeUi()
        {
            ((GameScreenGumRuntime)GameScreenGum).SetNumberOfPlayers(PlayerList.Count);
        }

        #endregion

        #region Activity

        void CustomActivity(bool firstTimeCalled)
		{
            if(IsPaused == false)
            {
                spawnManager.DoActivity(EnemyList, SpawnPointList, Spawns);

                if (PlayerList.Count > 0)
                {
                    DoAi();
                }
            }

            DoUiActivity();

            DoCheckForGameOver();

            if (PlayerList.Count > 0)
            {
                DoCheckPauseInput();
            }
        }

            

        private void DoUiActivity()
        {
            (GameScreenGum as GameScreenGumRuntime).CustomActivity(
                PlayerList);
        }


        private void DoCheckPauseInput()
        {
            foreach(var player in PlayerList)
            {
                if(player.PauseInputPressed)
                {
                    if(IsPaused)
                    {
                        UnpauseThisScreen();
                        ((GameScreenGumRuntime)GameScreenGum).SetPauseScreenVisibility(false);
                    }
                    else
                    {
                        PauseThisScreen();
                        ((GameScreenGumRuntime)GameScreenGum).SetPauseScreenVisibility(true);
                    }
                    
                }
            }
        }

        private void DoCheckForGameOver()
        {
            if(PlayerList.Count <= 0 && hasGameOverBeenTriggered == false)
            {
                hasGameOverBeenTriggered = true;
                ((GameScreenGumRuntime)GameScreenGum).ShowGameOver(this);
            }
        }

        private void DoAi()
        {
            for(int i = currentFrameSkipIndex; i < EnemyList.Count; i+= AiFrameSkip)
            {
                var enemy = EnemyList[i];

                enemy.DoAiActivity(true, nodeNetwork, PlayerList, SolidCollisions);

            }
            currentFrameSkipIndex = (currentFrameSkipIndex + 1) % AiFrameSkip;

            foreach(var enemy in EnemyList)
            {
                var ai = enemy.InputDevice as TopDown.TopDownAiInput<Enemy>;

                ai.Activity();
            }
        }

        private void HandleEnemySpawn(Enemy enemy)
        {
            var input = new TopDown.TopDownAiInput<Enemy>(enemy);
            input.RemoveTargetOnReaching = true;
            input.StopOnTarget = false;
            enemy.InitializeTopDownInput(input);
        }

        #endregion

        void CustomDestroy()
		{
            Camera.Main.Detach();

		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
