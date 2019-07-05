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
using Teotihuacan.TopDown;
using System.Linq;
using FlatRedBall.TileCollisions;

namespace Teotihuacan.Entities
{
    public enum Behavior
    {
        Chasing,
        Shooting
    }

	public partial class Enemy : ITopDownEntity
	{
        Polygon pathFindingPolygon;

        Behavior CurrentBehavior;

        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
		private void CustomInitialize()
		{
            if(pathFindingPolygon == null)
            {
                pathFindingPolygon = Polygon.CreateRectangle(7, 7);
            }

		}

		private void CustomActivity()
		{


		}

        public void DoAiActivity(bool refreshPath, NodeNetwork nodeNetwork, 
            PositionedObject target, TileShapeCollection solidCollisions)
        {
            if (CurrentBehavior == Behavior.Chasing)
            {
                DoChasingBehavior(nodeNetwork, target, solidCollisions);

            }
        }

        private void DoChasingBehavior(NodeNetwork nodeNetwork, PositionedObject target, TileShapeCollection solidCollisions)
        {
            var ai = InputDevice as TopDown.TopDownAiInput<Enemy>;
            var path = nodeNetwork.GetPathOrClosest(ref Position, ref target.Position);
            ai.Path.Clear();
            var points = path.Select(item => item.Position).ToList();

            while (points.Count > 0)
            {
                var length = (points[0] - Position).Length();
                pathFindingPolygon.SetPoint(0, length / 2.0f, CircleInstance.Radius);
                pathFindingPolygon.SetPoint(1, length / 2.0f, -CircleInstance.Radius);
                pathFindingPolygon.SetPoint(2, -length / 2.0f, -CircleInstance.Radius);
                pathFindingPolygon.SetPoint(3, -length / 2.0f, CircleInstance.Radius);
                pathFindingPolygon.SetPoint(4, length / 2.0f, CircleInstance.Radius);

                pathFindingPolygon.X = (points[0].X + Position.X) / 2.0f;
                pathFindingPolygon.Y = (points[0].Y + Position.Y) / 2.0f;

                var angle = (float)System.Math.Atan2(points[0].Y - Position.Y, points[0].X - Position.X);
                pathFindingPolygon.RotationZ = angle;

                var hasClearPath = !solidCollisions.CollideAgainst(pathFindingPolygon);

                if (hasClearPath && points.Count > 1)
                {
                    points.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }


            ai.Path.AddRange(points);
            ai.Target = ai.Path.FirstOrDefault();
        }

        private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
