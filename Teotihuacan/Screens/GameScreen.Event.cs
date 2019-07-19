using System;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Specialized;
using FlatRedBall.Audio;
using FlatRedBall.Screens;
using Teotihuacan.Entities;
using Teotihuacan.Screens;
using FlatRedBall.TileCollisions;
using Microsoft.Xna.Framework;

namespace Teotihuacan.Screens
{
    public partial class GameScreen
    {
        void OnBulletVsPlayerCollisionOccurred (Bullet bullet, Player player) 
        {
            if(bullet.TeamIndex == 1)
            {
                if (player.TakeDamage(bullet.DamageToDeal))
                {
                    bullet.Destroy();
                }
            }
        }
        void OnBulletVsEnemyCollisionOccurred (Bullet bullet, Enemy enemy) 
        {
            if(bullet.TeamIndex == 0)
            {
                if(enemy.TakeDamage(bullet.DamageToDeal, bullet.Owner))
                {
                    bullet.PlayerDestroyVfx();

                    bullet.TryExplode();
                    bullet.Destroy();
                }
            }
        }
        void OnBulletVsSolidCollisionOccurred (Bullet bullet, TileShapeCollection solid) 
        {
            bullet.PlayerDestroyVfx();
            bullet.TryExplode();

            bullet.Destroy();
        }
        void OnBulletVsPlayerBaseSolidCollisionOccurred (Entities.Bullet bullet, Entities.PlayerBase playerBase) 
        {
            if(bullet.TeamIndex == 1)
            {

                playerBase.TakeDamage(bullet.DamageToDeal);
                bullet.PlayerDestroyVfx();
                bullet.TryExplode();

                bullet.Destroy();
            }
        }
        void OnPlayerVsPlayerBaseHealingCollisionOccurred (Entities.Player player, Entities.PlayerBase playerBase) 
        {
            player.Heal(playerBase.HealingRatePerSecond);
        }
        void OnEnemyVsBulletExplosionCollisionCollisionOccurred (Entities.Enemy enemy, Entities.BulletExplosion bulletExplosion) 
        {
            enemy.TakeDamage(bulletExplosion.DamageToDeal, bulletExplosion.Owner);
            // don't destroy the explosion, it's aoe and may hit multiple enemies.

            var direction = Vector3.Right;

            if(bulletExplosion.X != enemy.X || bulletExplosion.Y != enemy.Y)
            {
                var enemyToBullet = enemy.Position - bulletExplosion.Position;
                enemyToBullet.Normalize();

                enemy.Velocity += enemyToBullet * bulletExplosion.ExplosionForce;
            }

        }

    }
}
