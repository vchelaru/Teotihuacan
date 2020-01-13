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
using Microsoft.Xna.Framework.Audio;
using Teotihuacan.Models;

namespace Teotihuacan.Screens
{
	public partial class GameScreen // base class for all Level* screens
	{
        #region StatMultipliers

        public class StatMultipliers
        {
            GameScreen gameScreen;

            public float LevelHealthMultiplier = 1;
            public float LevelDamageMultiplier = 1;
            public float LevelSpeedMultiplier = 1;
            public float LevelRangeMultiplier = 1;

            public float EffectiveHealthMultiplier
            {
                get
                {
                    var playerMultiplier = 1f;
                    switch(gameScreen.PlayerList.Count)
                    {
                        case 2: playerMultiplier = 1.50f;
                            break;
                        case 3: playerMultiplier = 1.75f;
                            break;
                        case 4: playerMultiplier = 2f;
                            break;
                    }
                    return LevelHealthMultiplier * playerMultiplier;
                }
            }
            public float EffectiveDamageMultiplier => LevelDamageMultiplier;
            public float EffectiveSpeedMultiplier => LevelSpeedMultiplier;
            public float EffectiveRangeMultiplier => LevelRangeMultiplier;

            public StatMultipliers(GameScreen gameScreen)
            {
                this.gameScreen = gameScreen;
            }
        }

        #endregion

        #region Fields/Properties

        private double levelStartTime;
        private double musicFadeTime = 45.0;

        protected SoundEffectInstance LoopedBackgroundMusic;

        protected StatMultipliers CurrentMultipliers
        {
            get; private set;
        }

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
            CurrentMultipliers = new StatMultipliers(this);

            TileEntityInstantiator.CreateEntitiesFrom(Map);
            Map.RemoveTiles(t => t.Any(item => item.Name == "Type" && (item.Value as string) == "RemoveMe"), Map.TileProperties);

            //var gameScreenGumRuntime = GameScreenGum as GameScreenGumRuntime;

            // Create players after other entities so they can be spawned next to the base
            CreatePlayers();

            spawnManager = new SpawnManager();

            InitializeCollisions();

            if (PlayerList.Count > 0)
                InitializeCameraController();

            Factories.EnemyFactory.EntitySpawned = HandleEnemySpawn;
            EnemyList.CollectionChanged += (not, used) => HandleEnemyListChanged();

            InitializeNodeNetworks();

            InitializeUi();

            InitializeMusic();

            levelStartTime = FlatRedBall.TimeManager.CurrentTime;
        }

        protected virtual void InitializeMusic()
        {
            if (FlatRedBall.Audio.AudioManager.CurrentSong != null)
            {
                FlatRedBall.Audio.AudioManager.StopSong();
            }
        }

        protected void InitializeCameraController()
        {
            CameraControllerInstance.Targets = PlayerList;

            CameraControllerInstance.Map = Map;

            CameraControllerInstance.SetStartPositionAndZoom();
        }

        private void CreatePlayers()
        {
            // Auto re-join players on start of level
            // Also rejoin dead players !
            foreach (var slotPlayerData in PlayerManager.PlayersSlots)
            {
                if (slotPlayerData != null)
                {
                    if (slotPlayerData.SlotState == PlayerData.eSlotState.Full
                        ||
                        slotPlayerData.SlotState == PlayerData.eSlotState.Full_PlayerDead)
                    {
                        JoinWith(slotPlayerData);
                    }
                    else if (slotPlayerData.SlotState == PlayerData.eSlotState.Reserved_Disconnect
                             && 
                             slotPlayerData.InputControls.AreConnected)
                    {
                        JoinWith(slotPlayerData);
                    }
                }
            }
        }

        private void HandlePlayerSwappedWeapon(Player player)
        {
            ((GameScreenGumRuntime)GameScreenGum).RefreshExperienceBar(
                player, 
                UpdateType.Instant, false);
        }

        private void SetInitialPlayerPosition(Player player)
        {
            player.Y = this.PlayerBaseList[0].Y;
            player.X = this.PlayerBaseList[0].X + 48;
        }

        private void InitializeNodeNetworks()
        {
            nodeNetwork = TileNodeNetworkCreator.CreateFromTypes(
                Map, DirectionalType.Four, new string[] { "Ground", "Mud" });
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
            if(DebuggingVariables.ShowSolidCollision)
            {
                SolidCollisions.Visible = true;
                PitSolidCollisions.Visible = true;
                MudCollision.Visible = true;
                FirePitCollision.Visible = true;

                PitSolidCollisions.SetColor(Color.Gray);
                MudCollision.SetColor(Color.Orange);
                FirePitCollision.SetColor(Color.Yellow);
            }

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
            PlayerVsWeaponCollision.IsActive = false;
            PlayerVsWeaponCollision.SetFirstSubCollision(item => item.CircleInstance);
            PlayerVsWeaponCollision.CollisionLimit = FlatRedBall.Math.Collision.CollisionLimit.Closest;

            PlayerVsMudCollision.IsActive = false;
            PlayerVsMudCollision.SetFirstSubCollision(item => item.CircleInstance);
            PlayerVsMudCollision.CollisionOccurred += (player, tileShapeCollection) => player.IsOnMud = true;

            PlayerVsFirePitCollision.SetFirstSubCollision(item => item.CircleInstance);

            EnemyVsMudCollision.IsActive = false;
            EnemyVsMudCollision.CollisionOccurred += (enemy, tileShapeCollection) => enemy.IsOnMud = true;

            EnemyVsSolidCollision.SetFirstSubCollision(item => item.NavigationCollider);
            EnemyVsPitSolidCollision.SetFirstSubCollision(item => item.NavigationCollider);
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
            //gameScreenGumRuntime.SetNumberOfPlayers(PlayerList.Count);

            gameScreenGumRuntime.QuitClicked += (not, used) => FlatRedBallServices.Game.Exit();
            gameScreenGumRuntime.ResumeClicked += (not, used) => DoUnpause();
            gameScreenGumRuntime.ClearDataClicked += (not, used) =>
            {
                PlayerManager.ClearAll();
                this.MoveToScreen(nameof(MainMenuScreen));
            };

            gameScreenGumRuntime.StartLevel += () => DoStartLevel();
            gameScreenGumRuntime.ShowLevelStartOverlay($"{LevelName}");

            int numberOfControlsConnected = 1; // 1 for keyboard+mouse
            foreach (var gamePad in InputManager.Xbox360GamePads)
            {
                if (gamePad.IsConnected)
                    numberOfControlsConnected++;
            }
            gameScreenGumRuntime.SetJoinHUDsVisibility(numberOfControlsConnected);

            foreach (var player in PlayerList)
            {
                SetPlayerHudOnJoin(player);
            }
        }

        /*private void AssignPlayerData(Player player)
        {
            // see if it's cached

            //var inputDevice = player.InputDevice;

            if (PlayerManager.PlayersSlots[player.Index] != null)
            {
                player.PlayerData =
                    PlayerManager.PlayersSlots[player.Index];
            }
            else
            {
                // try to load or create:
                player.PlayerData =
                    PlayerManager.TryLoadPlayerData(player.Index);

                if(player.PlayerData == null)
                {
                    player.PlayerData = new PlayerData(player);
                    player.PlayerData.InitializeAllWeapons();
                }

                PlayerManager.PlayersSlots[player.Index] =
                    player.PlayerData;
            }

            player.OnPlayerDeath += OnPlayerDeath;
        }*/

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
            
            DoCheckPauseInput();

            // do this after pause/unpause
            PlayersJoinLeaveActivity();
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

                if(player.CurrentSecondaryAction == Animation.SecondaryActions.Shooting 
                   &&
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


            // reset mud before doing collision
            foreach(var player in PlayerList)
            {
                player.IsOnMud = false;
                player.SetActionIconVisibility(false);
            }

            foreach(var enemy in EnemyList)
            {
                enemy.IsOnMud = false;
            }

            PlayerVsWeaponCollision.DoCollisions();
            PlayerVsMudCollision.DoCollisions();
            EnemyVsMudCollision.DoCollisions();
        }

        private void PlayersJoinLeaveActivity()
        {
            // -- Joining
            if (PlayerManager.ActivePlayers.Count < PlayerManager.MaxNumberOfPlayers)
            {
                //foreach (var gamePad in InputManager.Xbox360GamePads)
                Xbox360GamePad gamePad;
                for (int gamepadIndex = 0; gamepadIndex < InputManager.Xbox360GamePads.Length; gamepadIndex++)
                {
                    gamePad = InputManager.Xbox360GamePads[gamepadIndex];
                    if (gamePad.ButtonPushed(InputControls.Xbox360GamePad_Button_Join))
                    {
                        // See if this is connected with no player:
                        // Or if input device did not die this level
                        /*var canJoin = PlayerList.Any(item => 
                                        item.InputControls.ControlsID == gamepadIndex
                                      ) == false 
                                      &&
                                      PlayerManager.DeadPlayers.Any(playerData => 
                                        playerData.InputControls.ControlsID == gamepadIndex
                                      ) == false;*/

                        PlayerData slotPlayerData;
                        if (CanPlayerJoin(gamepadIndex, out slotPlayerData))
                        {
                            Player newPlayer;
                            if (slotPlayerData != null)
                                newPlayer = JoinWith(slotPlayerData);
                            else
                                newPlayer = JoinWith(new Xbox360GamePadControls(gamePad, gamepadIndex));

                            if (newPlayer != null)
                                SetPlayerHudOnJoin(newPlayer);
                        }
                    }
                }

                if (InputManager.Keyboard.KeyPushed(InputControls.KeyboardAndMouse_Button_Join))
                {
                    /*var canJoin = PlayerList.Any(item => item.InputDevice == gamePad) == false
                                      &&
                                      PlayerManager.DeadPlayers.Any(playerData =>
                                        playerData.InputControls.ControlsID == InputControls.KeyboardAndMouse_ControlsID
                                      ) == false;

                    if (canJoin)
                    {
                        JoinPlayer(keyboard);
                    }*/

                    PlayerData slotPlayerData;
                    if (CanPlayerJoin(InputControls.KeyboardAndMouse_ControlsID, out slotPlayerData))
                    {
                        Player newPlayer;
                        if (slotPlayerData != null)
                            newPlayer = JoinWith(slotPlayerData);
                        else
                            newPlayer = JoinWith(new KeyboardMouseControls());

                        if (newPlayer != null)
                            SetPlayerHudOnJoin(newPlayer);
                    }
                }
            }

            // -- Leaving & Disconnects

            /*Xbox360GamePad gamePad;
            for (int gamepadIndex = 0; gamepadIndex < InputManager.Xbox360GamePads.Length; gamepadIndex++)
            {
                gamePad = InputManager.Xbox360GamePads[gamepadIndex];

                if (gamePad.ButtonPushed(InputControls.Xbox360GamePad_Button_Leave) && PlayerList.Count > 1)
                {
                    DropPlayer();
                }
            }

            if (InputManager.Keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.Delete) && PlayerList.Count > 1)
            {
                xDropPlayer(PlayerList.First(item => item.InputDevice == keyboard));
            }*/

            /*PlayerData playerSlotData;
            for (int i = PlayerManager.ActivePlayers.Count - 1; i > -1; i--)
            {
                playerSlotData = PlayerManager.ActivePlayers[i];

                if (playerSlotData.InputControls.AreConnected == false)
                {
                    // player disconnected, so pause ? and drop the player:
                    playerSlotData.SlotState = PlayerData.eSlotState.Reserved_Disconnect;
                    DropPlayer(playerSlotData);
                }
                else if (playerSlotData.InputControls.WasLeaveJustPressed && this.IsPaused)
                {
                    // player wants to leave
                    playerSlotData.SlotState = PlayerData.eSlotState.Reserved_Left;
                        
                    DropPlayer(playerSlotData);
                }
            }*/

            foreach (var playerSlotData in PlayerManager.PlayersSlots)
            {
                if (playerSlotData != null)
                {
                    // Have to account for dead players who want to leave as well
                    // TODO: need to separate eSlotState and dead / alive state ?

                    if (playerSlotData.InputControls.AreConnected == false)
                    {
                        // player disconnected, so pause ? and drop the player:
                        DropPlayer(playerSlotData, PlayerData.eSlotState.Reserved_Disconnect);
                    }
                    else if (playerSlotData.InputControls.WasLeaveJustPressed && this.IsPaused)
                    {
                        // player wants to leave
                        if (playerSlotData.SlotState == PlayerData.eSlotState.Full)
                            DropPlayer(playerSlotData, PlayerData.eSlotState.Reserved_Left);
                        // else PlayerData.eSlotState.FullPlayerDead
                        //  stays in dead state
                    }
                }
            }
        }

        private bool CanPlayerJoin(int controlsID, out PlayerData playerdata)
        {
            foreach (var slotData in PlayerManager.PlayersSlots)
            {
                if (slotData != null
                    &&
                    slotData.InputControls != null
                    &&
                    slotData.InputControls.ControlsID == controlsID)
                {
                    /*
                    Free = 0,                   = cant't happen

                    Reserved_Left = 1,           = can re-join
                    Reserved_Disconnect = 2,     = can re-join

                    FullPlayer_Dead = 3,         = can't re-join this level

                    Full = 4,                   = cant't happen (can't re-join)
                    */

                    // Can join if 
                    //  has no playerdata
                    //  has player data state ReservedLeft or ReservedDisconnect

                    if (slotData.SlotState <= PlayerData.eSlotState.Reserved_Disconnect)
                    {
                        playerdata = slotData;
                        return true;
                    }
                    else
                    {
                        playerdata = null;
                        return false;
                    }
                }
            }

            // PlayerData for this controls not found in any slot = not previously connected
            playerdata = null;
            return true;
        }

        protected Player JoinWith(InputControls inputControls)
        {
            //PlayerWeaponLevelManager.AddUniqueInputDevice(inputDevice);
            PlayerData playerData;
            if (!PlayerManager.TryAssignSlotToPlayer(inputControls, out playerData))
                return null;

            return JoinWith(playerData);
        }
        protected Player JoinWith(PlayerData playerData)
        {
            //PlayerWeaponLevelManager.AddUniqueInputDevice(inputDevice);
            playerData.SlotState = PlayerData.eSlotState.Full;

            var newPlayer = new Player();

            newPlayer.CurrentColorCategoryState =
                playerData.SlotIndex.ToPlayerColorCategory();
            newPlayer.SwappedWeapon += () => HandlePlayerSwappedWeapon(newPlayer);
            newPlayer.OnPlayerDeath += OnPlayerDeath;
            newPlayer.PlayerData = playerData;
            newPlayer.InputControls = playerData.InputControls;
            newPlayer.InitializeTopDownInput(playerData.InputControls.PrimaryInputDevice);
            newPlayer.InitializeActionIcon();

            PlayerList.Add(newPlayer);

            SetInitialPlayerPosition(newPlayer);

            return newPlayer;
        }
        /*private Player JoinWith(int controlsID)
        {
            //PlayerWeaponLevelManager.AddUniqueInputDevice(inputDevice);
            PlayerData playerData;
            if (!PlayerManager.TryAssignSlotToPlayer(inputControls, out playerData))
                return null;

            var player = new Player();

            player.CurrentColorCategoryState =
                playerData.SlotIndex.ToPlayerColorCategory();

            PlayerList.Add(player);
            player.InitializeTopDownInput(inputControls.PrimaryInputDevice);

            player.SwappedWeapon += () => HandlePlayerSwappedWeapon(player);

            //AssignPlayerData(player);
            player.PlayerData = playerData;

            SetInitialPlayerPosition(player);

            return player;
        }*/
        protected void SetPlayerHudOnJoin(Player newPlayer)
        {
            //var playerSlotIndex = 
            var gameScreenGumRuntime = GameScreenGum as GameScreenGumRuntime;

            gameScreenGumRuntime.PlayerJoinHuds[newPlayer.PlayerData.SlotIndex].Visible = false;
            gameScreenGumRuntime.PlayerHuds[newPlayer.PlayerData.SlotIndex].Visible = true;

            gameScreenGumRuntime
                .RefreshExperienceBar(
                newPlayer,
                UpdateType.Instant, false
            );
        }

        private void DropPlayer(PlayerData playerSlotData, PlayerData.eSlotState reason)
        {
            PlayerManager.SetPlayerRemoved(playerSlotData, reason);

            var gameScreenGumRuntime = GameScreenGum as GameScreenGumRuntime;

            gameScreenGumRuntime.PlayerHuds[playerSlotData.SlotIndex].Visible = false;
            gameScreenGumRuntime.PlayerDeadHuds[playerSlotData.SlotIndex].Visible = false;
            gameScreenGumRuntime.PlayerJoinHuds[playerSlotData.SlotIndex].Visible = true;

            Player player = PlayerList.FirstOrDefault(p => p.PlayerData.SlotIndex == playerSlotData.SlotIndex);
            if (player != null)
            {
                player.SwappedWeapon -= () => HandlePlayerSwappedWeapon(player);
                player.OnPlayerDeath -= OnPlayerDeath;

                player.Destroy();
            }
        }

        void OnPlayerDeath(Player deadPlayer)
        {
            //deadPlayerInputDevices.Add(deadPlayer.InputDevice);
            PlayerManager.SetPlayerDead(deadPlayer.PlayerData);

            var gameScreenGumRuntime = GameScreenGum as GameScreenGumRuntime;
            gameScreenGumRuntime.PlayerHuds[deadPlayer.PlayerData.SlotIndex].Visible = false;
            gameScreenGumRuntime.PlayerJoinHuds[deadPlayer.PlayerData.SlotIndex].Visible = false;
            gameScreenGumRuntime.PlayerDeadHuds[deadPlayer.PlayerData.SlotIndex].Visible = true;

            foreach (var enemy in EnemyList)
            {
                enemy.ReactToPlayerDeath(deadPlayer);
            }

            deadPlayer.SwappedWeapon -= () => HandlePlayerSwappedWeapon(deadPlayer);
            deadPlayer.OnPlayerDeath -= OnPlayerDeath;
        }

        private void DoUiActivity()
        {
            (GameScreenGum as GameScreenGumRuntime).CustomActivity(
                PlayerList, PlayerBaseList[0]/*, deadPlayerInputDevices*/);
        }

        private void DoCheckPauseInput()
        {
            if (PlayerList.Count > 0)
            {
                foreach (var player in PlayerList)
                {
                    if (player.PauseInputPressed)
                    {
                        if (IsPaused)
                        {
                            DoUnpause();
                        }
                        else
                        {
                            PauseThisScreen(); // generated method
                            ((GameScreenGumRuntime)GameScreenGum).ShowPauseMenu();
                        }
                    }
                }
            }
        }

        private void DoUnpause()
        {
            ((GameScreenGumRuntime)GameScreenGum).HidePauseMenu();
            UnpauseThisScreen(); // generated
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
                    FlatRedBall.Audio.AudioManager.Play(GameOver);
                    hasGameOverBeenTriggered = true;
                    ((GameScreenGumRuntime)GameScreenGum).ShowGameOverOverlay(this);
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

            for (int i = EnemyList.Count -1; i >= 0; i --)
            {
                EnemyList[i].DoPostAiActivity();
            }
        }

        private void HandleEnemySpawn(Enemy enemy)
        {
            var input = new TopDown.TopDownAiInput<Enemy>(enemy);
            input.RemoveTargetOnReaching = true;
            input.StopOnTarget = false;
            enemy.InitializeTopDownInput(input);

            enemy.StatMultipliers = CurrentMultipliers;
            enemy.TopDownSpeedMultiplier = CurrentMultipliers.EffectiveSpeedMultiplier;
        }

        /// <summary>
        /// Checks if enemy list is empty and if, calls wave complete or level complete code.
        /// </summary>
        private void HandleEnemyListChanged()
        {
            if(EnemyList.Count <= 0 && !spawnManager.CanSpawn && PlayerList.Count > 0 && PlayerBaseList.Count > 0)
            {
                var gameScreenGumRuntime = GameScreenGum as GameScreenGumRuntime;
                gameScreenGumRuntime.ShowWaveStateOverlay($"Wave Complete");
                FlatRedBall.Audio.AudioManager.Play(WaveEnds);

                var isLevelComplete = spawnManager.CurrentWaveIndex >= Spawns.Waves.Count;
                if (isLevelComplete)
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

            // Wait 1 sec & Show level finished overlay
            this.Call(() =>
            {
                gameScreenGumRuntime.ShowWaveStateOverlay($"Level Complete");
            }).After(1);

            // Wait 3 sec and fade out
            gameScreenGumRuntime.FadeOutAnimation.PlayAfter(5);

            var animationDuration = gameScreenGumRuntime.FadeOutAnimation.Length;

            // Wait for fade out finished and wait 3 sec and finish level
            this.Call(() =>
            {
                gameScreenGumRuntime.HideWaveStateOverlay();
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
                    gameScreenGumRuntime.ShowWaveStateOverlay($"Wave {spawnManager.CurrentWaveIndex + 1}");
                }
                else 
                {
                    gameScreenGumRuntime.ShowWaveStateOverlay($"Final Wave");
                }
            }).After(TimeBetweenWaves * .5);

            this.Call(() =>
            {
                gameScreenGumRuntime.HideWaveStateOverlay();
                if (Spawns.Waves.Count > spawnManager.CurrentWaveIndex)
                {
                    spawnManager.EnableSpawning();
                }
            }).After(TimeBetweenWaves);

            //Final wave, but without delay
            if (Spawns.Waves.Count <= spawnManager.CurrentWaveIndex + 1)
            {
                FlatRedBall.Audio.AudioManager.Play(WaveStart);
            }
        }

        #endregion

        void CustomDestroy()
		{
            PlayerManager.SaveAll();

            Camera.Main.Detach();
            LoopedBackgroundMusic?.Stop();
            LoopedBackgroundMusic?.Dispose();
        }

        static void CustomLoadStaticContent(string contentManagerName) { }

	}
}
