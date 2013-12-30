using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace poolio_balls
{
    public class Polygon
    {
        public static List<Polygon> Polygons = new List<Polygon>();

        List<Vector2> vertices = new List<Vector2>();
        public Vector2[] Vertices
        {
            get
            {
                return vertices.ToArray();
            }
        }

        List<LineSegment> sides = new List<LineSegment>();
        public LineSegment[] Sides
        {
            get
            {
                return sides.ToArray();
            }
        }

        PrimitiveLine line;

        public float MinX { get; private set; }
        public float MaxX { get; private set; }
        public float MinY { get; private set; }
        public float MaxY { get; private set; }
        public float MaxWidthOverTwo { get; private set; }
        public float MaxHeightOverTwo { get; private set; }
        public Vector2 CenterPoint { get; private set; }

        public List<GridNode> OccupiedGridNodes = new List<GridNode>();

        float epsilon; // padding for ray casting

        /*Polygon(Vector2 p1, Vector2 p2, params Vector2[] vertices)
        {
            Polygons.Add(this);

            if (p1.X < p2.X)
            {
                MinX = p1.X;
                MaxX = p2.X;
            }
            else
            {
                MinX = p2.X;
                MaxX = p1.X;
            }

            if (p1.Y < p2.Y)
            {
                MinY = p1.Y;
                MaxY = p2.Y;
            }
            else
            {
                MinY = p2.Y;
                MaxY = p1.Y;
            }

            this.vertices.Add(p1);
            this.vertices.Add(p2);

            foreach (Vector2 vertex in vertices)
            {
                this.vertices.Add(vertex);

                if (vertex.X < MinX)
                    MinX = vertex.X;
                else if (vertex.X > MaxX)
                    MaxX = vertex.X;

                if (vertex.Y < MinY)
                    MinY = vertex.Y;
                else if (vertex.Y > MaxY)
                    MaxY = vertex.Y;
            }

            for (int i = 0; i < this.vertices.Count; i++)
            {
                if (i < this.vertices.Count - 1)
                {

                    sides.Add(new LineSegment(this.vertices[i], this.vertices[i + 1]));
                }
                else
                {
                    sides.Add(new LineSegment(this.vertices[i], p1));
                }
            }

            epsilon = (MaxX - MinX) / 100f;

            line = new PrimitiveLine(Game1.Graphics.GraphicsDevice, 1);
            line.Colour = Color.White;
        }*/

        /*public Polygon(Vector2 p1, Vector2 p2, params Vector2[] vertices)
        {
            Polygons.Add(this);

            if (p1.X < p2.X)
            {
                MinX = p1.X;
                MaxX = p2.X;
            }
            else
            {
                MinX = p2.X;
                MaxX = p1.X;
            }

            if (p1.Y < p2.Y)
            {
                MinY = p1.Y;
                MaxY = p2.Y;
            }
            else
            {
                MinY = p2.Y;
                MaxY = p1.Y;
            }

            this.vertices.Add(p1);
            this.vertices.Add(p2);

            foreach (Vector2 vertex in vertices)
            {
                this.vertices.Add(vertex);

                if (vertex.X < MinX)
                    MinX = vertex.X;
                else if (vertex.X > MaxX)
                    MaxX = vertex.X;

                if (vertex.Y < MinY)
                    MinY = vertex.Y;
                else if (vertex.Y > MaxY)
                    MaxY = vertex.Y;
            }

            for (int i = 0; i < this.vertices.Count; i++)
            {
                if (i < this.vertices.Count - 1)
                {

                    sides.Add(new LineSegment(this.vertices[i], this.vertices[i + 1]));
                }
                else
                {
                    sides.Add(new LineSegment(this.vertices[i], p1));
                }
            }

            epsilon = (MaxX - MinX) / 100f;

            CenterPoint = new Vector2((MinX + MaxX) / 2, (MinY + MaxY) / 2);

            line = new PrimitiveLine(Game1.Graphics.GraphicsDevice, 1);
            line.Colour = Color.White;
        }*/

        public Polygon(params Vector2[] vertices)
        {
            if (vertices.Length < 3)
                throw new Exception("inside Polygon constructor: must have at least 3 vertices");

            Polygons.Add(this);

            MinX = MaxX = vertices[0].X;
            MinY = MaxY = vertices[0].Y;

            foreach (Vector2 vertex in vertices)
            {
                this.vertices.Add(vertex);

                if (vertex.X < MinX)
                    MinX = vertex.X;
                else if (vertex.X > MaxX)
                    MaxX = vertex.X;

                if (vertex.Y < MinY)
                    MinY = vertex.Y;
                else if (vertex.Y > MaxY)
                    MaxY = vertex.Y;
            }

            for (int i = 0; i < this.vertices.Count; i++)
            {
                if (i < this.vertices.Count - 1)
                {

                    sides.Add(new LineSegment(this.vertices[i], this.vertices[i + 1]));
                }
                else
                {
                    sides.Add(new LineSegment(this.vertices[i], this.vertices[0]));
                }
            }

            epsilon = (MaxX - MinX) / 100f;

            CenterPoint = new Vector2((MinX + MaxX) / 2, (MinY + MaxY) / 2);

            line = new PrimitiveLine(Game1.Graphics.GraphicsDevice, 1);
            line.Colour = Color.White;

            initializeOccupiedGridNodes();
        }

        void initializeOccupiedGridNodes()
        {
        }

        public bool ContainsPoint(Vector2 p)
        {
            if (p.X < MinX || p.X > MaxX || p.Y < MinY || p.Y > MaxY)
                return false;

            Vector2 rayStart = new Vector2(MinX - epsilon, p.Y);
            Vector2 rayEnd = p;
            LineSegment ray = new LineSegment(rayStart, rayEnd);

            // Test the ray against all sides
            int intersections = 0;
            foreach (LineSegment side in sides)
            {
                if (Geometry.LineSegmentIntersect(ray, side))
                    intersections++;
            }
            // if odd return true
            if ((intersections & 1) == 1)
                return true;
            // if even return false
            else
                return false;
        }

        public void Render(SpriteBatch spriteBatch, SpriteFont font)
        {
            /*foreach (LineSegment side in sides)
            {
                line.ClearVectors();
                line.AddVector(side.Point1);
                line.AddVector(side.Point2);
                line.Render(spriteBatch);
            }*/

            line.ClearVectors();

            foreach (Vector2 vert in vertices)
                line.AddVector(vert);

            line.AddVector(vertices[0]);

            line.Render(spriteBatch);

            string str = vertices.Count.ToString();
            Vector2 strSize = font.MeasureString(str);
            spriteBatch.DrawString(font, str, new Vector2((int)(CenterPoint.X - strSize.X / 2), (int)(CenterPoint.Y - strSize.Y / 2)), Color.Black);
        }
    }
}
