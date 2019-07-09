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

        Polygon pathFindingPolygon;

        Behavior CurrentBehavior;

        double lastFireShotTime;

        int shotsLeftInClip;

        public int CurrentHP { get; private set; }

        Vector3 aimingVector = Vector3.Right;

        bool canTakeDamage => CurrentHP > 0;

        AnimationController spriteAnimationController;
        AnimationLayer shootingAnimationLayer;

        PrimaryActions currentPrimaryAction;

        PositionedObject target;

        PositionedObject forcedTarget;
        double timeForcedTargetSet;
        #endregion

        #region Initialize

        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
        private void CustomInitialize()
		{
            if(pathFindingPolygon == null)
            {
                pathFindingPolygon = Polygon.CreateRectangle(7, 7);
            }


            shotsLeftInClip = ClipSize;
            InitializeAnimations();
		}

        private void InitializeAnimations()
        {
            spriteAnimationController = new AnimationController(SpriteInstance);

            AnimationLayer walkingLayer = new AnimationLayer();
            walkingLayer.EveryFrameAction = () =>
            {
                return GetChainName(currentPrimaryAction);
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
            PositionedObjectList<Player> players, TileShapeCollection solidCollisions)
        {
            if (CurrentHP > 0)
            {
                DoTargetDecision(players);

                if (refreshPath)
                {
                    // enemies always move towards player, but really slowly when shooting
                    RefreshPath(nodeNetwork, target, solidCollisions);
                }

                UpdateAimingBehavior(target);

                UpdatePrimaryAction();

                UpdateCurrentBehavior(nodeNetwork, target);

                UpdateCurrentMovementValues();

                DoShootingActivity(target);

            }

            spriteAnimationController.Activity();
        }

        private void DoTargetDecision(PositionedObjectList<Player> players)
        {

            if(forcedTarget != null)
            {
                target = forcedTarget;

                var timeSinceSet =
                    FlatRedBall.Screens.ScreenManager.CurrentScreen.PauseAdjustedSecondsSince(timeForcedTargetSet);

                if(timeSinceSet > 8)
                {
                    forcedTarget = null;
                }
            }
            else if(target == null)
            {
                PositionedObject closest = GetClosest(players);

                target = closest;
            }
            else
            {
                if(CurrentBehavior == Behavior.Chasing)
                {
                    // see if there is anything closer
                    target = GetClosest(players);
                }
                // else if shooting/reloading, chase that same target so long as in chasing mode

            }
        }

        private PositionedObject GetClosest(PositionedObjectList<Player> players)
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

                if(isInRange)
                {
                    // line of site
                    CurrentBehavior = Behavior.Shooting;
                }
                else
                {
                    CurrentBehavior = Behavior.Chasing;
                }
            }
        }

        private void RefreshPath(NodeNetwork nodeNetwork, PositionedObject target, TileShapeCollection solidCollisions)
        {
            var ai = InputDevice as TopDown.TopDownAiInput<Enemy>;
            var path = nodeNetwork.GetPathOrClosest(ref Position, ref target.Position);
            ai.Path.Clear();
            var points = path.Select(item => item.Position).ToList();

            while (points.Count > 0)
            {
                var length = (points[0] - Position).Length();
                pathFindingPolygon.SetPoint(0, length / 2.0f, CircleInstance.Radius);
                pathFindingPolygon.SetPoint(1, length / 2.0f, -CircleInstance.Radius);
                pathFindingPolygon.SetPoint(2, -length / 2.0f, -CircleInstance.Radius);
                pathFindingPolygon.SetPoint(3, -length / 2.0f, CircleInstance.Radius);
                pathFindingPolygon.SetPoint(4, length / 2.0f, CircleInstance.Radius);

                pathFindingPolygon.X = (points[0].X + Position.X) / 2.0f;
                pathFindingPolygon.Y = (points[0].Y + Position.Y) / 2.0f;

                var angle = (float)System.Math.Atan2(points[0].Y - Position.Y, points[0].X - Position.X);
                pathFindingPolygon.RotationZ = angle;

                var hasClearPath = !solidCollisions.CollideAgainst(pathFindingPolygon);

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
                bullet.SetAnimationChainFromVelocity(TopDownDirectionExtensions.FromDirection(aimingVector, PossibleDirections));
                shootingAnimationLayer.PlayOnce(GetChainName(PrimaryActions.shoot));

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
                    PerformDeath();
                }
                else
                {
                    FlashWhite();

                    forcedTarget = owner;
                    timeForcedTargetSet = FlatRedBall.Screens.ScreenManager.CurrentScreen.PauseAdjustedCurrentTime;
                }
            }

            return tookDamage;
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


        private string GetChainName(PrimaryActions primaryAction, SecondaryActions secondaryAction = SecondaryActions.None)
        {
            if (aimingVector.X != 0 || aimingVector.Y != 0)
            {
                var direction = TopDownDirectionExtensions.FromDirection(new Vector2(aimingVector.X, aimingVector.Y), PossibleDirections.EightWay);

                return ChainNameHelperMethods.GenerateChainName(primaryAction, secondaryAction, direction);
            }
            else
            {
                return ChainNameHelperMethods.GenerateChainName(primaryAction, secondaryAction, TopDownDirection.Right);
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
