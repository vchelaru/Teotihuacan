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

        public void DoAiActivity(bool refreshPath, NodeNetwork nodeNetwork, 
            PositionedObjectList<Player> players, PlayerBase playerBase, TileShapeCollection solidCollisions, TileShapeCollection pitCollision)
        {
            

            if (CurrentHP > 0)
            {
                RefreshVisualsFromDamageState();

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

                DoShootingActivity(target);

            }

            spriteAnimationController.Activity();
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
            if (X - target.X != 0 || Y - target.Y != 0)
            {
                aimingVector = new Vector3(target.X, target.Y, 0) - new Vector3(X, Y, 0);
                aimingVector.Normalize();
            }
        }

        private void UpdateCurrentMovementValues()
        {
            if (CurrentBehavior == Behavior.Shooting)
            {
                mCurrentMovement = TopDownValues[DataTypes.TopDownValues.WhileShooting];
            }
            else if(CurrentBehavior == Behavior.Reloading)
            {
                mCurrentMovement = TopDownValues[DataTypes.TopDownValues.WhileReloading];
            }
            else 
            {
                mCurrentMovement = TopDownValues[DataTypes.TopDownValues.DefaultValues];
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
                shootingAnimationLayer.StopPlay();
            }
            else
            {
                var ai = InputDevice as TopDown.TopDownAiInput<Enemy>;
                var path = ai.Path;

                bool isLineOfSight = path.Count < 2 && target != null;

                bool isInRange = false;

                if(isLineOfSight)
                {
                    isInRange = (target.Position - this.Position).Length() < MaxShootingDistance;
                }

                var shouldWalkForward = true;
                
                if(isInRange && CurrentDataCategoryState != DataCategory.Suicider)
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
                lineOfSightPathFindingPolygon.SetPoint(0, length / 2.0f, CircleInstance.Radius);
                lineOfSightPathFindingPolygon.SetPoint(1, length / 2.0f, -CircleInstance.Radius);
                lineOfSightPathFindingPolygon.SetPoint(2, -length / 2.0f, -CircleInstance.Radius);
                lineOfSightPathFindingPolygon.SetPoint(3, -length / 2.0f, CircleInstance.Radius);
                lineOfSightPathFindingPolygon.SetPoint(4, length / 2.0f, CircleInstance.Radius);

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
                1 / FireShotsPerSecond
                )
            {
                var bullet = Factories.BulletFactory.CreateNew(this.X, this.Y);
                bullet.Z = this.Z - 1;
                bullet.CurrentDataCategoryState = Bullet.DataCategory.EnemyBullet;
                bullet.Velocity = bullet.BulletSpeed * aimingVector;
                bullet.SetAnimationChainFromVelocity(TopDownDirectionExtensions.FromDirection(aimingVector, PossibleDirections), Weapon.ShootingFire);
                shootingAnimationLayer.PlayOnce(GetChainName(PrimaryActions.shoot, SecondaryActions.Shooting ));

                lastFireShotTime = FlatRedBall.Screens.ScreenManager.CurrentScreen.PauseAdjustedCurrentTime;

                shotsLeftInClip--;
            }
        }

        public bool TakeDamage(int damageToTake, Player owner)
        {
            bool tookDamage = false;
            if(canTakeDamage)
            {
                tookDamage = true;
                CurrentHP -= damageToTake;

                if(CurrentHP <= 0)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"Took {damageToTake} damage and died");
                    playerThatDealtKillingBlow = owner;
                    PerformDeath();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"Took {damageToTake} damage and has {CurrentHP} left");
                    isFlashingWhite = true;
                    // We may need to be more careful here if there's other instructions.
                    this.Instructions.Clear();
                    this.Call(() => isFlashingWhite = false).After(FlashDuration);

                    SetForcedTarget(owner);
                }
            }

            return tookDamage;
        }

        private void SetForcedTarget(Player owner)
        {
            forcedTarget = owner;
            timeForcedTargetSet = FlatRedBall.Screens.ScreenManager.CurrentScreen.PauseAdjustedCurrentTime;
        }

        public void TakeLightningDamage(float dps, Player owner)
        {
            if(canTakeDamage)
            {
                CurrentHP -= dps * TimeManager.SecondDifference;

                if(CurrentHP < 0)
                {
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
            if (CurrentDataCategoryState == DataCategory.Suicider)
            {
                PerformExplode();
            }
            else
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
        }

        public void PerformExplode()
        {
            if (CurrentDataCategoryState == DataCategory.Suicider)
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
