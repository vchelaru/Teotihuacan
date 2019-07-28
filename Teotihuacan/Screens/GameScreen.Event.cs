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
using Teotihuacan.GumRuntimes;

namespace Teotihuacan.Screens
{
    public partial class GameScreen
    {
        void OnBulletVsPlayerCollisionOccurred (Bullet bullet, Player player) 
        {
            if(bullet.TeamIndex == 1)
            {
                if (player.TakeDamage(bullet.DamageToDeal * CurrentMultipliers.EffectiveDamageMultiplier))
                {
                    bullet.Destroy();
                }
            }
        }
        void OnBulletVsEnemyCollisionOccurred (Bullet bullet, Enemy enemy) 
        {
            if(bullet.TeamIndex == 0)
            {
                if(enemy.TakeDamage(bullet, bullet.DamageToDeal, bullet.Owner))
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
            if (bulletExplosion.TeamIndex != 1)
            {
                enemy.TakeDamage(bulletExplosion, bulletExplosion.DamageToDeal, bulletExplosion.Owner);
                // don't destroy the explosion, it's aoe and may hit multiple enemies.

                var direction = Vector3.Right;

                if (bulletExplosion.X != enemy.X || bulletExplosion.Y != enemy.Y)
                {
                    var enemyToBullet = enemy.Position - bulletExplosion.Position;
                    enemyToBullet.Normalize();

                    enemy.Velocity += enemyToBullet * bulletExplosion.ExplosionForce;
                }
            }
        }
        void OnPlayerVsEnemyRelationshipCollisionOccurred (Entities.Player player, Entities.Enemy enemy) 
        {
            if(enemy.CurrentTarget == player && enemy.ExplodesOnDeath)
            {
                enemy.PerformExplode();
            }
        }
        void OnEnemyVsPlayerBaseSolidCollisionCollisionOccurred (Entities.Enemy enemy, Entities.PlayerBase playerBase) 
        {
            if (enemy.CurrentTarget == playerBase && enemy.ExplodesOnDeath)
            {
                enemy.PerformExplode();
            }
        }

        void OnPlayerBaseVsBulletExplosionCollisionCollisionOccurred (Entities.PlayerBase playerBase, Entities.BulletExplosion bulletExplosion) 
        {
            if (bulletExplosion.TeamIndex != 0)
            {
                playerBase.TakeDamage(bulletExplosion.DamageToDeal);
            }
        }
        void OnPlayerVsBulletExplosionCollisionCollisionOccurred (Entities.Player player, Entities.BulletExplosion bulletExplosion) 
        {
            if (bulletExplosion.TeamIndex != 0)
            {
                player.TakeDamage(bulletExplosion.DamageToDeal * CurrentMultipliers.EffectiveDamageMultiplier);

                var direction = Vector3.Right;

                if (bulletExplosion.X != player.X || bulletExplosion.Y != player.Y)
                {
                    var enemyToBullet = player.Position - bulletExplosion.Position;
                    enemyToBullet.Normalize();

                    player.Velocity += enemyToBullet * bulletExplosion.ExplosionForce;
                }
            }
        }           

        void OnPlayerVsWeaponCollisionCollisionOccurred (Entities.Player player, Entities.WeaponDrop weaponDrop) 
        {
            if (player.InputDevice.DefaultPrimaryActionInput.WasJustPressed)
            {
                player.ConsumeWeaponDrop(weaponDrop.WeaponType);
                weaponDrop.Destroy();

                ((GameScreenGumRuntime)GameScreenGum).RefreshExperienceBar(player, UpdateType.Interpolate);
            }
        }
        void OnPlayerVsFirePitCollisionCollisionOccurred (Entities.Player player, FlatRedBall.TileCollisions.TileShapeCollection second) 
        {
            player.TakeDamage(FirePitDps * TimeManager.SecondDifference);
        }

        void OnEnemyVsFirePitCollisionCollisionOccurred (Entities.Enemy enemy, FlatRedBall.TileCollisions.TileShapeCollection second) 
        {
            enemy.TakeNonLethalDamage(FirePitDps * TimeManager.SecondDifference);

        }

    }
}
