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
using FlatRedBall.Math;
using System.Linq;

namespace Teotihuacan.Entities
{
	public partial class CameraController
	{
        public List<PositionedObject> Targets { get; private set; } = new List<PositionedObject>();

        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
		private void CustomInitialize()
		{
            Camera.Main.RelativeX = 0;
            Camera.Main.RelativeY = 0;

        }

		private void CustomActivity()
		{
            if (Targets.Count > 0)
            {
                var minX = Targets.Min(item => item.X);
                var maxX = Targets.Max(item => item.X);

                var minY = Targets.Min(item => item.Y);
                var maxY = Targets.Max(item => item.Y);



                var targetX = (minX + maxX) / 2.0f;
                var targetY = (minY + maxY) / 2.0f;

                this.XVelocity = targetX - this.X;
                this.YVelocity = targetY - this.Y;
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
