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
        public Vector2 LineEndpoint;

        Vector2? lastLineCollisionPoint;
        Vector2? lastEnemyLineCollisionPoint;

        public void StartCollisionFrameLogic()
        {
            lastLineCollisionPoint = null;
            lastEnemyLineCollisionPoint = null;
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
        }

        public void EndCollisionFrameLogic(Player player)
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
                }
            }

            player.UpdateLightningSprites();
        }

    }
}
