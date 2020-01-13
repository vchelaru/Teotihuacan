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
using Teotihuacan.Animation;

namespace Teotihuacan.Entities
{
	public partial class Bullet
	{
        #region Fields/Properties

        public Player Owner { get; set; }

        #endregion

        #region Initialize

        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
        private void CustomInitialize()
		{


		}

        #endregion

        private void CustomActivity()
		{


		}

        public void SetAnimationChainFromVelocity(TopDownDirection aimingDirection, Weapon weapon)
        {
            string weaponAnimationName = null;
            switch(weapon)
            {
                case Weapon.ShootingFire: weaponAnimationName = "Fireball"; break;
                case Weapon.ShootingSkull: weaponAnimationName = "Skull"; break;
            }
            SpriteInstance.CurrentChainName = weaponAnimationName + "_" + aimingDirection.ToFriendlyString();
            var positionOffset = TopDownDirectionExtensions.ToVector(aimingDirection);

            Position += positionOffset * OffsetRadius;
        }

        public void PlayerDestroyVfx()
        {
            var explosion = SpriteManager.AddParticleSprite(misc_sprites_SpriteSheet);
            explosion.AnimationChains = misc_sprites;

            if(CurrentDataCategoryState == Bullet.DataCategory.EnemyBullet ||
                CurrentDataCategoryState == Bullet.DataCategory.PlayerFire)
            {
                explosion.CurrentChainName = nameof(PrimaryActions.ProjectileExplosion);
            }
            else if(CurrentDataCategoryState == Bullet.DataCategory.PlayerSkull)
            {
                explosion.CurrentChainName = BigExplosionSkull.Name;
            }
            explosion.Position = SpriteInstance.Position;
            explosion.TextureScale = 1;
            explosion.Animate = true;

            PlayImpactSound();

#pragma warning disable CS0618 // Type or member is obsolete - can't do call.after because the bullet will be destroyed so its instructions will be removed
            SpriteManager.RemoveSpriteAtTime(explosion, explosion.CurrentChain.TotalLength);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        public void TryExplode()
        {
            if(AoeRadius > 0)
            {
                var bulletExplosion = Factories.BulletExplosionFactory.CreateNew(this.X, this.Y);
                bulletExplosion.Owner = this.Owner;
                bulletExplosion.DamageToDeal = this.AoeDamage;
                bulletExplosion.CircleInstance.Radius = this.AoeRadius;
                bulletExplosion.TeamIndex = this.TeamIndex;
                bulletExplosion.Call(bulletExplosion.Destroy).After(0); // next frame
                
            }
        }

        private void PlayImpactSound()
        {
            if (CurrentDataCategoryState == Bullet.DataCategory.PlayerFire)
            {
                switch (FlatRedBallServices.Random.Next(0, 4))
                {
                    case 0: FlatRedBall.Audio.AudioManager.Play(BulletFlameHit1); break;
                    case 1: FlatRedBall.Audio.AudioManager.Play(BulletFlameHit2); break;
                    case 2: FlatRedBall.Audio.AudioManager.Play(BulletFlameHit3); break;
                    case 3: FlatRedBall.Audio.AudioManager.Play(BulletFlameHit4); break;
                }
            }
            else if (CurrentDataCategoryState == Bullet.DataCategory.PlayerSkull)
            {
                switch (FlatRedBallServices.Random.Next(0, 4))
                {
                    case 0: FlatRedBall.Audio.AudioManager.Play(BulletSkullHit1); break;
                    case 1: FlatRedBall.Audio.AudioManager.Play(BulletSkullHit2); break;
                    case 2: FlatRedBall.Audio.AudioManager.Play(BulletSkullHit3); break;
                    case 3: FlatRedBall.Audio.AudioManager.Play(BulletSkullHit4); break;
                }
            }
            else if (CurrentDataCategoryState == Bullet.DataCategory.EnemyBullet)
            {
                FlatRedBall.Audio.AudioManager.Play(BulletEnemyShotHit);
            }
        }

		private void CustomDestroy()
		{


        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
