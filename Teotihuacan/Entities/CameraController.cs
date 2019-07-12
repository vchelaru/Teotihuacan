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
        float orthoZoomVelocity = 0;
        float zoomOutBounds = 30;
        float zoomInBounds = 70;
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

            float desiredWidth = maxXThisFrame - minXThisFrame + zoomOutBounds;
            float desiredHeight = maxYThisFrame - minYThisFrame + +zoomOutBounds;

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

                //Commenting out for now.
                //if (Map != null)
                //{
                //    targetX = Math.Max(Map.X + Camera.Main.OrthogonalWidth / 2.0f, targetX);
                //    targetX = Math.Min(Map.X + Map.Width - Camera.Main.OrthogonalWidth / 2.0f, targetX);

                //    targetY = Math.Max(Map.Y + Camera.Main.OrthogonalHeight / 2.0f, targetY);
                //    targetY = Math.Min(Map.Y - Camera.Main.OrthogonalHeight / 2.0f, targetY);

                //    X = Map.X + Map.Width / 2.0f;
                //    Y = Map.Y - Map.Height / 2.0f;
                //}

                this.XVelocity = targetX - this.X;
                this.YVelocity = targetY - this.Y;


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

        public override void UpdateDependencies(double currentTime)
        {
            base.UpdateDependencies(currentTime);
            CalculateZoomFromBounds();
            //CalculateZoomFromDistance();

            SetOrthoZoom();
        }

        private void CalculateZoomFromDistance()
        {
            float currentWidth = maxXThisFrame - minXThisFrame;
            float currentHeight = maxYThisFrame - minYThisFrame;

            float widthDiff = (Camera.Main.OrthogonalWidth - zoomOutBounds) - currentWidth;
            float heightDiff = (Camera.Main.OrthogonalHeight - zoomOutBounds) - currentHeight;

            orthoZoomVelocity = Math.Min(0, widthDiff);
            orthoZoomVelocity = Math.Min(orthoZoomVelocity, heightDiff);

            orthoZoomVelocity = Math.Abs(orthoZoomVelocity);
            if(orthoZoomVelocity > 0)
            {
                orthoZoom += orthoZoomVelocity * TimeManager.SecondDifference;
            }
            else if(orthoZoom > 1)
            {
                widthDiff = (Camera.Main.OrthogonalWidth - zoomInBounds) - currentWidth;
                heightDiff = (Camera.Main.OrthogonalHeight - zoomInBounds) - currentHeight;

                bool canZoomIn = widthDiff > 0 && heightDiff > 0;

                if(canZoomIn)
                {
                    orthoZoomVelocity = Math.Min(widthDiff, heightDiff);
                    orthoZoom -= orthoZoomVelocity * TimeManager.SecondDifference;
                    orthoZoom = 
            }Math.Max(orthoZoom, 1);
                }
        }

        private void CalculateZoomFromBounds()
        {
            float cameraHalfWidth = Camera.Main.OrthogonalWidth * .5f;
            float cameraHalfHeight = Camera.Main.OrthogonalHeight * .5f;

            float leftBound = (X - cameraHalfWidth);
            float rightBound = (X + cameraHalfWidth);
            float bottomBound = (Y - cameraHalfHeight);
            float topBound = (Y + cameraHalfHeight);

            float leftDifference = minXThisFrame - (leftBound + zoomOutBounds);
            float rightDifference = (rightBound - zoomOutBounds) - maxXThisFrame;
            float bottomDifference = minYThisFrame - (bottomBound + zoomOutBounds);
            float topDifference = (topBound - zoomOutBounds) - maxYThisFrame;

            orthoZoomVelocity = Math.Min(0, leftDifference);
            orthoZoomVelocity = Math.Min(orthoZoomVelocity, rightDifference);
            orthoZoomVelocity = Math.Min(orthoZoomVelocity, bottomDifference);
            orthoZoomVelocity = Math.Min(orthoZoomVelocity, topDifference);

            orthoZoomVelocity = Math.Abs(orthoZoomVelocity);
            if (orthoZoomVelocity > 0)
            {
                orthoZoom += orthoZoomVelocity * TimeManager.SecondDifference;
            }
            else if (orthoZoom > 1)
            {
                leftDifference = minXThisFrame - (leftBound + zoomInBounds);
                rightDifference = (rightBound - zoomInBounds) - maxXThisFrame;
                bottomDifference = minYThisFrame - (bottomBound + zoomInBounds);
                topDifference = (topBound - zoomInBounds) - maxYThisFrame;

                bool shouldZoom = leftDifference > 0 &&
                                  rightDifference > 0 &&
                                  bottomDifference > 0 &&
                                  topDifference > 0;

                if (shouldZoom)
                {
                    orthoZoomVelocity = Math.Min(rightDifference, leftDifference);
                    orthoZoomVelocity = Math.Min(orthoZoomVelocity, rightDifference);
                    orthoZoomVelocity = Math.Min(orthoZoomVelocity, bottomDifference);
                    orthoZoomVelocity = Math.Min(orthoZoomVelocity, topDifference);

                    orthoZoomVelocity = Math.Abs(orthoZoomVelocity);
                    if (orthoZoom > 0)
                    {
                        orthoZoom -= orthoZoomVelocity * TimeManager.SecondDifference;
                        orthoZoom = Math.Max(orthoZoom, 1);
                    }
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
