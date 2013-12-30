using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace poolio_balls
{
    public class Ball : IComparable<Ball>
    {
        public static float Friction = 0f;

        static List<Ball> balls = new List<Ball>();
        static float medianX;

        Rectangle rectangle;
        Vector2 position;
        Vector2 moveVector;
        public Rectangle Rectangle
        {
            get
            {
                return rectangle;
            }
        }
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                rectangle.Location = new Point((int)(position.X - Radius), (int)(position.Y - Radius));
            }
        }
        public Vector2 MoveVector
        {
            get
            {
                return moveVector;
            }
        }

        public GridNode CurrentGridNode { get; private set; }

        public float Rotation { get; set; }
        public float Radius { get; private set; }
        public float RadiusSquared { get; private set; }
        public float Weight { get; private set; }

        Ball(Vector2 position, int diameter, float density)
        {
            Radius = diameter / 2f;
            RadiusSquared = (float)Math.Pow(Radius, 2);
            rectangle = new Rectangle(0, 0, diameter, diameter);
            this.position = position;
            Weight = (float)(Math.PI * Math.Pow(Radius, 2) * density);

            CurrentGridNode = Game1.Grid.NodeAt(position.X, position.Y);
            CurrentGridNode.BallsContained.Add(this);
        }

        void doFriction(GameTime gameTime)
        {
            float actualFriction = Friction * (float)gameTime.ElapsedGameTime.TotalSeconds;

            moveVector = moveVector - moveVector * actualFriction;
        }

        void checkForWallCollision()
        {
            if (position.X - Radius <= 0)
                moveVector.X = -moveVector.X;
            else if (position.X + Radius >= Game1.Graphics.GraphicsDevice.Viewport.Width)
                moveVector.X = -moveVector.X;

            if (position.Y - Radius <= 0)
                moveVector.Y = -moveVector.Y;
            else if (position.Y + Radius >= Game1.Graphics.GraphicsDevice.Viewport.Height)
                moveVector.Y = -moveVector.Y;

            //restrictToViewPort();
        }

        List<Ball> alreadyHit = new List<Ball>();

        void checkGridNodeForBallCollision(GridNode node)
        {
            if (node == null)
                return;

            foreach (Ball ball in node.BallsContained)
            {
                if (ball == this)
                    continue;

                if (alreadyHit.Contains(ball))
                    continue;

                if (intersects(ball))
                {
                    ballCollision(ball);
                    ball.alreadyHit.Add(this);
                }
            }
        }

        void checkForBallCollision()
        {
            checkGridNodeForBallCollision(CurrentGridNode);

            if (position.X == CurrentGridNode.Rectangle.Center.X && position.Y == CurrentGridNode.Rectangle.Center.Y)
                return;

            // west
            if (position.X < CurrentGridNode.Rectangle.Center.X)
            {
                // just west
                if (position.Y == CurrentGridNode.Rectangle.Center.Y)
                {
                    checkGridNodeForBallCollision(CurrentGridNode.WestNeighbor);
                }
                // northwest
                else if (position.Y < CurrentGridNode.Rectangle.Center.Y)
                {
                    checkGridNodeForBallCollision(CurrentGridNode.WestNeighbor);
                    checkGridNodeForBallCollision(CurrentGridNode.NorthWestNeighbor);
                    checkGridNodeForBallCollision(CurrentGridNode.NorthNeighbor);
                }
                // southwest
                else
                {
                    checkGridNodeForBallCollision(CurrentGridNode.WestNeighbor);
                    checkGridNodeForBallCollision(CurrentGridNode.SouthWestNeighbor);
                    checkGridNodeForBallCollision(CurrentGridNode.SouthNeighbor);
                }
            }
            // east
            else
            {
                // just east
                if (position.Y == CurrentGridNode.Rectangle.Center.Y)
                {
                    checkGridNodeForBallCollision(CurrentGridNode.EastNeighbor);
                }
                // northeast
                else if (position.Y < CurrentGridNode.Rectangle.Center.Y)
                {
                    checkGridNodeForBallCollision(CurrentGridNode.EastNeighbor);
                    checkGridNodeForBallCollision(CurrentGridNode.NorthEastNeighbor);
                    checkGridNodeForBallCollision(CurrentGridNode.NorthNeighbor);
                }
                // southeast
                else
                {
                    checkGridNodeForBallCollision(CurrentGridNode.EastNeighbor);
                    checkGridNodeForBallCollision(CurrentGridNode.SouthEastNeighbor);
                    checkGridNodeForBallCollision(CurrentGridNode.SouthNeighbor);
                }
            }

            /*foreach (GridNode neighbor in CurrentGridNode.Neighbors)
            {
                foreach (Ball ball in neighbor.BallsContained)
                {
                    if (ball == this)
                        continue;

                    if (alreadyHit.Contains(ball))
                        continue;

                    if (intersects(ball))
                    {
                        ballCollision(ball);
                        ball.alreadyHit.Add(this);
                    }
                }
            }*/

            /*if (position.X <= medianX)
            {
                for (int i = 0; i < balls.Count; i++)
                {
                    Ball ball = balls[i];

                    if (ball == this)
                        continue;

                    if (ball.Position.X - ball.Radius > position.X + Radius)
                        break;
                    if (ball.Position.X + ball.Radius < position.X - Radius)
                        continue;

                    if (alreadyHit.Contains(ball))
                        continue;

                    if (intersects(ball))
                    {
                        ballCollision(ball);
                        ball.alreadyHit.Add(this);
                    }
                }
            }
            else
            {
                for (int i = balls.Count - 1; i >= 0; i--)
                {
                    Ball ball = balls[i];

                    if (ball == this)
                        continue;

                    if (ball.Position.X + ball.Radius < position.X - Radius)
                        break;
                    if (ball.Position.X - ball.Radius > position.X + Radius)
                        continue;

                    if (alreadyHit.Contains(ball))
                        continue;

                    if (intersects(ball))
                    {
                        ballCollision(ball);
                        ball.alreadyHit.Add(this);
                    }
                }
            }*/
        }

        void ballCollision(Ball ball)
        {
            float distance = Radius + ball.Radius;

            float v1 = moveVector.Length(), // my velocity
                v2 = ball.moveVector.Length(), // other ball velocity
                m1 = Weight, // my mass
                m2 = ball.Weight, // other ball mass
                theta1 = (float)Math.Atan2(moveVector.Y, moveVector.X), // move angle of me
                theta2 = (float)Math.Atan2(ball.moveVector.Y, ball.moveVector.X); // move angle of other ball

            bool adjustOtherBallFirst = (Weight > ball.Weight * 2 || v2 > v1);

            //float myMomentum = m1 * v1,
            //    otherMomentum = m2 * v2;

            // contact angle from other ball to me
            float phi = (float)Math.Atan2(position.Y - ball.position.Y, position.X - ball.position.X);

            // first find my new move components
            float sinTheta1MinusPhi = (float)Math.Sin(theta1 - phi);

            float numerator = (v1 * (float)Math.Cos(theta1 - phi) * (m1 - m2) + 2 * m2 * v2 * (float)Math.Cos(theta2 - phi));

            float newMoveX = numerator / (m1 + m2) * (float)Math.Cos(phi) + v1 * sinTheta1MinusPhi * (float)Math.Cos(phi + MathHelper.PiOver2);

            float newMoveY = numerator / (m1 + m2) * (float)Math.Sin(phi) + v1 * sinTheta1MinusPhi * (float)Math.Sin(phi + MathHelper.PiOver2);

            moveVector.X = newMoveX;
            moveVector.Y = newMoveY;

            // if im moving faster than other, adjust my position first
            if (!adjustOtherBallFirst)
                Position = new Vector2(ball.position.X + distance * (float)Math.Cos(phi), ball.position.Y + distance * (float)Math.Sin(phi));

            // contact angle from me to other ball
            //phi = (float)Math.Atan2(ball.position.Y - position.Y, ball.position.X - position.X);
            phi += MathHelper.Pi;

            // then find other ball new move components
            float sinTheta2MinusPhi = (float)Math.Sin(theta2 - phi);

            numerator = (v2 * (float)Math.Cos(theta2 - phi) * (m2 - m1) + 2 * m1 * v1 * (float)Math.Cos(theta1 - phi));

            newMoveX = numerator / (m2 + m1) * (float)Math.Cos(phi) + v2 * sinTheta2MinusPhi * (float)Math.Cos(phi + MathHelper.PiOver2);

            newMoveY = numerator / (m2 + m1) * (float)Math.Sin(phi) + v2 * sinTheta2MinusPhi * (float)Math.Sin(phi + MathHelper.PiOver2);

            ball.moveVector.X = newMoveX;
            ball.moveVector.Y = newMoveY;

            if (adjustOtherBallFirst)
                ball.Position = new Vector2(position.X + distance * (float)Math.Cos(phi), position.Y + distance * (float)Math.Sin(phi));
        }

        /*void ballCollision(Ball ball, Vector2 forceVector)
        {
            float angle = (float)Math.Atan2(position.Y - ball.position.Y, position.X - ball.position.X);
            //float angle = (float)Math.Atan2(ball.position.Y - position.Y, ball.position.X - position.X);

            // b, unit vector in direction of incoming force
            Vector2 angleVector = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

            //Vector2 moveVectorDifference = moveVector - ball.MoveVector;
            Vector2 moveVectorDifference = forceVector - moveVector;

            // comp forceVector on b
            //float forceScalar = Vector2.Dot(forceVector, angleVector) / angleVector.Length();
            float forceScalar = Vector2.Dot(moveVectorDifference, angleVector) / angleVector.Length();

            //// unit vector in direction of b
            //angleVector.Normalize();

            //float weightRatio = ball.Weight / Weight;

            float combinedWeight = ball.Weight + Weight;
            float otherWeightPercentage = ball.Weight / combinedWeight;

            otherWeightPercentage -= .5f;

            // proj c on b
            Vector2 forceProjectionVector = angleVector * forceScalar;// *incomingForce;
            forceProjectionVector += forceProjectionVector * otherWeightPercentage;

            //moveVector += weightRatio * angleVector * differenceInSpeed + forceProjectionVector;
            //moveVector += weightRatio * angleVector + forceProjectionVector + ball.Weight * 2 * angleVector;
            moveVector += forceProjectionVector;// *weightRatio;// *differenceInSpeed;

            float distance = Radius + ball.Radius;

            Position = new Vector2(ball.position.X + distance * (float)Math.Cos(angle), ball.position.Y + distance * (float)Math.Sin(angle));
        }*/

        void checkForPolygonCollision()
        {
            LineSegment edge = new LineSegment();
            Vector2 point = new Vector2();

            foreach (Polygon polygon in Polygon.Polygons)
            {
                if (Intersects(polygon, ref edge, ref point))
                {
                    polygonCollision(polygon, edge, point);
                }
            }
        }

        void polygonCollision(Polygon polygon, LineSegment edge, Vector2 point)
        {
            Vector2 edgeVector = edge.Point2 - edge.Point1;

            float dotProduct = Vector2.Dot(moveVector, edgeVector);

            if (dotProduct == 0)
            {
                moveVector = -moveVector;
                return;
            }

            if (dotProduct < 0)
            {
                edgeVector = -edgeVector;
                dotProduct = -dotProduct;
            }

            //dotProduct = Vector2.Dot(moveVector, edgeVector);
            float edgeVectorLength = edgeVector.Length();
            float moveVectorLength = moveVector.Length();

            float dotProductOverLength = dotProduct / (edgeVectorLength * moveVectorLength);

            float angleBetweenVectors;

            if (dotProductOverLength >= 1f)
                angleBetweenVectors = 0f;
            else if (dotProductOverLength <= -1f)
                angleBetweenVectors = MathHelper.Pi;
            else
                angleBetweenVectors = (float)Math.Acos(dotProduct / (edgeVectorLength * moveVectorLength));

            if (float.IsNaN(angleBetweenVectors))
            {

                int wut = 0;
            }

            float currentMoveAngle = (float)Math.Atan2(moveVector.Y, moveVector.X);

            Vector3 crossProduct = Vector3.Cross(new Vector3(edgeVector.X, edgeVector.Y, 0), new Vector3(moveVector.X, moveVector.Y, 0));

            float rotation;

            if (crossProduct.Z > 0)
                //rotation = (float)Math.PI - (2 * angleBetweenVectors);
                rotation = -2 * angleBetweenVectors;
            else
                //rotation = -((float)Math.PI - (2 * angleBetweenVectors));
                rotation = 2 * angleBetweenVectors;

            //float newMoveAngle = currentMoveAngle + rotation + (float)Math.PI;
            float newMoveAngle = currentMoveAngle + rotation;

            moveVector = new Vector2(moveVectorLength * (float)Math.Cos(newMoveAngle), moveVectorLength * (float)Math.Sin(newMoveAngle));

            //reposition ball
            float angleFromPoint = (float)Math.Atan2(position.Y - point.Y, position.X - point.X);
            // if inside polygon, reverse angle
            if (Geometry.PointInPolygon(position, polygon.Vertices))
                angleFromPoint += (float)Math.PI;
            Position = point + new Vector2(Radius * (float)Math.Cos(angleFromPoint), Radius * (float)Math.Sin(angleFromPoint));
        }

        void restrictToViewPort()
        {
            int viewportWidth = Game1.Graphics.GraphicsDevice.Viewport.Width,
                viewportHeight = Game1.Graphics.GraphicsDevice.Viewport.Height;

            //Position = new Vector2(MathHelper.Clamp(position.X, Radius, viewportWidth - Radius), 
            //    MathHelper.Clamp(position.Y, Radius, viewportHeight - Radius));

            if (position.X - Radius < 0)
            {
                position.X = Radius;
                rectangle.X = (int)(position.X - Radius);
            }
            else if (position.X + Radius > viewportWidth)
            {
                position.X = viewportWidth - Radius;
                rectangle.X = (int)(position.X - Radius);
            }

            if (position.Y - Radius < 0)
            {
                position.Y = Radius;
                rectangle.Y = (int)(position.Y - Radius);
            }
            else if (position.Y + Radius > viewportHeight)
            {
                position.Y = viewportHeight - Radius;
                rectangle.Y = (int)(position.Y - Radius);
            }
        }

        public void Push(Vector2 force)
        {
            moveVector += force;
        }

        void Move(GameTime gameTime)
        {
            position += MoveVector * (float)gameTime.ElapsedGameTime.TotalSeconds;
            rectangle.Location = new Point((int)(position.X - Radius), (int)(position.Y - Radius));

            restrictToViewPort();

            updateCurrentGridNode();
        }

        void updateCurrentGridNode()
        {
            GridNode newNode = Game1.Grid.NodeAt(position.X, position.Y);

            if (newNode != CurrentGridNode)
            {
                CurrentGridNode.BallsContained.Remove(this);
                newNode.BallsContained.Add(this);
                CurrentGridNode = newNode;
            }
        }

        void Update(GameTime gameTime)
        {
            doFriction(gameTime);

            checkForWallCollision();

            //checkForPolygonCollision();

            checkForBallCollision();
        }

        public static void UpdateBalls(GameTime gameTime)
        {
            //balls.Sort();

            //medianX = balls[balls.Count / 2].position.X;

            foreach (Ball ball in balls)
                ball.alreadyHit.Clear();

            foreach (Ball ball in balls)
            {
                ball.Move(gameTime);
                if (ball.moveVector == Vector2.Zero)
                    continue;
                ball.Update(gameTime);
            }
        }

        public static Ball CreateBall(Vector2 position, int diameter, float weight)
        {
            Ball ball = new Ball(position, diameter, weight);
            balls.Add(ball);

            return ball;
        }

        public static void RemoveBall(Ball ball)
        {
            balls.Remove(ball);
            ball.CurrentGridNode.BallsContained.Remove(ball);
        }

        public bool Contains(Vector2 point)
        {
            return Vector2.Distance(position, point) < (Radius);
        }

        bool intersects(Ball ball)
        {
            if (ball.position.X + ball.Radius < position.X - Radius
                || ball.position.X - ball.Radius > position.X + Radius
                || ball.position.Y + ball.Radius < position.Y - Radius
                || ball.position.Y - ball.Radius > position.Y + Radius)
                return false;

            return (Vector2.Distance(position, ball.position) <= Radius + ball.Radius);
        }

        public bool Intersects(Polygon polygon)
        {
            // return false if too far from polygon
            if (position.X + Radius < polygon.MinX || position.X - Radius > polygon.MaxX ||
                position.Y + Radius < polygon.MinY || position.Y - Radius > polygon.MaxY)
                return false;

            LineSegment edge = new LineSegment();

            // find closest point on polygon
            Vector2 pointOnPolygon = Geometry.ClosestPointOnPolygon(position, polygon, ref edge);

            // return true if that point is in the circle
            // aka if a polygon edge is in circle
            if (Vector2.DistanceSquared(position, pointOnPolygon) <= RadiusSquared)
                return true;

            // return true if circle center is inside polygon
            // aka circle is inside polygon
            return (Geometry.PointInPolygon(position, polygon.Vertices));
        }

        public bool Intersects(Polygon polygon, ref LineSegment edge, ref Vector2 pointOnPolygon)
        {
            // return false if too far from polygon
            if (position.X + Radius < polygon.MinX || position.X - Radius > polygon.MaxX ||
                position.Y + Radius < polygon.MinY || position.Y - Radius > polygon.MaxY)
                return false;

            // find closest point on polygon
            pointOnPolygon = Geometry.ClosestPointOnPolygon(position, polygon, ref edge);

            // return true if that point is in the circle
            // aka if a polygon edge is in circle
            if (Vector2.DistanceSquared(position, pointOnPolygon) <= RadiusSquared)
                return true;

            // return true if circle center is inside polygon
            // aka circle is inside polygon
            return (Geometry.PointInPolygon(position, polygon.Vertices));
        }

        public void Stop()
        {
            moveVector = Vector2.Zero;
        }

        public static Ball[] Balls
        {
            get
            {
                return balls.ToArray();
            }
        }

        /*static void sortBalls()
        {
            // sort balls by x
            for (int i = 1; i < balls.Count; i++)
            {
                for (int j = 1; j > 1 && balls[j].position.X < balls[j - 1].position.X; j--)
                {
                    Ball tempItem = balls[j];
                    balls.RemoveAt(j);
                    balls.Insert(j - 1, tempItem);
                }
            }
        }*/

        public int CompareTo(Ball other)
        {
            return position.X.CompareTo(other.position.X);
        }
    }
}
