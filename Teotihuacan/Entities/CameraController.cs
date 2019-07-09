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


		}

		private void CustomActivity()
		{
            if (Targets.Count > 0)
            {
                this.XVelocity = Targets[0].X - this.X;
                this.YVelocity = Targets[0].Y - this.Y;
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
