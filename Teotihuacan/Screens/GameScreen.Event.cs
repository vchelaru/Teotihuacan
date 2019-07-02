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
                // it's an enemy bullet
                bullet.Destroy();

                //player.TakeDamage();
            }
        }
        void OnBulletVsEnemyCollisionOccurred (Bullet bullet, Enemy enemy) 
        {
            if(bullet.TeamIndex == 0)
            {
                // it's a player bullet
                bullet.Destroy();

                enemy.Destroy();
            }
        }
        void OnBulletVsSolidCollisionOccurred (Bullet bullet, TileShapeCollection solid) 
        {
            bullet.Destroy();
        }

    }
}
