using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace poolio_balls
{
    public struct LineSegment
    {
        public Vector2 Point1, Point2;

        public LineSegment(Vector2 point1, Vector2 point2)
        {
            Point1 = point1;
            Point2 = point2;
        }
    }
}
