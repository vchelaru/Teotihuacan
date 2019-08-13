using System;
using System.Collections.Generic;
using System.Text;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;
using FlatRedBall.Math.Geometry;
using Microsoft.Xna.Framework;
using FlatRedBall.Gui;
using Teotihuacan.Animation;
using System.Linq;
using Teotihuacan.Managers;
using Teotihuacan.GameData;
using Teotihuacan.Models;
using Microsoft.Xna.Framework.Audio;

namespace Teotihuacan.Entities
{
    #region ColorStateExtension Class
    public static class ColorStateExtensions
    {
        public static int ToInt(this Player.ColorCategory category)
        {
            if (category == Player.ColorCategory.Blue)
                return 0;
            if (category == Player.ColorCategory.Red)
                return 1;
            if (category == Player.ColorCategory.Yellow)
                return 2;
            if (category == Player.ColorCategory.Green)
                return 3;

            return -1;
        }

        public static Player.ColorCategory ToPlayerColorCategory(this int value)
        {
            switch(value)
            {
                case 0: return Player.ColorCategory.Blue;
                case 1: return Player.ColorCategory.Red;
                case 2: return Player.ColorCategory.Yellow;
                case 3: return Player.ColorCategory.Green;
            }

            return Player.ColorCategory.Blue;
        }
    }
    #endregion

	public partial class Player
	{
        #region Fields/Properties

        public InputControls InputControls;
        public PlayerData PlayerData;


        private static int PLAYER_COUNT = 0;

        Vector2 aimingVector = new Vector2(1f,0f);

        AnimationController spriteAnimationController;

        public Action SwappedWeapon;

        private float playerSoundsPitchAdjustment;

        private SoundEffectInstance emptyClipSoundEffectInstance;
        private SoundEffectInstance electricShotSoundEffect;
        private SoundEffectInstance footStepLeftSoundEffect;
        private SoundEffectInstance footStepRightSoundEffect;
        private SoundEffectInstance burnPainSoundEffect;
        private SoundEffectInstance playerDeathSoundEffect;


        PrimaryActions currentPrimaryAction = PrimaryActions.idle;
        public SecondaryActions CurrentSecondaryAction
        {
            get; private set;
        } = SecondaryActions.None;
        AnimationLayer shootingLayer;
        double lastFireShotTime;
        double lastHealingTime;

        public Weapon EquippedWeapon => PlayerData.EquippedWeapon;

        public float CurrentHP { get; private set; }

        bool canTakeDamage => CurrentHP > 0;

        public Action UpdateHud;

        public bool PauseInputPressed => InputControls.WasPauseJustPressed;

        PositionedObject lightningAttachment;

        public LightningWeaponManager LightningWeaponManager
        {
            get; private set;
        } = new LightningWeaponManager();

        public float CurrentEnergy { get; private set; }

        public bool IsOnMud { get; set; }


        WeaponLevelBase CurrentWeaponLevelData
        {
            get
            {
                var found = PlayerData.WeaponLevels.FirstOrDefault(item => item.WeaponType == EquippedWeapon);

                return found;
            }
        }

        public float WeaponDamageModifier => CurrentWeaponLevelData.CurrentWeaponLevel * WeaponLevelDamageIncrement + 1;
        public int CurrentWeaponLevel => CurrentWeaponLevelData.CurrentWeaponLevel;
        public float ProgressToNextLevel => CurrentWeaponLevelData.ProgressToNextLevel();

        public Action<Player> OnPlayerDeath;
        #endregion

        #region Initialize

        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
        private void CustomInitialize()
		{
            this.PossibleDirections = PossibleDirections.EightWay;
            CurrentHP = MaxHP;
            CurrentEnergy = MaxEnergy;

            lightningAttachment = new PositionedObject();
            lightningAttachment.AttachTo(this);

            InitializeAnimationLayers();
            InitializeCollision();
            InitializeActionIcon();
            InitializeSounds();

            PLAYER_COUNT++;
        }

        private void InitializeSounds()
        {
            electricShotSoundEffect = PlayerElectricShot.CreateInstance();
            electricShotSoundEffect.IsLooped = true;

            emptyClipSoundEffectInstance = PlayerEmptyClip.CreateInstance();

            switch (PLAYER_COUNT % 4)
            {
                case 0:
                    footStepLeftSoundEffect = Player1Footstep.CreateInstance();
                    footStepRightSoundEffect = Player2Footstep.CreateInstance();
                    burnPainSoundEffect = Player1Burn.CreateInstance();
                    playerDeathSoundEffect = Player1Death.CreateInstance();
                    playerSoundsPitchAdjustment = 1f;
                    break;
                case 1:
                    footStepLeftSoundEffect = Player3Foostep.CreateInstance();
                    footStepRightSoundEffect = Player4Footstep.CreateInstance();
                    burnPainSoundEffect = Player2Burn.CreateInstance();
                    playerDeathSoundEffect = Player2Death.CreateInstance();
                    playerSoundsPitchAdjustment = 1.05f;
                    break;
                case 2:
                    footStepLeftSoundEffect = Player2Footstep.CreateInstance();
                    footStepRightSoundEffect = Player4Footstep.CreateInstance();
                    burnPainSoundEffect = Player3Burn.CreateInstance();
                    playerDeathSoundEffect = Player3Death.CreateInstance();
                    playerSoundsPitchAdjustment = 0.95f;
                    break;
                case 3:
                    footStepLeftSoundEffect = Player1Footstep.CreateInstance();
                    footStepRightSoundEffect = Player3Foostep.CreateInstance();
                    burnPainSoundEffect = Player4Burn.CreateInstance();
                    playerDeathSoundEffect = Player4Death.CreateInstance();
                    playerSoundsPitchAdjustment = 1.08f;
                    break;
            }
        }

        private void InitializeActionIcon()
        {
            if(InputControls is Xbox360GamePadControls)
            {
                CurrentInputDeviceTypeState = InputDeviceType.GamePad;
            }
            else if(InputControls is KeyboardMouseControls)
            {
                CurrentInputDeviceTypeState = InputDeviceType.Keyboard;
            }
        }

        private void InitializeAnimationLayers()
        {
            spriteAnimationController = new AnimationController(SpriteInstance);
            AnimationLayer walkAnimationLayer = new AnimationLayer();
            walkAnimationLayer.EveryFrameAction  = () =>
            {
                return GetChainName(currentPrimaryAction);
            };
            spriteAnimationController.Layers.Add(walkAnimationLayer);

            shootingLayer = new AnimationLayer();
            spriteAnimationController.Layers.Add(shootingLayer);
        }

        /*partial void CustomInitializeTopDownInput()
        {
        }*/


        private void InitializeCollision()
        {
            this.LightningCollisionLine.RelativePoint1 = new Point3D();
            this.LightningCollisionLine.RelativePoint2 = new Point3D(LightningLength, 0, 0);
            this.LightningCollisionLine.Visible = false;

            LightningCollisionLine.ParentRotationChangesRotation = false;
        }

        #endregion

        #region Activity

        private void CustomActivity()
		{
            DoPrimaryActionActivity();
            DoAimingActivity();
            DoShootingActivity();
            DoMovementValueUpdate();
            DoWeaponSwappingLogic();
            spriteAnimationController.Activity();
            UpdateOverlaySprite();
            LightningEndpointSprite.Visible = 
                CurrentSecondaryAction == SecondaryActions.Shooting && EquippedWeapon == Weapon.ShootingLightning;
            UpdateLightningSoundEffectStatus();
#if DEBUG
            if (DebuggingVariables.DisplayWeaponLevelInfo)
            {
                string debugString = $@"Current Level: {CurrentWeaponLevelData.CurrentWeaponLevel}
Weapon Modifier: {WeaponDamageModifier}
Weapon Drain: {1 - CurrentWeaponLevelData.CurrentWeaponLevel * WeaponLevelEnergyDrainDecrement}";

                FlatRedBall.Debugging.Debugger.Write(debugString);
            }
#endif
        }

        private void DoWeaponSwappingLogic()
        {
            if(InputControls.WasSwapWeaponsBackJustPressed)
            {
                switch(EquippedWeapon)
                {
                    case Weapon.ShootingFire: PlayerData.EquippedWeapon = Weapon.ShootingLightning; break;
                    case Weapon.ShootingLightning: PlayerData.EquippedWeapon = Weapon.ShootingSkulls; break;
                    case Weapon.ShootingSkulls: PlayerData.EquippedWeapon = Weapon.ShootingFire; break;
                }
                SwappedWeapon();
            }
            if(InputControls.WasSwapWeaponsForwardJustPressed)
            {
                switch (EquippedWeapon)
                {
                    case Weapon.ShootingFire: PlayerData.EquippedWeapon = Weapon.ShootingSkulls; break;
                    case Weapon.ShootingSkulls: PlayerData.EquippedWeapon = Weapon.ShootingLightning; break;
                    case Weapon.ShootingLightning: PlayerData.EquippedWeapon = Weapon.ShootingFire; break;
                }
                SwappedWeapon();

            }
        }

        private void DoPrimaryActionActivity()
        {
            const float movementThreashHold = 0.01f;
            currentPrimaryAction = MovementInput.Magnitude > movementThreashHold ? PrimaryActions.walk : PrimaryActions.idle;
        }

        private void DoAimingActivity()
        {
            Vector2? newAimingVector = InputControls.TryGetAimingVector(ref Position);

            if (newAimingVector.HasValue)
            {
                aimingVector = newAimingVector.Value;

                LightningCollisionLine.RelativeRotationZ = (float)Math.Atan2(aimingVector.Y, aimingVector.X);
            }
        }

        private void DoShootingActivity()
        {
            Bullet.DataCategory bulletData = null;
            if (InputControls.IsPrimaryFireDown)
            {
                if(EquippedWeapon == Weapon.ShootingFire)
                {
                    CurrentSecondaryAction = SecondaryActions.Shooting;
                    bulletData = Bullet.DataCategory.PlayerFire;
                }
                else if(EquippedWeapon == Weapon.ShootingLightning && CurrentEnergy > 0)
                {
                    CurrentSecondaryAction = SecondaryActions.Shooting;
                }
                else if(EquippedWeapon == Weapon.ShootingSkulls)
                {
                    CurrentSecondaryAction = SecondaryActions.Shooting;
                    bulletData = Bullet.DataCategory.PlayerSkull;
                }
                else
                {
                    CurrentSecondaryAction = SecondaryActions.None;
                }
            }
            else
            {
                CurrentSecondaryAction = SecondaryActions.None;
            }

            if(CurrentSecondaryAction == SecondaryActions.Shooting &&
                bulletData != null &&
                FlatRedBall.Screens.ScreenManager.CurrentScreen.PauseAdjustedSecondsSince(lastFireShotTime) > 
                1/ bulletData.ShotsPerSecond && 
                CurrentEnergy > ModifyEnergyDrain(bulletData.EnergyUsePerShot)
                )
            {
                DoShootingFireActivity(bulletData);
            }
            else if(CurrentSecondaryAction == SecondaryActions.Shooting && EquippedWeapon == Weapon.ShootingLightning)
            {
                CurrentEnergy -= ModifyEnergyDrain(LightningEnergyUsePerSecond) * TimeManager.SecondDifference;
            }
            else if(InputControls.IsPrimaryFireDown == false)
            {
                CurrentEnergy += EnergyRecoveryRate * TimeManager.SecondDifference;
            }
            else
            {
                if (emptyClipSoundEffectInstance?.State != SoundState.Playing)
                    emptyClipSoundEffectInstance?.Play();
            }
            
            CurrentEnergy = System.Math.Min(CurrentEnergy, MaxEnergy);
            CurrentEnergy = System.Math.Max(CurrentEnergy, 0);

            if(DebuggingVariables.PlayersHaveInfiniteEnergy)
            {
                CurrentEnergy = MaxEnergy;
            }
        }

        private void UpdateLightningSoundEffectStatus()
        {
            if (CurrentSecondaryAction == SecondaryActions.Shooting && EquippedWeapon == Weapon.ShootingLightning && CurrentEnergy > 0)
            {
                if (electricShotSoundEffect.State != SoundState.Playing) electricShotSoundEffect?.Play();
            }
            else if (electricShotSoundEffect.State == SoundState.Playing)
            {
                electricShotSoundEffect?.Stop();
            }
        }

        private float ModifyEnergyDrain(float baseEnergyDrain)
        {
            return baseEnergyDrain * (1 - CurrentWeaponLevel * WeaponLevelEnergyDrainDecrement);
        }

        private void DoShootingFireActivity(Bullet.DataCategory bulletData)
        {
            var direction = new Vector3(aimingVector.X, aimingVector.Y, 0f);

            var bullet = Factories.BulletFactory.CreateNew(this.X, this.Y);
            bullet.Owner = this;
            bullet.CurrentDataCategoryState = bulletData;
            bullet.Velocity = bullet.BulletSpeed * direction;
            bullet.SetAnimationChainFromVelocity(TopDownDirectionExtensions.FromDirection(aimingVector, PossibleDirections), EquippedWeapon);

            CurrentEnergy -= ModifyEnergyDrain(bulletData.EnergyUsePerShot);

            shootingLayer.PlayOnce(GetChainName(currentPrimaryAction, SecondaryActions.Shooting));

            lastFireShotTime = FlatRedBall.Screens.ScreenManager.CurrentScreen.PauseAdjustedCurrentTime;
            if (bulletData == Bullet.DataCategory.PlayerFire)
            {
                FlatRedBall.Audio.AudioManager.Play(PlayerFlameShot);
            } else if (bulletData == Bullet.DataCategory.PlayerSkull)
            {
                switch (FlatRedBallServices.Random.Next(0, 3))
                {
                    case 0: FlatRedBall.Audio.AudioManager.Play(PlayerShotSkull1); break;
                    case 1: FlatRedBall.Audio.AudioManager.Play(PlayerShotSkull2); break;
                    case 2: FlatRedBall.Audio.AudioManager.Play(PlayerShotSkull3); break;
                }
            }
        }

        private void DoMovementValueUpdate()
        {
            switch(CurrentSecondaryAction)
            {
                case SecondaryActions.None:
                    if(IsOnMud)
                    {
                        mCurrentMovement = TopDownValues[DataTypes.TopDownValues.OnMud];
                    }
                    else
                    {
                        mCurrentMovement = TopDownValues[DataTypes.TopDownValues.DefaultValues];
                    }
                    break;
                case SecondaryActions.Shooting:
                    if(IsOnMud)
                    {
                        mCurrentMovement = TopDownValues[DataTypes.TopDownValues.WhileShootingMud];
                    }
                    else
                    {
                        mCurrentMovement = TopDownValues[DataTypes.TopDownValues.WhileShooting];
                    }
                    break;
            }
        }

        private string GetChainName(PrimaryActions primaryAction, SecondaryActions secondaryAction = SecondaryActions.None)
        {
            Weapon? weapon = null;

            if(secondaryAction == SecondaryActions.Shooting)
            {
                weapon = EquippedWeapon;
            }
            if(aimingVector.X != 0 || aimingVector.Y != 0)
            {
                var direction = TopDownDirectionExtensions.FromDirection(new Vector2(aimingVector.X, aimingVector.Y), PossibleDirections.EightWay);

                return ChainNameHelperMethods.GenerateChainName(primaryAction, weapon, direction);
            }
            else
            {
                return ChainNameHelperMethods.GenerateChainName(primaryAction, weapon, TopDownDirection.Right);
            }
        }

        public bool TakeDamage(float damageToTake, bool isBurn = false)
        {
            bool didTakeDamage = false;

            if(canTakeDamage)
            {
                didTakeDamage = true;

                if(CurrentHP > 0)
                {
                    if(DebuggingVariables.ArePlayersInvincible == false)
                    {
                        CurrentHP -= damageToTake;
                    }

                    CurrentHP = Math.Max(CurrentHP, 0);
                    UpdateHud?.Invoke();

                    if (CurrentHP > 0)
                    {
                        if (isBurn && burnPainSoundEffect?.State != SoundState.Playing) burnPainSoundEffect?.Play();

                        FlashWhite();
                    }
                    else
                    {
                        playerDeathSoundEffect?.Play();
                        PerformDeath();
                    }

                }
            }
            return didTakeDamage;
        }

        public void Heal(float healingRate)
        {
            var amount = healingRate * TimeManager.SecondDifference;

            var newValue = CurrentHP + amount;

            this.CurrentHP = System.Math.Min(newValue, MaxHP) ;

            lastHealingTime = FlatRedBall.Screens.ScreenManager.CurrentScreen.PauseAdjustedCurrentTime;
        }

        private void PerformDeath()
        {
            var death = SpriteManager.AddParticleSprite(Death_1_SpriteSheet);
            death.AnimationChains = Death_1;
            death.CurrentChainName = nameof(PrimaryActions.Death);
            death.Position = SpriteInstance.Position;
            death.TextureScale = 1;
            death.Animate = true;
            SpriteManager.RemoveSpriteAtTime(death, death.CurrentChain.TotalLength);

            OnPlayerDeath?.Invoke(this);
            Destroy();
        }

        private void FlashWhite()
        {
            this.SpriteInstance.ColorOperation = FlatRedBall.Graphics.ColorOperation.ColorTextureAlpha;
            this.SpriteInstance.Red = 1;
            this.SpriteInstance.Green = 1;
            this.SpriteInstance.Blue = 1;

            this.Call(ReturnFromFlash).After(FlashDuration);
        }

        private void ReturnFromFlash()
        {
            this.SpriteInstance.ColorOperation = FlatRedBall.Graphics.ColorOperation.Texture;
        }

        private void UpdateOverlaySprite()
        {
            var isHealing = FlatRedBall.Screens.ScreenManager.CurrentScreen.PauseAdjustedSecondsSince(lastHealingTime) <
                .25f;
            var isLowHealth = (CurrentHP / MaxHP) < .25f;
            var isLowEnergy = (CurrentEnergy / MaxEnergy) < .25;

            if(isHealing)
            {
                OverlaySprite.Visible = true;
                CurrentOverlayColorState = Player.OverlayColor.BeingHealed;
            }
            else if(isLowEnergy)
            {
                OverlaySprite.Visible = true;
                CurrentOverlayColorState = Player.OverlayColor.CriticalMp;
            }
            else if(isLowHealth)
            {
                OverlaySprite.Visible = true;
                CurrentOverlayColorState = Player.OverlayColor.CriticalHp;
            }
            else
            {
                OverlaySprite.Visible = false;
            }

            if(OverlaySprite.Visible)
            {
                OverlaySprite.RelativePosition = SpriteInstance.RelativePosition;
                OverlaySprite.LeftTextureCoordinate = SpriteInstance.LeftTextureCoordinate;
                OverlaySprite.RightTextureCoordinate = SpriteInstance.RightTextureCoordinate;
                OverlaySprite.TopTextureCoordinate = SpriteInstance.TopTextureCoordinate;
                OverlaySprite.BottomTextureCoordinate = SpriteInstance.BottomTextureCoordinate;
                OverlaySprite.FlipHorizontal = SpriteInstance.FlipHorizontal;
                OverlaySprite.FlipVertical = SpriteInstance.FlipVertical;

                var zeroTo2 = (TimeManager.CurrentTime * 2) % 2;

                if(zeroTo2 < 1)
                {
                    OverlaySprite.Alpha = (float)(zeroTo2 * .7f);
                }
                else
                {
                    var from2 = 2 - zeroTo2;

                    OverlaySprite.Alpha = (float)(.7 * (from2));
                }

            }
        }

        public void UpdateLightningSprites()
        {
            Vector2 target = LightningWeaponManager.LineEndpoint;
            var widthPerSprite = Lightning_Stream_Anim[0].Texture.Width * 
                (Lightning_Stream_Anim[0].RightCoordinate -
                Lightning_Stream_Anim[0].LeftCoordinate);

            var widthHalf = widthPerSprite / 2.0f;

            var thisVector2 = new Vector2(this.X, this.Y);
            var thisToTarget = (target - thisVector2);
            var spriteCountFloat = thisToTarget.Length() / widthPerSprite;

            var spriteCount = 1 + (int)spriteCountFloat;


            while(LightningSpriteList.Count < spriteCount)
            {
                var sprite = SpriteManager.AddSprite(Lightning_Stream_Anim);
                // eventually attach to the rotation object.
                sprite.TextureScale = 1;
                sprite.AttachTo(lightningAttachment);
                if(LightningSpriteList.Count > 0)
                {
                    sprite.TimeIntoAnimation = LightningSpriteList[0].TimeIntoAnimation;
                }
                LightningSpriteList.Add(sprite);

            }

            while(LightningSpriteList.Count > spriteCount)
            {
                SpriteManager.RemoveSprite(LightningSpriteList.Last());
            }

            for(int i = 0; i < LightningSpriteList.Count; i++)
            {
                var sprite = LightningSpriteList[i];
                sprite.UpdateToCurrentAnimationFrame();
                sprite.RelativeX = widthHalf + widthPerSprite * i;
            }



            lightningAttachment.RelativeRotationZ =
                (float)Math.Atan2(thisToTarget.Y, thisToTarget.X);

            LightningEndpointSprite.RelativeX = target.X - X;
            LightningEndpointSprite.RelativeY = target.Y - Y;

            DoLastLightingSpriteResizeActivity();
        }

        public void ClearLightningSprites()
        {
            while(LightningSpriteList.Count > 0)
            {
                SpriteManager.RemoveSprite(LightningSpriteList.Last());
            }
        }

        void DoLastLightingSpriteResizeActivity()
        {

            var thisVector2 = new Vector2(this.X, this.Y);
            var lengthPerSprite = Lightning_Stream_Anim[0].Texture.Width *
                (Lightning_Stream_Anim[0].RightCoordinate -
                Lightning_Stream_Anim[0].LeftCoordinate);
            var thisToTarget = (LightningWeaponManager.LineEndpoint - thisVector2);

            var spriteCountFloat = thisToTarget.Length() / lengthPerSprite;
            var leftover = spriteCountFloat - (int)spriteCountFloat;
            if (leftover != 0)
            {
                var lastSprite = LightningSpriteList.Last();

                var animationFrameWidth = Lightning_Stream_Anim[0].RightCoordinate -
                    Lightning_Stream_Anim[0].LeftCoordinate;

                lastSprite.RightTextureCoordinate = lastSprite.LeftTextureCoordinate +
                    animationFrameWidth * leftover;


                var widthPerSprite = Lightning_Stream_Anim[0].Texture.Width *
                    (Lightning_Stream_Anim[0].RightCoordinate -
                    Lightning_Stream_Anim[0].LeftCoordinate);

                var widthHalf = widthPerSprite / 2.0f;

                int index = LightningSpriteList.Count - 1;
                var leftOfPreviousSprite = widthPerSprite * index;
                lastSprite.RelativeX = leftOfPreviousSprite + widthPerSprite * leftover/2.0f ;

            }
        }

        public void ConsumeWeaponDrop(Weapon weaponType)
        {
            int levelBefore = PlayerData.WeaponLevels.Single(item => item.WeaponType == weaponType).CurrentWeaponLevel;
            PlayerData.AddWeaponExperience(weaponType);
            int levelAfter = PlayerData.WeaponLevels.Single(item => item.WeaponType == weaponType).CurrentWeaponLevel;

            var isLevelUp = levelBefore != levelAfter;
            if (isLevelUp)
            {
                FlatRedBall.Audio.AudioManager.Play(PlayerFullyPowered);
            }
            else
            {
                FlatRedBall.Audio.AudioManager.Play(PlayerGetPowerup);
            }
        }

        public void SetActionIconVisibility(bool isVisible)
        {
            ActionIcon.Visible = isVisible;
        }

        #endregion

        private void CustomDestroy()
		{
            electricShotSoundEffect?.Stop();
            electricShotSoundEffect?.Dispose();

            emptyClipSoundEffectInstance?.Stop();
            emptyClipSoundEffectInstance?.Dispose();

            footStepLeftSoundEffect?.Stop();
            footStepLeftSoundEffect?.Dispose();

            footStepRightSoundEffect?.Stop();
            footStepRightSoundEffect?.Dispose();

            burnPainSoundEffect?.Stop();
            burnPainSoundEffect?.Dispose();

            playerDeathSoundEffect?.Stop();
            playerDeathSoundEffect?.Dispose();
        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
