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
using FlatRedBall.TileGraphics;

namespace Teotihuacan.Entities
{
	public partial class CameraController
	{
        public List<PositionedObject> Targets { get; private set; } = new List<PositionedObject>();

        public LayeredTileMap Map { get; set; }

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

                if(Map != null)
                {
                    targetX = Math.Max(Map.X + Camera.Main.OrthogonalWidth/2.0f, targetX);
                    targetX = Math.Min(Map.X + Map.Width - Camera.Main.OrthogonalWidth/2.0f, targetX);

                    targetY = Math.Max(Map.Y + Camera.Main.OrthogonalHeight / 2.0f, targetY);
                    targetY = Math.Min(Map.Y - Camera.Main.OrthogonalHeight / 2.0f, targetY);

                    X = Map.X + Map.Width / 2.0f;
                    Y = Map.Y - Map.Height / 2.0f;
                }

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
