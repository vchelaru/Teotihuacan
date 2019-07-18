using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teotihuacan.Entities;

namespace Teotihuacan.Managers
{
    public class LightningWeaponManager
    {
        #region Fields/Properties

        public Vector2 LineEndpoint;

        Vector2? lastLineCollisionPoint;
        Vector2? lastEnemyLineCollisionPoint;

        /// <summary>
        /// The closest enemy reported through collision events, but this enemy
        /// may not take damage if a wall was closer.
        /// </summary>
        Enemy closestEnemyThisFrame;

        /// <summary>
        /// The clsoest enemy actually hit this frame - meaning it was the closest
        /// in the collision event and no wall was closer.
        /// </summary>
        public Enemy EnemyHitThisFrame { get; private set;}

        #endregion

        public void StartCollisionFrameLogic()
        {
            lastLineCollisionPoint = null;
            lastEnemyLineCollisionPoint = null;
            closestEnemyThisFrame = null;
            EnemyHitThisFrame = null;
        }

        public void HandleCollisionVsSolid(Player player)
        {
            lastLineCollisionPoint = new Vector2(
                (float)player.LightningCollisionLine.LastCollisionPoint.X,
                (float)player.LightningCollisionLine.LastCollisionPoint.Y);

        }

        public void HandleCollisionVsEnemy(Enemy enemy)
        {
            lastEnemyLineCollisionPoint = new Vector2(enemy.Position.X, enemy.Position.Y);
            closestEnemyThisFrame = enemy;
        }

        public void EndCollisionFrameLogic(Player player)
        {
            if(player.CurrentSecondaryAction == Animation.SecondaryActions.Shooting &&
                player.EquippedWeapon == Animation.Weapon.ShootingLightning)
            {
                if (lastLineCollisionPoint == null && lastEnemyLineCollisionPoint == null)
                {
                    // collided with nothing, endpoint is endpoint of line
                    LineEndpoint = new Vector2(
                        (float)player.LightningCollisionLine.AbsolutePoint2.X,
                        (float)player.LightningCollisionLine.AbsolutePoint2.Y
                    );
                }
                else if (lastEnemyLineCollisionPoint != null && lastLineCollisionPoint == null)
                {
                    LineEndpoint = lastEnemyLineCollisionPoint.Value;
                    EnemyHitThisFrame = closestEnemyThisFrame;
                }
                else if (lastLineCollisionPoint != null && lastEnemyLineCollisionPoint == null)
                {
                    LineEndpoint = lastLineCollisionPoint.Value;
                }
                else
                {
                    var playerPos = new Vector2(player.X, player.Y);

                    var distanceToSolid = (lastLineCollisionPoint.Value - playerPos).LengthSquared();
                    var distanceToEnemy = (lastEnemyLineCollisionPoint.Value - playerPos).LengthSquared();

                    if (distanceToSolid < distanceToEnemy)
                    {
                        LineEndpoint = lastLineCollisionPoint.Value;
                    }
                    else
                    {
                        LineEndpoint = lastEnemyLineCollisionPoint.Value;
                        EnemyHitThisFrame = closestEnemyThisFrame;
                    }
                }
                player.UpdateLightningSprites();
            }
            else
            {
                player.ClearLightningSprites();
            }
        }

    }
}
