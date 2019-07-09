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
        public Player Owner { get; set; }


        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
		private void CustomInitialize()
		{


		}

		private void CustomActivity()
		{


		}

        public void SetAnimationChainFromVelocity(TopDownDirection aimingDirection)
        {
            SpriteInstance.CurrentChainName = ChainNameHelperMethods.GenerateChainName(PrimaryActions.Fireball, aimingDirection);
        }

        public void SpawnVFX()
        {
            var explosion = SpriteManager.AddParticleSprite(misc_sprites_SpriteSheet);
            explosion.AnimationChains = misc_sprites;
            explosion.CurrentChainName = nameof(PrimaryActions.ProjectileExplosion);
            explosion.Position = SpriteInstance.Position;
            explosion.TextureScale = 1;
            explosion.Animate = true;

            SpriteManager.RemoveSpriteAtTime(explosion, explosion.CurrentChain.TotalLength);
        }

		private void CustomDestroy()
		{
            

		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
