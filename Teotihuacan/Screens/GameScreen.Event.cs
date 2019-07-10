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
                    bullet.Destroy();
                }
            }
        }
        void OnBulletVsSolidCollisionOccurred (Bullet bullet, TileShapeCollection solid) 
        {
            bullet.SpawnVFX();
            bullet.Destroy();
        }
        void OnBulletVsPlayerBaseSolidCollisionOccurred (Entities.Bullet bullet, Entities.PlayerBase playerBase) 
        {
            if(bullet.TeamIndex == 1)
            {

                playerBase.TakeDamage(bullet.DamageToDeal);
                bullet.SpawnVFX();
                bullet.Destroy();
            }
        }
        void OnPlayerVsPlayerBaseHealingCollisionOccurred (Entities.Player bullet, Entities.PlayerBase playerBase) 
        {
            // todo - make the player heal HP
        }

    }
}
