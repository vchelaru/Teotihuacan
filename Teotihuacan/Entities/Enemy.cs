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
using Teotihuacan.TopDown;
using System.Linq;
using FlatRedBall.TileCollisions;
using Microsoft.Xna.Framework;
using Teotihuacan.Animation;
using FlatRedBall.Math;
using Teotihuacan.Screens;

namespace Teotihuacan.Entities
{
    #region Enums

    public enum Behavior
    {
        Chasing,
        Shooting,
        Reloading
    }

    #endregion

    public partial class Enemy : ITopDownEntity
	{
        #region Fields/Properties

        public GameScreen.StatMultipliers StatMultipliers;

        Polygon lineOfSightPathFindingPolygon;

        Behavior CurrentBehavior;

        double lastFireShotTime;

        int shotsLeftInClip;

        public float CurrentHP { get; private set; }

        Vector3 aimingVector = Vector3.Right;

        bool canTakeDamage => CurrentHP > 0;

        AnimationController spriteAnimationController;
        AnimationLayer shootingAnimationLayer;

        PrimaryActions currentPrimaryAction;

        PositionedObject target;
        public PositionedObject CurrentTarget => target;

        Player playerThatDealtKillingBlow;

        PositionedObject forcedTarget;
        double timeForcedTargetSet;

        // damage bools
        bool isTakingLightningDamage;
        bool isFlashingWhite;

        /// <summary>
        /// The ratio used to multiply the perpendicular vector by. This can be
        /// viewed as the enemy "personality" which controls whether the enemy likes to attack from the left or right.
        /// </summary>
        float perpendicularLengthRatio;


        // ai visualization:
        Line thisToPerpendicularTarget;

        public bool IsOnMud { get; set; }


        #endregion

        #region Initialize

        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
        private void CustomInitialize()
        {
            InitializeAi();

            shotsLeftInClip = ClipSize;
            InitializeAnimations();
        }

        private void InitializeAi()
        {
            if (lineOfSightPathFindingPolygon == null)
            {
                lineOfSightPathFindingPolygon = Polygon.CreateRectangle(7, 7);
            }

            perpendicularLengthRatio = FlatRedBallServices.Random.Between(-1, 1);

            if(DebuggingVariables.ShowEnemyAiShapes)
            {
                thisToPerpendicularTarget = new Line();
                thisToPerpendicularTarget.Visible = true;
            }
        }

        private void InitializeAnimations()
        {
            spriteAnimationController = new AnimationController(SpriteInstance);

            AnimationLayer walkingLayer = new AnimationLayer();
            walkingLayer.EveryFrameAction = () =>
            {
                return GetChainName(currentPrimaryAction, SecondaryActions.None);
            };
            spriteAnimationController.Layers.Add(walkingLayer);

            shootingAnimationLayer = new AnimationLayer();
            spriteAnimationController.Layers.Add(shootingAnimationLayer);
        }

        #endregion

        #region Activity

        private void CustomActivity()
        {
            
        }

        public void DoPostAiActivity()
        {
            if (CurrentHP > 0)
            {
                RefreshVisualsFromDamageState();
                DoShootingActivity(target);
            }
            spriteAnimationController.Activity();
        }

        public void DoAiActivity(bool refreshPath, NodeNetwork nodeNetwork, 
            PositionedObjectList<Player> players, PlayerBase playerBase, TileShapeCollection solidCollisions, TileShapeCollection pitCollision)
        {
            if (CurrentHP > 0)
            {
                DoTargetDecision(players, playerBase);

                if (refreshPath)
                {
                    // enemies always move towards player, but really slowly when shooting
                    RefreshPath(nodeNetwork, target, solidCollisions, pitCollision);
                }

                UpdateAimingBehavior(target);

                UpdatePrimaryAction();

                UpdateCurrentBehavior(nodeNetwork, target);

                UpdateCurrentMovementValues();
            }
        }

        private void RefreshVisualsFromDamageState()
        {
            if(isFlashingWhite)
            {
                this.SpriteInstance.ColorOperation =
                    FlatRedBall.Graphics.ColorOperation.ColorTextureAlpha;
                this.SpriteInstance.Red = 1;
                this.SpriteInstance.Green = 1;
                this.SpriteInstance.Blue = 1;

            }
            else if(isTakingLightningDamage)
            {
                SpriteInstance.ColorOperation =
                    FlatRedBall.Graphics.ColorOperation.ColorTextureAlpha;

                SpriteInstance.Red = 0;
                SpriteInstance.Green = 1;
                SpriteInstance.Blue = 1;
            }
            else
            {
                SpriteInstance.ColorOperation =
                    FlatRedBall.Graphics.ColorOperation.Texture;
            }

            // lightning only lasts 1 frame:
            isTakingLightningDamage = false;
        }

        private void DoTargetDecision(PositionedObjectList<Player> players, PlayerBase playerBase)
        {

            if(forcedTarget != null)
            {
                target = forcedTarget;

                var timeSinceSet =
                    FlatRedBall.Screens.ScreenManager.CurrentScreen.PauseAdjustedSecondsSince(timeForcedTargetSet);

                if(timeSinceSet > AggroDuration)
                {
                    forcedTarget = null;
                }
            }
            else if(target == null)
            {
                PositionedObject closest = GetClosest(players, playerBase);

                target = closest;
            }
            else
            {
                if(CurrentBehavior == Behavior.Chasing)
                {
                    // see if there is anything closer
                    target = GetClosest(players, playerBase);
                }
                // else if shooting/reloading, chase that same target so long as in chasing mode

            }
        }

        private PositionedObject GetClosest(PositionedObjectList<Player> players, PlayerBase playerBase)
        {
            PositionedObject closest = null;
            // get closest:
            float closestDistance = float.PositiveInfinity;

            foreach (var player in players)
            {
                var distanceToPlayer = (player.Position - this.Position).Length();
                if (distanceToPlayer < closestDistance)
                {
                    closestDistance = distanceToPlayer;
                    closest = player;
                }
            }

            var distancetoBase = (playerBase.Position - this.Position).Length();
            if(distancetoBase < closestDistance)
            {
                closestDistance = distancetoBase;
                closest = playerBase;
            }

            return closest;
        }

        private void UpdatePrimaryAction()
        {
            const float movementThreashHold = 0.01f;
            currentPrimaryAction = Velocity.LengthSquared() > movementThreashHold ? PrimaryActions.walk : PrimaryActions.idle;
            
        }

        private void UpdateAimingBehavior(PositionedObject target)
        {
            if (CurrentDataCategoryState != DataCategory.Boss || 
                (CurrentDataCategoryState == DataCategory.Boss && CurrentBehavior != Behavior.Reloading))
            {
                if (X - target.X != 0 || Y - target.Y != 0)
                {
                    aimingVector = new Vector3(target.X, target.Y, 0) - new Vector3(X, Y, 0);
                    aimingVector.Normalize();
                }
            }
        }

        private void UpdateCurrentMovementValues()
        {
            bool isBossReloading = false;
            if (CurrentBehavior == Behavior.Shooting)
            {
                mCurrentMovement = TopDownValues[DataTypes.TopDownValues.WhileShooting];
            }
            else if(CurrentBehavior == Behavior.Reloading)
            {
                if (CurrentDataCategoryState == DataCategory.Boss)
                {
                    mCurrentMovement = TopDownValues[DataTypes.TopDownValues.BossReloading];
                    currentPrimaryAction = PrimaryActions.reloading;
                    isBossReloading = true;
                }
                else
                {
                    mCurrentMovement = TopDownValues[DataTypes.TopDownValues.WhileReloading];
                }
            }
            else 
            {
                if(this.CurrentDataCategoryState == DataCategory.SuiciderFast)
                {
                    mCurrentMovement = TopDownValues[DataTypes.TopDownValues.SuiciderFast];
                }
                else
                {
                    mCurrentMovement = TopDownValues[DataTypes.TopDownValues.DefaultValues];
                }
            }

            if(IsOnMud && !isBossReloading)
            {
                if (this.CurrentDataCategoryState == DataCategory.SuiciderFast)
                {
                    mCurrentMovement = TopDownValues[DataTypes.TopDownValues.SuiciderFastMud];
                }
                else
                {
                    mCurrentMovement = TopDownValues[DataTypes.TopDownValues.EnemyMud];
                }
            }
        }

        private void UpdateCurrentBehavior(NodeNetwork nodeNetwork, PositionedObject target)
        {
            if(shotsLeftInClip == 0)
            {
                // do reload activity
                CurrentBehavior = Behavior.Reloading;

                var isDoneReloading = FlatRedBall.Screens.ScreenManager.CurrentScreen.PauseAdjustedSecondsSince(lastFireShotTime) >
                    SecondsForReload;

                if(isDoneReloading)
                {
                    shotsLeftInClip = ClipSize;
                    CurrentBehavior = Behavior.Chasing;
                }
                //shootingAnimationLayer.StopPlay();
            }
            else
            {
                var ai = InputDevice as TopDown.TopDownAiInput<Enemy>;
                var path = ai.Path;

                bool isLineOfSight = path.Count < 2 && target != null;

                bool isInRange = false;

                if(isLineOfSight)
                {
                    isInRange = (target.Position - this.Position).Length() < MaxShootingDistance * StatMultipliers.EffectiveRangeMultiplier;
                }

                var shouldWalkForward = true;
                
                if(isInRange && MaxShootingDistance > 0)
                {
                    // line of site
                    CurrentBehavior = Behavior.Shooting;

                    if((this.Position - target.Position).Length() < ClosestShootingDistance)
                    {
                        shouldWalkForward = false;
                    }
                }
                else
                {
                    CurrentBehavior = Behavior.Chasing;
                }

                ai.IsActive = shouldWalkForward;
            }
        }

        private void RefreshPath(NodeNetwork nodeNetwork, PositionedObject target, TileShapeCollection solidCollisions, TileShapeCollection pitCollision)
        {
            var ai = InputDevice as TopDown.TopDownAiInput<Enemy>;

            // We are going to pathfind to a location
            // that is not exactly on the target but on
            // a point on a line perpendicular to the enemy.
            // This is a quicky way to add some randomness and 
            // spread out the enemies so they come from different directions

            var pathfindingTarget = target.Position;

            var lineToTarget = target.Position - this.Position;
            var perpendicular = new Vector3(-lineToTarget.Y, lineToTarget.X, 0);
            if(perpendicular.Length() != 0)
            {
                perpendicular.Normalize();
                var distanceFromTarget = lineToTarget.Length();

                const float distanceToPerpendicularLengthRatio = 1 / 2f;

                pathfindingTarget = target.Position + perpendicular * perpendicularLengthRatio * distanceToPerpendicularLengthRatio * distanceFromTarget;

            }
            if(DebuggingVariables.ShowEnemyAiShapes)
            {
                thisToPerpendicularTarget.SetFromAbsoluteEndpoints(this.Position, pathfindingTarget);
            }

            var path = nodeNetwork.GetPathOrClosest(ref Position, ref pathfindingTarget);
            ai.Path.Clear();
            var points = path.Select(item => item.Position).ToList();

            while (points.Count > 0)
            {
                var length = (points[0] - Position).Length();
                lineOfSightPathFindingPolygon.SetPoint(0, length / 2.0f, NavigationCollider.Radius);
                lineOfSightPathFindingPolygon.SetPoint(1, length / 2.0f, -NavigationCollider.Radius);
                lineOfSightPathFindingPolygon.SetPoint(2, -length / 2.0f, -NavigationCollider.Radius);
                lineOfSightPathFindingPolygon.SetPoint(3, -length / 2.0f, NavigationCollider.Radius);
                lineOfSightPathFindingPolygon.SetPoint(4, length / 2.0f, NavigationCollider.Radius);

                lineOfSightPathFindingPolygon.X = (points[0].X + Position.X) / 2.0f;
                lineOfSightPathFindingPolygon.Y = (points[0].Y + Position.Y) / 2.0f;

                var angle = (float)System.Math.Atan2(points[0].Y - Position.Y, points[0].X - Position.X);
                lineOfSightPathFindingPolygon.RotationZ = angle;

                var hasClearPath = !solidCollisions.CollideAgainst(lineOfSightPathFindingPolygon) && !pitCollision.CollideAgainst(lineOfSightPathFindingPolygon);

                if (hasClearPath && points.Count > 1)
                {
                    points.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }


            ai.Path.AddRange(points);
            ai.Target = ai.Path.FirstOrDefault();
        }

        private void DoShootingActivity(PositionedObject target)
        {
            
            if (CurrentBehavior == Behavior.Shooting &&
                FlatRedBall.Screens.ScreenManager.CurrentScreen.PauseAdjustedSecondsSince(lastFireShotTime) >
                (1 / FireShotsPerSecond)
                )
            {
                // Rotate the aiming vector to the farthest angle counterclockwise. (Positive)
                // For each increment rotate clockwise. (Negative)
                Vector3 currentAimingVector = RotateVector(aimingVector, BulletSpreadHalfAngle);
                float spreadIncremet = -(BulletSpreadHalfAngle * 2) / Math.Max((BulletsToFire -1), 1);

                for (int i = 0; i < BulletsToFire; i++)
                {

                    var bullet = Factories.BulletFactory.CreateNew(this.X, this.Y);
                    bullet.Z = this.Z - 1;
                    bullet.CurrentDataCategoryState = Bullet.DataCategory.EnemyBullet;
                    bullet.Velocity = bullet.BulletSpeed * currentAimingVector;
                    //bullet.SetAnimationChainFromVelocity(TopDownDirectionExtensions.FromDirection(currentAimingVector, PossibleDirections), Weapon.ShootingFire);

                    currentAimingVector = RotateVector(currentAimingVector, spreadIncremet);
                }
                shootingAnimationLayer.PlayOnce(GetChainName(PrimaryActions.shoot, SecondaryActions.Shooting));
                lastFireShotTime = FlatRedBall.Screens.ScreenManager.CurrentScreen.PauseAdjustedCurrentTime;
                FlatRedBall.Audio.AudioManager.Play(EnemyShot);
                shotsLeftInClip--;
            }
        }
        private Vector3 RotateVector(Vector3 vector, float angleDegrees)
        {
            /// **************************
            /// EARLY OUT
            ///***************************
            if(angleDegrees == 0)
            {
                return vector;
            }
            /// **************************
            /// END EARLY OUT
            ///***************************

            var angleRadians = angleDegrees * (Math.PI / 180);
            float cos = (float)Math.Cos(angleRadians);
            float sin = (float)Math.Sin(angleRadians);
            
            return new Vector3(vector.X * cos - vector.Y * sin, vector.X * sin + vector.Y * cos, 0);
        }

        public void TakeNonLethalDamage(float damageToTake)
        {
            if(canTakeDamage)
            {
                if(CurrentHP - damageToTake > 0)
                {
                    CurrentHP -= damageToTake;
                    FlashWhite();

                }
            }
        }

        public bool TakeDamage(PositionedObject damageDealer, float damageToTake, Player owner)
        {
            bool tookDamage = false;
            if(canTakeDamage)
            {
                tookDamage = true;
                var modifedDamage = ModifyDamageToTake(damageDealer, owner.WeaponDamageModifier, damageToTake);
                CurrentHP -= modifedDamage;

                if(CurrentHP <= 0)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"Took {modifedDamage} damage and died");
                    playerThatDealtKillingBlow = owner;
                    PerformDeath();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"Took {modifedDamage} damage and has {CurrentHP} left");
                    FlashWhite();
                    FlatRedBall.Audio.AudioManager.Play(EnemyHitByShot);
                    SetForcedTarget(owner);
                }
            }

            return tookDamage;
        }

        private void FlashWhite()
        {
            isFlashingWhite = true;
            // We may need to be more careful here if there's other instructions.
            this.Instructions.Clear();
            this.Call(() => isFlashingWhite = false).After(FlashDuration);
        }

        private float ModifyDamageToTake(PositionedObject damageDealer, float weaponLevelModifier, float baseDamage)
        {
            float toReturn = baseDamage;
            if (ShieldHalfAngle != 0)
            {
                Vector3 directionOfDamage = damageDealer.Position - Position;
                directionOfDamage.Normalize();
                var directionFacing = TopDownDirectionExtensions.FromDirection(new Vector2(aimingVector.X, aimingVector.Y), PossibleDirections.EightWay);
                Vector3 forwardVector = directionFacing.ToVector();

                var dotProd = directionOfDamage.X * forwardVector.X + directionOfDamage.Y * forwardVector.Y;

                var angleDegrees = Math.Acos(dotProd) * (180 / Math.PI);

                toReturn = baseDamage;

                if (angleDegrees < ShieldHalfAngle)
                {
                    toReturn /= 2;
                }
            }

            toReturn = toReturn * weaponLevelModifier;

            return toReturn;
            
        }

        private void SetForcedTarget(Player owner)
        {
            forcedTarget = owner;
            timeForcedTargetSet = FlatRedBall.Screens.ScreenManager.CurrentScreen.PauseAdjustedCurrentTime;
        }

        public void ReactToPlayerDeath(PositionedObject killedPlayer)
        {
            if(target == killedPlayer)
            {
                // Null out the target if the player is destroyed.
                target = null;
            }
        }

        public void TakeLightningDamage(float dps, Player owner)
        {
            if(canTakeDamage)
            {
                //For the lightning weapon use the player as the position to calculate rear hits.
                float modifiedDps = ModifyDamageToTake(owner, owner.WeaponDamageModifier, dps);
                CurrentHP -= modifiedDps * TimeManager.SecondDifference;

                if(CurrentHP < 0)
                {
                    playerThatDealtKillingBlow = owner;
                    PerformDeath();
                }
                else
                {
                    // todo - flash blue
                    SetForcedTarget(owner);

                    isTakingLightningDamage = true;
                }
            }
        }

        private void PerformDeath()
        {
            if (ExplodesOnDeath)
            {
                PerformExplode();
            }
            else
            {
                switch (FlatRedBallServices.Random.Next(0, 4))
                {
                    case 0: FlatRedBall.Audio.AudioManager.Play(EnemyDeath1); break;
                    case 1: FlatRedBall.Audio.AudioManager.Play(EnemyDeath2); break;
                    case 2: FlatRedBall.Audio.AudioManager.Play(EnemyDeath3); break;
                }

                var death = SpriteManager.AddParticleSprite(Death_1_SpriteSheet);
                death.AnimationChains = Death_1;
                death.CurrentChainName = DeathChainName;
                death.Position = SpriteInstance.Position;
                death.TextureScale = 1;
                death.Animate = true;
                SpriteManager.RemoveSpriteAtTime(death, death.CurrentChain.TotalLength);
                Destroy();
            }

            PerformWeaponDrop();
        }

        private void PerformWeaponDrop()
        {
            if(FlatRedBallServices.Random.NextDouble() <= ChanceForWeaponDrop)
            {
                Factories.WeaponDropFactory.CreateNew(X, Y);
            }
        }

        public void PerformExplode()
        {
            var bulletExplosion = Factories.BulletExplosionFactory.CreateNew(X, Y);
            bulletExplosion.DamageToDeal = this.AoeDamage;
            bulletExplosion.Owner = playerThatDealtKillingBlow;
            bulletExplosion.CircleInstance.Radius = this.AoeRadius;
            bulletExplosion.TeamIndex = playerThatDealtKillingBlow != null ? 3 : 1;
            bulletExplosion.Call(bulletExplosion.Destroy).After(0); // next frame

            ExplodeVfx();

            Destroy();

            bulletExplosion.ForceUpdateDependenciesDeep();
        }

        public void ExplodeVfx()
        {
            var explode = SpriteManager.AddParticleSprite(misc_sprites_SpriteSheet);
            explode.AnimationChains = misc_sprites;
            explode.CurrentChainName = nameof(BigExplosion);
            explode.Position = SpriteInstance.Position;
            explode.TextureScale = 1;
            explode.Animate = true;
            SpriteManager.RemoveSpriteAtTime(explode, explode.CurrentChain.TotalLength);
        }


        private string GetChainName(PrimaryActions primaryAction, SecondaryActions secondaryActions)
        {
            if (aimingVector.X != 0 || aimingVector.Y != 0)
            {
                var direction = TopDownDirectionExtensions.FromDirection(new Vector2(aimingVector.X, aimingVector.Y), PossibleDirections.EightWay);


                return ChainNameHelperMethods.GenerateChainName(primaryAction, null, direction);
            }
            else
            {
                return ChainNameHelperMethods.GenerateChainName(primaryAction, null, TopDownDirection.Right);
            }
        }

        #endregion

        private void CustomDestroy()
		{
            if (DebuggingVariables.ShowEnemyAiShapes)
            {
                ShapeManager.Remove(thisToPerpendicularTarget);
            }

        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
