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

namespace Teotihuacan.Entities
{
    #region Enums

    public enum Behavior
    {
        Chasing,
        Shooting
    }

    #endregion

    public partial class Enemy : ITopDownEntity
	{
        #region Fields/Properties

        Polygon pathFindingPolygon;

        Behavior CurrentBehavior;

        double lastFireShotTime;

        int damageTaken;

        bool canTakeDamage => damageTaken < CurrentDataCategoryState?.MaxHp;
        #endregion

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
		}

		private void CustomActivity()
		{
            SetMovementValues();

		}

        private void SetMovementValues()
        {

        }

        public void DoAiActivity(bool refreshPath, NodeNetwork nodeNetwork, 
            PositionedObject target, TileShapeCollection solidCollisions)
        {
            if (CurrentDataCategoryState.MaxHp > damageTaken)
            {
                if (refreshPath)
                {
                    // enemies always move towards player, but really slowly when shooting
                    RefreshPath(nodeNetwork, target, solidCollisions);
                }

                UpdateCurrentBehavior(nodeNetwork, target);

                UpdateMovementBehavior();

                DoShootingActivity(target);
            }
        }

        private void UpdateMovementBehavior()
        {
            if (CurrentBehavior == Behavior.Shooting)
            {
                mCurrentMovement = TopDownValues[DataTypes.TopDownValues.WhileShooting];
            }
            else
            {
                mCurrentMovement = TopDownValues[DataTypes.TopDownValues.DefaultValues];
            }
        }

        private void UpdateCurrentBehavior(NodeNetwork nodeNetwork, PositionedObject target)
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
            if (
                CurrentBehavior == Behavior.Shooting &&
                FlatRedBall.Screens.ScreenManager.CurrentScreen.PauseAdjustedSecondsSince(lastFireShotTime) >
                1 / FireShotsPerSecond
                )
            {
                var direction = Vector3.Right;
                if(X - target.X != 0 || Y - target.Y != 0)
                {
                    direction = target.Position - this.Position;
                    direction.Normalize();
                }

                var bullet = Factories.BulletFactory.CreateNew(this.X, this.Y);
                bullet.Velocity = bullet.BulletSpeed * direction;
                bullet.TeamIndex = 1; // be explicit about it. Player team is 0.

                // For rick - animations on enemies here
                //shootingLayer.PlayOnce(GetChainName(currentPrimaryAction, SecondaryActions.ShootingFire));

                lastFireShotTime = FlatRedBall.Screens.ScreenManager.CurrentScreen.PauseAdjustedCurrentTime;
            }
        }

        public bool TakeDamage(int damageToTake)
        {
            bool tookDamage = false;
            if(canTakeDamage)
            {
                tookDamage = true;
                damageTaken += damageToTake;

                if(damageTaken >= CurrentDataCategoryState.MaxHp)
                {
                    PerformDeath();
                }
            }

            return tookDamage;
        }

        private void PerformDeath()
        {
            Velocity = Vector3.Zero;
            //ToDo: Perform death animations
            Destroy();
        }

        private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
