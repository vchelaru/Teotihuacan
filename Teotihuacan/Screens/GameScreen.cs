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
using FlatRedBall.TileGraphics;
using Microsoft.Xna.Framework;

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

            TileEntityInstantiator.CreateEntitiesFrom(Map);
            Map.RemoveTiles(t => t.Any(item => item.Name == "Type" && (item.Value as string) == "RemoveMe"), Map.TileProperties);

            // Create players after other entities so they can be spawned next to the base
            CreatePlayers();

            spawnManager = new SpawnManager();

            InitializeCollisions();

            CameraControllerInstance.Targets.AddRange(PlayerList);
            CameraControllerInstance.Targets.AddRange(PlayerBaseList);
            CameraControllerInstance.Map = Map;

            CameraControllerInstance.SetStartPositionAndZoom();

            Factories.EnemyFactory.EntitySpawned = HandleEnemySpawn;
            EnemyList.CollectionChanged += (a, b) => HandleEnemyListChanged();

            InitializeNodeNetworks();

            InitializeUi();
        }

        private void CreatePlayers()
        {
            var numberOfControllers = InputManager.NumberOfConnectedGamePads;

            if(numberOfControllers == 0)
            {
                var player = new Player();
                //player.EquippedWeapon = Animation.SecondaryActions.ShootingLightning;
                player.EquippedWeapon = Animation.Weapon.ShootingFire;

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
                        JoinWith(controller);
                    }
                }
            }

            for(int i = 0; i < PlayerList.Count; i++)
            {
                var player = PlayerList[i];
                SetInitialPlayerPosition(player);
            }
        }

        private void SetInitialPlayerPosition(Player player)
        {
            player.Y = this.PlayerBaseList[0].Y;
            player.X = this.PlayerBaseList[0].X + 48;
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
                    subItem.Name == "Type" && 
                        ((subItem.Value as string) == "Wall" || (subItem.Value as string) == "Pit")))
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
            PitSolidCollisions.Visible = DebuggingVariables.ShowSolidCollision;

            // add border around the tile map
            AddBorderAroundMap();

            PlayerVsSolidCollision.SetFirstSubCollision(item => item.CircleInstance);

            PlayerVsPlayerSolidCollision.SetFirstSubCollision(item => item.CircleInstance);
            PlayerVsPlayerSolidCollision.SetSecondSubCollision(item => item.CircleInstance);

            PlayerVsEnemyRelationship.SetFirstSubCollision(item => item.CircleInstance);

            PlayerLightningVsEnemyRelationship.SetFirstSubCollision(item => item.LightningCollisionLine);
            PlayerLightningVsEnemyRelationship.CollisionOccurred += HandleLightningVsEnemyCollision;
            PlayerLightningVsEnemyRelationship.IsActive = false;
            PlayerLightningVsEnemyRelationship.CollisionLimit = FlatRedBall.Math.Collision.CollisionLimit.Closest;

            BulletVsPlayerCollision.SetSecondSubCollision(item => item.CircleInstance);
            BulletVsPlayerCollision.CollisionLimit = FlatRedBall.Math.Collision.CollisionLimit.First;

            EnemyVsPlayerBaseSolidCollision.SetSecondSubCollision(item => item.SolidRectangle);

            PlayerVsPlayerBaseSolidCollision.SetFirstSubCollision(item => item.CircleInstance);
            PlayerVsPlayerBaseSolidCollision.SetSecondSubCollision(item => item.SolidRectangle);

            BulletVsEnemyCollision.CollisionLimit = FlatRedBall.Math.Collision.CollisionLimit.First;

            BulletVsPlayerBaseSolidCollision.SetSecondSubCollision(item => item.SolidRectangle);

            PlayerVsPlayerBaseHealingCollision.SetFirstSubCollision(item => item.CircleInstance);
            PlayerVsPlayerBaseHealingCollision.SetSecondSubCollision(item => item.HealingAura);
            PlayerVsPlayerBaseHealingCollision.Name = nameof(PlayerVsPlayerBaseHealingCollision);

            PlayerLightningVsSolidCollision.SetFirstSubCollision(item => item.LightningCollisionLine);
            PlayerLightningVsSolidCollision.CollisionOccurred += HandleLightningVsSolidCollision;
            PlayerLightningVsSolidCollision.IsActive = false;
            PlayerLightningVsSolidCollision.CollisionLimit = FlatRedBall.Math.Collision.CollisionLimit.Closest;

            PlayerVsPitSolidCollision.SetFirstSubCollision(item => item.CircleInstance);

            PlayerVsBulletExplosionCollision.SetFirstSubCollision(item => item.CircleInstance);

            MudCollision.Visible = true;
        }

        private void AddBorderAroundMap()
        {
            int borderSizeWide = Map.NumberTilesWide.Value + 2;

            var leftX = Map.X - Map.WidthPerTile.Value / 2.0f;
            var rightX = Map.X + Map.Width + Map.WidthPerTile.Value / 2.0f;

            float worldX = leftX;
            float topY = Map.Y + Map.HeightPerTile.Value / 2.0f;

            float bottomY = Map.Y - Map.Height - Map.HeightPerTile.Value / 2.0f;

            for (int x = 0; x < borderSizeWide; x++)
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
            
            var gameScreenGumRuntime = ((GameScreenGumRuntime)GameScreenGum);
            gameScreenGumRuntime.SetNumberOfPlayers(PlayerList.Count);

            gameScreenGumRuntime.QuitClicked += (not, used) => FlatRedBallServices.Game.Exit();
            gameScreenGumRuntime.ResumeClicked += (not, used) =>
                DoUnpause();

            gameScreenGumRuntime.StartLevel += () =>DoStartLevel();
            gameScreenGumRuntime.ShowLevelStart($"Level {LevelName}");
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

                DoCollisionActivity();

            }

            DoUiActivity();

            DoCheckForGameOver();

            if (PlayerList.Count > 0)
            {
                DoCheckPauseInput();
            }

            // do this after pause/unpause
            JoinUnjoinActivity();
        }

        private void HandleLightningVsSolidCollision(Player player, TileShapeCollection tileMap)
        {
            player.LightningWeaponManager.HandleCollisionVsSolid(player);
            
        }

        private void HandleLightningVsEnemyCollision(Player player, Enemy enemy)
        {
            player.LightningWeaponManager.HandleCollisionVsEnemy(enemy);
        }

        private void DoCollisionActivity()
        {
            var areAnyShootingLightning = false;
            foreach (var player in PlayerList)
            {
                player.LightningWeaponManager.StartCollisionFrameLogic();
                if(player.CurrentSecondaryAction == Animation.SecondaryActions.Shooting &&
                    player.EquippedWeapon == Animation.Weapon.ShootingLightning)
                {
                    areAnyShootingLightning = true;
                }
            }

            if(areAnyShootingLightning)
            {
                PlayerLightningVsSolidCollision.DoCollisions();
                PlayerLightningVsEnemyRelationship.DoCollisions();
            }

            foreach(var player in PlayerList)
            {
                player.LightningWeaponManager.EndCollisionFrameLogic(player);

                var enemyHit = player.LightningWeaponManager.EnemyHitThisFrame;
                if (enemyHit != null)
                {
                    var playerToEnemy = enemyHit.Position -
                        player.Position;

                    enemyHit.TakeLightningDamage(Player.LightningDps, player);

                    if(playerToEnemy.LengthSquared() != 0)
                    {
                        playerToEnemy.Normalize();
                        enemyHit.Velocity += playerToEnemy * Player.LightningHitAcceleration * TimeManager.SecondDifference;
                    }
                }
            }
        }

        private void JoinUnjoinActivity()
        {
            foreach(var gamePad in InputManager.Xbox360GamePads)
            {
                if(gamePad.ButtonPushed(Xbox360GamePad.Button.Start))
                {
                    // See if this is connected with no player:
                    var alreadyUsed = PlayerList.Any(item => item.InputDevice == gamePad);

                    if(alreadyUsed == false)
                    {
                        var newPlayer = JoinWith(gamePad);
                        SetInitialPlayerPosition(newPlayer);
                    }

                }
                else if(gamePad.ButtonPushed(Xbox360GamePad.Button.Back) && this.IsPaused && PlayerList.Count > 1)
                {
                    DropPlayer(PlayerList.First(item => item.InputDevice == gamePad));
                }
                else if(gamePad.IsConnected == false && PlayerList.Any(item => item.InputDevice == gamePad))
                {
                    // player disconnected, so pause and drop the player:
                    DropPlayer(PlayerList.First(item => item.InputDevice == gamePad));
                }
            }
        }

        private void DropPlayer(Player player)
        {
            player.Destroy();
        }

        private Player JoinWith(Xbox360GamePad controller)
        {
            var player = new Player();
            player.CurrentColorCategoryState =
                PlayerList.Count.ToPlayerColorCategory();
            player.EquippedWeapon = Animation.Weapon.ShootingFire;

            PlayerList.Add(player);
            player.InitializeTopDownInput(controller);

            return player;
        }

        private void DoUiActivity()
        {
            (GameScreenGum as GameScreenGumRuntime).CustomActivity(
                PlayerList, PlayerBaseList[0]);
        }

        private void DoCheckPauseInput()
        {
            foreach(var player in PlayerList)
            {
                if(player.PauseInputPressed)
                {
                    if(IsPaused)
                    {
                        DoUnpause();
                    }
                    else
                    {
                        PauseThisScreen();
                        ((GameScreenGumRuntime)GameScreenGum).SetPauseScreenVisibility(true);
                    }
                    
                }
            }
        }

        private void DoUnpause()
        {
            UnpauseThisScreen();
            ((GameScreenGumRuntime)GameScreenGum).SetPauseScreenVisibility(false);
        }

        private void DoStartLevel()
        {
            spawnManager.EnableSpawning();
        }

        private void DoCheckForGameOver()
        {
            if(hasGameOverBeenTriggered == false)
            {
                if(PlayerList.Count <= 0 || PlayerBaseList[0].CurrentHP <= 0)
                {
                    hasGameOverBeenTriggered = true;
                    ((GameScreenGumRuntime)GameScreenGum).ShowGameOver(this);
                }

            }
        }

        private void DoAi()
        {
            for(int i = currentFrameSkipIndex; i < EnemyList.Count; i+= AiFrameSkip)
            {
                var enemy = EnemyList[i];

                enemy.DoAiActivity(true, nodeNetwork, PlayerList, PlayerBaseList[0], SolidCollisions, PitSolidCollisions);

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

        private void HandleEnemyListChanged()
        {
            if(EnemyList.Count <= 0 && !spawnManager.CanSpawn && PlayerList.Count > 0 && PlayerBaseList.Count > 0)
            {
                var gameScreenGumRuntime = GameScreenGum as GameScreenGumRuntime;
                gameScreenGumRuntime.SetWaveMessageText($"Wave Complete");

                var isLevelComplete = spawnManager.CurrentWaveIndex >= Spawns.Waves.Count;

                if(isLevelComplete)
                {
                    DoLevelCompleteLogic();
                }
                else
                {
                    DoWaveCompleteLogic();
                }
            }
        }

        private void DoLevelCompleteLogic()
        {
            var gameScreenGumRuntime = GameScreenGum as GameScreenGumRuntime;

            this.Call(() =>
            {
                gameScreenGumRuntime.SetWaveMessageText($"Level Complete");
            }).After(1);

            ((GameScreenGumRuntime)gameScreenGumRuntime).FadeOutAnimation.PlayAfter(3);

            var animationDuration = ((GameScreenGumRuntime)gameScreenGumRuntime).FadeOutAnimation.Length;

            this.Call(() =>
            {
                gameScreenGumRuntime.HideWaveStateInstance();
                IsActivityFinished = true;
            }).After(3 + animationDuration);
        }

        private void DoWaveCompleteLogic()
        {
            var gameScreenGumRuntime = GameScreenGum as GameScreenGumRuntime;

            foreach (var playerBase in PlayerBaseList)
            {
                playerBase.Heal(HealingBetweenWaves);
            }
            this.Call(() =>
            {
                if (Spawns.Waves.Count > spawnManager.CurrentWaveIndex + 1)
                {
                    gameScreenGumRuntime.SetWaveMessageText($"Wave {spawnManager.CurrentWaveIndex + 1}");
                }
                else 
                {
                    gameScreenGumRuntime.SetWaveMessageText($"Final Wave");
                }
            }).After(TimeBetweenWaves * .5);

            this.Call(() =>
            {
                gameScreenGumRuntime.HideWaveStateInstance();
                if (Spawns.Waves.Count > spawnManager.CurrentWaveIndex)
                {
                    spawnManager.EnableSpawning();
                }
            }).After(TimeBetweenWaves);
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
