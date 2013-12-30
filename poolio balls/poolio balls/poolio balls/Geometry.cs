using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace poolio_balls
{
    public class Geometry
    {
        public static LineSegment PointLinePerpendicular(Vector2 point, LineSegment line)
        {
            // vector of line
            Vector2 a = line.Point2 - line.Point1;

            // vector of point from initial point on line
            Vector2 b = point - line.Point1;

            // scalar projection of b onto a
            float compBOntoA = Vector2.Dot(a, b) / a.Length();

            // unit vector of a
            a = a / a.Length();

            // vector projection of b onto a
            Vector2 projBontoA = a * compBOntoA;

            // vector projection plus initial point of edge
            Vector2 pointOnEdge = line.Point1 + projBontoA;

            // return line from point on line to given point
            return new LineSegment(pointOnEdge, point);
        }

        // maybe not working
        /*public static bool LineSegmentIntersect(LineSegment line1, LineSegment line2)
        {
            float v1x1 = line1.Point1.X;
            float v1x2 = line1.Point2.X;
            float v1y1 = line1.Point1.Y;
            float v1y2 = line1.Point2.Y;

            float v2x1 = line2.Point1.X;
            float v2x2 = line2.Point2.X;
            float v2y1 = line2.Point1.Y;
            float v2y2 = line2.Point2.Y;

            float d1, d2;
            float a1, a2, b1, b2, c1, c2;

            // Convert vector 1 to a line (line 1) of infinite length.
            // We want the line in linear equation standard form: A*x + B*y + C = 0
            // See: http://en.wikipedia.org/wiki/Linear_equation
            a1 = v1y2 - v1y1;
            //a1 = line1.Point2.Y - line1.Point1.Y;
            b1 = v1x1 - v1x2;
            //b1 = line1.Point1.X - line1.Point2.X;
            c1 = (v1x2 * v1y1) - (v1x1 * v1y2);
            //c1 = (line1.Point2.X * line1.Point1.Y) - (line1.Point1.X * line1.Point2.Y);

            // Every point (x,y), that solves the equation above, is on the line,
            // every point that does not solve it, is either above or below the line.
            // We insert (x1,y1) and (x2,y2) of vector 2 into the equation above.
            d1 = (a1 * v2x1) + (b1 * v2y1) + c1;
            //d1 = (a1 * line2.Point1.X) + (b1 * line2.Point1.Y) + c1;
            d2 = (a1 * v2x2) + (b1 * v2y2) + c1;
            //d2 = (a1 * line2.Point2.X) + (b1 * line2.Point2.Y) + c1;

            // If d1 and d2 both have the same sign, they are both on the same side of
            // our line 1 and in that case no intersection is possible. Careful, 0 is
            // a special case, that's why we don't test ">=" and "<=", but "<" and ">".
            if (d1 > 0 && d2 > 0) return false;
            if (d1 < 0 && d2 < 0) return false;

            // We repeat everything above for vector 2.
            // We start by calculating line 2 in linear equation standard form.
            a2 = v2y2 - v2y1;
            //a2 = line2.Point2.Y - line2.Point1.Y;
            b2 = v2x1 - v2x2;
            //b2 = line2.Point1.X - line2.Point2.X;
            c2 = (v2x2 * v1y1) - (v2x1 * v2y2);
            //c2 = (line2.Point2.X * line1.Point1.Y) - (line2.Point1.X * line2.Point2.Y);

            // Calulate d1 and d2 again, this time using points of vector 1
            d1 = (a2 * v1x1) + (b2 * v1y1) + c2;
            //d1 = (a2 * line1.Point1.X) + (b2 * line1.Point1.Y) + c2;
            d2 = (a2 * v1x2) + (b2 * v1y2) + c2;
            //d2 = (a2 * line1.Point2.X) + (b2 * line1.Point2.Y) + c2;

            // Again, if both have the same sign (and neither one is 0),
            // no intersection is possible.
            if (d1 > 0 && d2 > 0) return false;
            if (d1 < 0 && d2 < 0) return false;

            // If we get here, only three possibilities are left. Either the two
            // vectors intersect in exactly one point or they are collinear
            // (they both lie both on the same infinite line), in which case they
            // may intersect in an infinite number of points or not at all.
            //if ((a1 * b2) - (a2 * b1) == 0.0f) return COLLINEAR;

            // If they are not collinear, they must intersect in exactly one point.
            return true;
        }*/

        // might work
        public static bool LineSegmentIntersect(LineSegment line1, LineSegment line2)
        {
            // Find the four orientations needed for general and
            // special cases
            int o1 = orientation(line1.Point1, line1.Point2, line2.Point1);
            int o2 = orientation(line1.Point1, line1.Point2, line2.Point2);
            int o3 = orientation(line2.Point1, line2.Point2, line1.Point1);
            int o4 = orientation(line2.Point1, line2.Point2, line1.Point2);

            // General case
            if (o1 != o2 && o3 != o4)
                return true;

            return false;
        }

        // To find orientation of ordered triplet (p, q, r).
        // The function returns following values
        // 0 --> p, q and r are colinear
        // 1 --> Clockwise
        // 2 --> Counterclockwise
        static int orientation(Vector2 p, Vector2 q, Vector2 r)
        {
            // See 10th slides from following link for derivation of the formula
            // http://www.dcs.gla.ac.uk/~pat/52233/slides/Geometry1x1.pdf
            float val = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);

            if (val == 0) return 0;  // colinear

            return (val > 0) ? 1 : 2; // clock or counterclock wise
        }

        /// <summary>
        /// I run a semi-infinite ray horizontally (increasing x, fixed y)
        /// out from the test point, and count how many edges it crosses. 
        /// At each crossing, the ray switches between inside and outside. 
        /// This is called the Jordan curve theorem.
        /// </summary>
        /// <param name="numOfVertices">number of vertices</param>
        /// <param name="vertices">array of vertices</param>
        /// <param name="testPoint">point to test</param>
        /// <returns>true if point is in polygon</returns>
        public static bool PointInPolygon(Vector2 testPoint, Vector2[] vertices)
        {
            bool c = false;
            for (int i = 0, j = vertices.Length - 1; i < vertices.Length; j = i++)
            {
                if (((vertices[i].Y > testPoint.Y) != (vertices[j].Y > testPoint.Y)) &&
                 (testPoint.X < (vertices[j].X - vertices[i].X) * (testPoint.Y - vertices[i].Y) / (vertices[j].Y - vertices[i].Y) + vertices[i].X))
                    c = !c;
            }
            return c;
        }

        /// <summary>
        /// finds the closest point on a polygon to a given point
        /// </summary>
        /// <param name="point">given point</param>
        /// <param name="polygon">polygon</param>
        /// <returns>closest point on polygon</returns>
        public static Vector2 ClosestPointOnPolygon(Vector2 point, Polygon polygon)
        {
            Vector2 closestPoint = Vector2.Zero;
            float closestDistance = float.MaxValue;

            foreach (LineSegment side in polygon.Sides)
            {
                Vector2 pointOnEdge = ClosestPointOnEdge(point, side);

                float distance = Vector2.DistanceSquared(point, pointOnEdge);
                if (distance < closestDistance)
                {
                    closestPoint = pointOnEdge;
                    closestDistance = distance;
                }
            }

            return closestPoint;
        }

        /// <summary>
        /// finds the closest point on a polygon to a given point
        /// </summary>
        /// <param name="point">given point</param>
        /// <param name="polygon">polygon</param>
        /// <param name="closestEdge">edge the point is on</param>
        /// <returns>closest point on polygon</returns>
        public static Vector2 ClosestPointOnPolygon(Vector2 point, Polygon polygon, ref LineSegment closestEdge)
        {
            Vector2 closestPoint = Vector2.Zero;
            float closestDistance = float.MaxValue;

            foreach (LineSegment side in polygon.Sides)
            {
                Vector2 pointOnEdge = ClosestPointOnEdge(point, side);

                float distance = Vector2.DistanceSquared(point, pointOnEdge);
                if (distance < closestDistance)
                {
                    closestPoint = pointOnEdge;
                    closestDistance = distance;
                    closestEdge = side;
                }
            }

            return closestPoint;
        }

        /// <summary>
        /// finds the closest point on a line segment to a given point
        /// </summary>
        /// <param name="point">given point</param>
        /// <param name="edge">line segment</param>
        /// <returns>closest point on line segment</returns>
        public static Vector2 ClosestPointOnEdge(Vector2 point, LineSegment edge)
        {
            // vector of edge
            Vector2 a = edge.Point2 - edge.Point1;

            // vector from initial point of edge to our point
            Vector2 b = point - edge.Point1;

            // scalar projection of b onto a
            float compBOntoA = Vector2.Dot(a, b) / a.Length();

            if (compBOntoA < 0)
                return edge.Point1;
            if (compBOntoA > a.Length())
                return edge.Point2;

            // unit vector of a
            a.Normalize();
            //a = a / a.Length();

            // vector projection of b onto a
            Vector2 projBontoA = a * compBOntoA;

            // vector projection plus initial point of edge
            Vector2 pointOnEdge = edge.Point1 + projBontoA;

            //pointOnEdge.X = MathHelper.Clamp(pointOnEdge.X, edge.Point1.X, edge.Point2.X);
            //pointOnEdge.Y = MathHelper.Clamp(pointOnEdge.Y, edge.Point1.Y, edge.Point2.Y);

            return pointOnEdge;
        }

        /*Vector2 closestPointOnEdge(Vector2 point, LineSegment edge)
        {

            Vector2 e = edge.Point2 - edge.Point1;

            Vector2 f = point - edge.Point1;

	        float e2 = Vector2.Dot(e, e);

	        float t = Vector2.Dot(f, e) / e2;

	        if (t < 0.0f) t = 0.0f;

	        else if (t > 1.0f) t = 1.0f;

	        Vector2 closest = edge.Point1 + e * t;

	        //Vector2 d = (closest - point);

	        //return (d * d);
            return closest;
        }*/

        // adds point2 but not point1
        public static Vector2[] CreateHalfCircle(Vector2 point1, Vector2 point2, bool positiveRotation)
        {
            List<Vector2> vectors = new List<Vector2>();
            //vectors.Add(point1);

            float radius = Vector2.Distance(point1, point2) / 2f;
            int numOfIntermediatePoints = (int)(radius / 2f);

            Vector2 midPoint = (point1 + point2) / 2;

            float angle = (float)Math.Atan2(point1.Y - midPoint.Y, point1.X - midPoint.X);

            float amountToRotateBy = (float)Math.PI / (numOfIntermediatePoints + 1);

            if (!positiveRotation)
                amountToRotateBy = -amountToRotateBy;

            for (int i = 0; i < numOfIntermediatePoints; i++)
            {
                angle += amountToRotateBy;

                Vector2 point = midPoint + new Vector2(radius * (float)Math.Cos(angle), radius * (float)Math.Sin(angle));

                point = midPoint + new Vector2(radius * (float)Math.Cos(angle), radius * (float)Math.Sin(angle));

                vectors.Add(point);
            }

            vectors.Add(point2);

            return vectors.ToArray();
        }

        // adds point2 but not point1
        public static Vector2[] CreateCatmullCurve(Vector2 previousPoint, Vector2 point1, Vector2 point2, Vector2 afterPoint, float curviness)
        {
            float length = Vector2.Distance(point1, point2);
            float distanceBetweenPreviousAndAfter = Vector2.Distance(previousPoint, afterPoint);
            Vector2 v1 = point1 - previousPoint;
            Vector2 v2 = point2 - afterPoint;
            float cos = Vector2.Dot(v1, v2) / (v1.Length() * v2.Length());
            float angle = (float)Math.Acos(cos);

            float radius = Vector2.Distance(point1, point2) / 2f;

            int numOfIntermediatePoints = (int)(radius / 2f);

            float weightIncrement = 1f / numOfIntermediatePoints * (2f / curviness);// / Math.Abs(cos * 1.5f);//((cos + 1.01f));//*;

            /*float weightIncrement = 1f / numOfIntermediatePoints;
            float angleRatio = angle / (MathHelper.PiOver2);
            // if vectors pointing are towards each other
            if (distanceBetweenPreviousAndAfter > length)
            {
                weightIncrement += weightIncrement * angleRatio;
            }
            // if vectors are pointing away from each other or parallel
            else
            {
                weightIncrement -= weightIncrement * angleRatio;
            }*/

            List<Vector2> vectors = new List<Vector2>();
            for (float s = weightIncrement; s < 1f; s += weightIncrement)
            {
                vectors.Add(Vector2.CatmullRom(previousPoint, point1, point2, afterPoint, s));
            }

            vectors.Add(point2);

            return vectors.ToArray();
        }

        /*public static Vector2[] CreateCurve(Vector2 point1, Vector2 point2, float curviness)
        {
            List<Vector2> vectors = new List<Vector2>();

            float radius = Vector2.Distance(point1, point2) / 2f;
            int numOfIntermediatePoints = (int)(radius / 2f);

            float weightIncrement = 1f / numOfIntermediatePoints * (2f / curviness);

            for (float s = weightIncrement; s < 1f; s += weightIncrement)
            {
                vectors.Add(Vector2.Hermite(point1, point2, s));
            }

            vectors.Add(point2);

            return vectors.ToArray();
        }*/

        // need at least 3 vertices
        public static Vector2[] CreateCatmullBlob(float curviness, params Vector2[] verts)
        {
            List<Vector2> vertices = new List<Vector2>();

            vertices.AddRange(Geometry.CreateCatmullCurve(verts[verts.Length - 1], verts[0], verts[1], verts[2], curviness));

            for (int i = 1; i < verts.Length - 2; i++)
            {
                vertices.AddRange(Geometry.CreateCatmullCurve(verts[i - 1], verts[i], verts[i + 1], verts[i + 2], curviness));
            }

            vertices.AddRange(Geometry.CreateCatmullCurve(verts[verts.Length - 3], verts[verts.Length - 2], verts[verts.Length - 1], verts[0], curviness));

            vertices.AddRange(Geometry.CreateCatmullCurve(verts[verts.Length - 2], verts[verts.Length - 1], verts[0], verts[1], curviness));

            return vertices.ToArray();
        }

        /*public static Vector2[] CreateCurveBlob(float curviness, params Vector2[] verts)
        {
            List<Vector2> vertices = new List<Vector2>();

            for (int i = 0; i < verts.Length - 1; i++)
            {
                vertices.AddRange(Geometry.CreateCurve(verts[i], verts[i + 1], curviness));
            }

            vertices.AddRange(Geometry.CreateCurve(verts[verts.Length - 1], verts[0], curviness));

            return vertices.ToArray();
        }*/

        public static Vector2 BezierInterpolate(Vector2 p0, Vector2 p1, Vector2 c0, Vector2 c1, float fraction)
        {
            // first stage, linear interpolate point pairs: [p0, c0], [c0, c1], [c1, p1]
            Vector2 p0c0 = Vector2.Lerp(p0, c0, fraction);
            Vector2 c0c1 = Vector2.Lerp(c0, c1, fraction);
            Vector2 c1p1 = Vector2.Lerp(c1, p1, fraction);

            // second stage, reduce to two points
            Vector2 l = Vector2.Lerp(p0c0, c0c1, fraction);
            Vector2 r = Vector2.Lerp(c0c1, c1p1, fraction);

            // final stage, reduce to result point and return
            return Vector2.Lerp(l, r, fraction);
        }
    }
}