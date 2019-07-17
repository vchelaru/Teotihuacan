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
using Microsoft.Xna.Framework;

namespace Teotihuacan.Entities
{
	public partial class CameraController
	{
        public List<PositionedObject> Targets { get; private set; } = new List<PositionedObject>();

        public LayeredTileMap Map { get; set; }

        float defaultOrthoHeight;
        float defaultOrthoWidth;

        float orthoZoom = 1;

        float minXThisFrame = 0;
        float maxXThisFrame = 0;
        float minYThisFrame = 0;
        float maxYThisFrame = 0;
        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
		private void CustomInitialize()
		{
            Camera.Main.RelativeX = 0;
            Camera.Main.RelativeY = 0;

            defaultOrthoHeight = Camera.Main.OrthogonalHeight;
            defaultOrthoWidth = Camera.Main.OrthogonalWidth;
        }

        public void SetStartPositionAndZoom()
        {
            float targetX, targetY;

            CalculateTargetPosition(out targetX, out targetY);

            this.X = targetX;
            this.Y = targetY;

            float desiredWidth = maxXThisFrame - minXThisFrame + ZoomOutBounds;
            float desiredHeight = maxYThisFrame - minYThisFrame + +ZoomOutBounds;

            float startingZoom = Math.Max(1, desiredWidth / Camera.Main.OrthogonalWidth);
            startingZoom = Math.Max(startingZoom, desiredHeight / Camera.Main.OrthogonalHeight);

            orthoZoom = startingZoom;
            SetOrthoZoom();
        }

		private void CustomActivity()
		{
            if (Targets.Count > 0)
            {
                float targetX, targetY;

                CalculateTargetPosition(out targetX, out targetY);

                this.XVelocity = targetX - this.X;
                this.YVelocity = targetY - this.Y;

                CalculateZoomFromDistance();

                SetOrthoZoom();
            }

        }

        private void CalculateTargetPosition(out float targetX, out float targetY)
        {
            minXThisFrame = Targets.Min(item => item.X);
            maxXThisFrame = Targets.Max(item => item.X);

            minYThisFrame = Targets.Min(item => item.Y);
            maxYThisFrame = Targets.Max(item => item.Y);

            targetX = (minXThisFrame + maxXThisFrame) * .5f;
            targetY = (minYThisFrame + maxYThisFrame) * .5f;
        }

        private void SetOrthoZoom()
        {
            Camera.Main.OrthogonalHeight = defaultOrthoHeight * orthoZoom;
            Camera.Main.OrthogonalWidth = defaultOrthoWidth * orthoZoom;
        }

        private void CalculateZoomFromDistance()
        {
            float currentWidth = maxXThisFrame - minXThisFrame;
            float currentHeight = maxYThisFrame - minYThisFrame;

            float widthDiff = currentWidth / (Camera.Main.OrthogonalWidth - ZoomOutBounds);
            float heightDiff = currentHeight / (Camera.Main.OrthogonalHeight - ZoomOutBounds);

            float orthoZoomIncrement = Math.Max(0, widthDiff - 1);
            orthoZoomIncrement = Math.Max(orthoZoomIncrement, heightDiff - 1);

            if (orthoZoomIncrement > 0)
            {
                orthoZoom += orthoZoomIncrement;
            }
            else if (orthoZoom > 1)
            {
                widthDiff = (Camera.Main.OrthogonalWidth - ZoomInBounds) / currentWidth;
                heightDiff = (Camera.Main.OrthogonalHeight - ZoomInBounds) / currentHeight;

                bool canZoomIn = widthDiff > 1 && heightDiff > 1;
                
                if (canZoomIn)
                {
                    orthoZoomIncrement = Math.Min(widthDiff - 1, heightDiff - 1);
                    orthoZoom -= orthoZoomIncrement;
                    orthoZoom = Math.Max(orthoZoom, 1);
                }
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
