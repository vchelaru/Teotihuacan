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

        I2DInput rightStick;
        Vector3 aimingVector;
        AnimationController spriteAnimationController;
        IPressableInput swapWeaponsBack;
        IPressableInput swapWeaponsForward;

        bool isPrimaryInputDown
        {
            get
            {
                if(InputDevice == InputManager.Keyboard)
                {
                    return InputManager.Mouse.ButtonDown(Mouse.MouseButtons.LeftButton);

                }
                else if(InputDevice is Xbox360GamePad gamePad)
                {
                    return gamePad.RightTrigger.IsDown;
                }
                else
                {
                    return false;
                }
            }

        }

        PrimaryActions currentPrimaryAction = PrimaryActions.idle;
        public SecondaryActions CurrentSecondaryAction
        {
            get; private set;
        } = SecondaryActions.None;
        AnimationLayer shootingLayer;
        double lastFireShotTime;
        double lastHealingTime;

        public Weapon EquippedWeapon { get; set; }

        public float CurrentHP { get; private set; }

        bool canTakeDamage => CurrentHP > 0;

        public Action UpdateHud;

        public bool PauseInputPressed => InputDevice.DefaultPauseInput.WasJustPressed;

        PositionedObject lightningAttachment;

        public LightningWeaponManager LightningWeaponManager
        {
            get; private set;
        } = new LightningWeaponManager();

        public float CurrentEnergy { get; private set; }

        public bool IsOnMud { get; set; }

        private WeaponLevelBase weaponLevel = new WeaponLevelBase();
        public float WeaponDamageModifier => weaponLevel.CurrentWeaponLevel * WeaponLevelDamageIncrement + 1;
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

            weaponLevel.ChangeWeaponType(EquippedWeapon);

            lightningAttachment = new PositionedObject();
            lightningAttachment.AttachTo(this);

            InitializeAnimationLayers();

            InitializeCollision();
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

        partial void CustomInitializeTopDownInput()
        {
            if(InputDevice is Xbox360GamePad gamePad)
            {
                rightStick = gamePad.RightStick;
                swapWeaponsBack = gamePad.GetButton(Xbox360GamePad.Button.DPadLeft);
                swapWeaponsForward = gamePad.GetButton(Xbox360GamePad.Button.DPadRight);
            }
            else if(InputDevice is Keyboard keyboard)
            {
                swapWeaponsBack = keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.Q);
                swapWeaponsForward = keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.E);
            }
        }

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
            //DoWeaponSwappingLogic();
            spriteAnimationController.Activity();
            UpdateOverlaySprite();
            LightningEndpointSprite.Visible = 
                CurrentSecondaryAction == SecondaryActions.Shooting && EquippedWeapon == Weapon.ShootingLightning;

            string debugString = $@"Current Level: {weaponLevel.CurrentWeaponLevel}
Weapon Modifier: {WeaponDamageModifier}
Weapon Drain: {1 - weaponLevel.CurrentWeaponLevel * WeaponLevelEnergyDrainDecrement}";
            
            FlatRedBall.Debugging.Debugger.Write(debugString);
		}

        private void DoWeaponSwappingLogic()
        {
            if(swapWeaponsBack.WasJustPressed)
            {
                switch(EquippedWeapon)
                {
                    case Weapon.ShootingFire: EquippedWeapon = Weapon.ShootingLightning; break;
                    case Weapon.ShootingLightning: EquippedWeapon = Weapon.ShootingSkulls; break;
                    case Weapon.ShootingSkulls: EquippedWeapon = Weapon.ShootingFire; break;
                }
            }
            if(swapWeaponsForward.WasJustPressed)
            {
                switch (EquippedWeapon)
                {
                    case Weapon.ShootingFire: EquippedWeapon = Weapon.ShootingSkulls; break;
                    case Weapon.ShootingSkulls: EquippedWeapon = Weapon.ShootingLightning; break;
                    case Weapon.ShootingLightning: EquippedWeapon = Weapon.ShootingFire; break;
                }
            }
        }

        private void DoPrimaryActionActivity()
        {
            const float movementThreashHold = 0.01f;
            currentPrimaryAction = MovementInput.Magnitude > movementThreashHold ? PrimaryActions.walk : PrimaryActions.idle;
        }

        private void DoAimingActivity()
        {
            Vector3 newAimingVector = aimingVector;
            if (rightStick != null)
            {
                if (rightStick.Magnitude > 0)
                {
                    newAimingVector = new Vector3(rightStick.X, rightStick.Y, 0);
                }
            }
            else
            {

                Vector3 cursorPosition = new Vector3(GuiManager.Cursor.WorldXAt(Z), GuiManager.Cursor.WorldYAt(Z), 0);

                newAimingVector = cursorPosition - new Vector3(X, Y, 0);
            }

            // in case the stick happens to report 0:
            if(newAimingVector.X == 0 && newAimingVector.Y == 0)
            {
                newAimingVector.X = 1;
            }

            //Normalize at the end in case the right stick input is not at max magnitude
            newAimingVector.Normalize();
            aimingVector = newAimingVector;

            LightningCollisionLine.RelativeRotationZ = (float)Math.Atan2(aimingVector.Y, aimingVector.X);
        }

        private void DoShootingActivity()
        {
            Bullet.DataCategory bulletData = null;
            if (isPrimaryInputDown)
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
            else if(isPrimaryInputDown == false)
            {
                CurrentEnergy += EnergyRecoveryRate * TimeManager.SecondDifference;
            }
            
            CurrentEnergy = System.Math.Min(CurrentEnergy, MaxEnergy);
            CurrentEnergy = System.Math.Max(CurrentEnergy, 0);

            if(DebuggingVariables.PlayersHaveInfiniteEnergy)
            {
                CurrentEnergy = MaxEnergy;
            }
        }

        private float ModifyEnergyDrain(float baseEnergyDrain)
        {
            return baseEnergyDrain * (1 - weaponLevel.CurrentWeaponLevel * WeaponLevelEnergyDrainDecrement);
        }

        private void DoShootingFireActivity(Bullet.DataCategory bulletData)
        {
            var direction = aimingVector;

            var bullet = Factories.BulletFactory.CreateNew(this.X, this.Y);
            bullet.Owner = this;
            bullet.CurrentDataCategoryState = bulletData;
            bullet.Velocity = bullet.BulletSpeed * direction;
            bullet.SetAnimationChainFromVelocity(TopDownDirectionExtensions.FromDirection(aimingVector, PossibleDirections), EquippedWeapon);

            CurrentEnergy -= ModifyEnergyDrain(bulletData.EnergyUsePerShot);

            shootingLayer.PlayOnce(GetChainName(currentPrimaryAction, SecondaryActions.Shooting));

            lastFireShotTime = FlatRedBall.Screens.ScreenManager.CurrentScreen.PauseAdjustedCurrentTime;
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

        public bool TakeDamage(float damageToTake)
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
                        FlashWhite();
                    }
                    else
                    {
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
            if (EquippedWeapon == weaponType)
            {
                //For now only add 1 experience to the weapon.
                weaponLevel.AddWeaponExperience();
            }
            else
            {
                EquippedWeapon = weaponType;
                weaponLevel.ChangeWeaponType(EquippedWeapon);
            }
        }

        #endregion

        private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}

}
