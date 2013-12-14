using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace poolio_balls
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public static GraphicsDeviceManager Graphics;
        SpriteBatch spriteBatch;

        Random rand = new Random();

        TimeSpan fpsElapsedTime;
        string fpsMessage;
        int frameCounter;

        Texture2D ballTexture;
        Texture2D redTexture;
        SpriteFont font1;

        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
            //Graphics.ToggleFullScreen();
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            ballTexture = Content.Load<Texture2D>("blackcircle");
            redTexture = Content.Load<Texture2D>("red");
            font1 = Content.Load<SpriteFont>("font1");

            makeBalls();

            //new Polygon(new Vector2(200, 200), new Vector2(700, 150), new Vector2(800, 500), new Vector2(400, 500), new Vector2(500, 300));
            /*new Polygon(new Vector2(200, 200),
                new Vector2(Graphics.GraphicsDevice.Viewport.Width - 200, 200),
                new Vector2(Graphics.GraphicsDevice.Viewport.Width - 400, Graphics.GraphicsDevice.Viewport.Height - 200),
                new Vector2(Graphics.GraphicsDevice.Viewport.Width / 2, Graphics.GraphicsDevice.Viewport.Height),
                new Vector2(200, Graphics.GraphicsDevice.Viewport.Height - 200),
                new Vector2(100, Graphics.GraphicsDevice.Viewport.Height / 2));*/

            Vector2 p1 = new Vector2(200, 200),
                p2 = new Vector2(Graphics.GraphicsDevice.Viewport.Width - 200, 200),
                p3 = new Vector2(Graphics.GraphicsDevice.Viewport.Width - 400, Graphics.GraphicsDevice.Viewport.Height - 200),
                p4 = new Vector2(Graphics.GraphicsDevice.Viewport.Width / 2, Graphics.GraphicsDevice.Viewport.Height),
                p5 = new Vector2(200, Graphics.GraphicsDevice.Viewport.Height - 200),
                p6 = new Vector2(100, Graphics.GraphicsDevice.Viewport.Height / 2),
                p7 = new Vector2(Graphics.GraphicsDevice.Viewport.Width / 2, 300);

            List<Vector2> points = new List<Vector2>();

            /*points.AddRange(Geometry.CreateCatmullCurve(p6, p1, p2, p3, true, 1f));
            points.AddRange(Geometry.CreateCatmullCurve(p1, p2, p3, p4, true, 1f));
            points.AddRange(Geometry.CreateCatmullCurve(p2, p3, p4, p5, true, 1f));
            points.AddRange(Geometry.CreateCatmullCurve(p3, p4, p5, p6, true, 1f));
            points.AddRange(Geometry.CreateCatmullCurve(p4, p5, p6, p1, true, 1f));
            points.AddRange(Geometry.CreateCatmullCurve(p5, p6, p1, p2, true, 1f));*/

            /*List<Vector2> points = new List<Vector2>(Geometry.CreateCatmullCurve(p5, p6, p1, p2, true));
            points.Insert(0, p5);
            points.Insert(0, p4);
            points.Insert(0, p3);*/

            //new Polygon(points.ToArray());
            //new Polygon(Geometry.CreateBlob(1f, p1, p2, p3, p4, p5, p6));
            //new CatmullRomPolygon(1f, p1, p7, p2, p3, p4, p5, p6);

            Vector2 point1 = new Vector2(Graphics.GraphicsDevice.Viewport.Width - 250, Graphics.GraphicsDevice.Viewport.Height - 100),
                point2 = new Vector2(Graphics.GraphicsDevice.Viewport.Width - 175, Graphics.GraphicsDevice.Viewport.Height - 250),
                point3 = new Vector2(Graphics.GraphicsDevice.Viewport.Width - 150, Graphics.GraphicsDevice.Viewport.Height - 200);
            //new Polygon(point1, point2, point3);
            //List<Vector2> points = new List<Vector2>(Geometry.CreateHalfCircle(point2, point3, true));
            points = new List<Vector2>(Geometry.CreateCatmullCurve(point1, point2, point3, point1, 2f));
            points.Insert(0, point2);
            points.Insert(0, point1);
            
            //points.Insert(0, point3);
            new Polygon(points.ToArray());

            new CatmullRomPolygon(.5f, new Vector2(0, Graphics.GraphicsDevice.Viewport.Height - 400), new Vector2(0, Graphics.GraphicsDevice.Viewport.Height), new Vector2(220, Graphics.GraphicsDevice.Viewport.Height), new Vector2(220, Graphics.GraphicsDevice.Viewport.Height - 400));

            int xx = Graphics.GraphicsDevice.Viewport.Width / 2,
                yy = Graphics.GraphicsDevice.Viewport.Height / 2;
            new CatmullRomPolygon(1f, new Vector2(xx, yy), new Vector2(xx + 100, yy), new Vector2(xx + 100, yy + 100), new Vector2(xx, yy + 100));

            yy = Graphics.GraphicsDevice.Viewport.Height / 4;
            new CatmullRomPolygon(1f, new Vector2(xx - 50, yy - 50), new Vector2(xx + 25, yy + 50), new Vector2(xx + 50, yy), new Vector2(xx + 100, yy), new Vector2(xx + 125, yy + 50), new Vector2(xx + 200, yy - 50), new Vector2(xx + 100, yy - 100));

            base.Initialize();
        }

        void makeBalls()
        {
            for (int x = 1; x < 10; x++)
            {
                for (int y = 1; y < 10; y++)
                {
                    Ball ball = Ball.CreateBall(new Vector2(x * 26, y * 26), 20, 1);
                    //ball.Push(new Vector2(50000 * (float)rand.NextDouble() - 25000, 50000 * (float)rand.NextDouble() - 25000));
                }
            }

            //Ball ball1 = Ball.CreateBall(new Vector2(100, 100), 50, 1);
            //ball1.Push(new Vector2(5, 750));

            //Ball ball2 = Ball.CreateBall(new Vector2(100, 200), 50, 1);
            //ball2.Push(new Vector2(-5, -750));

            Ball ball3 = Ball.CreateBall(new Vector2(1000, 100), 100, 1);

            //Ball ball4 = Ball.CreateBall(new Vector2(1000, 100), 25, 1);
        }

        void removeBalls()
        {
            foreach (Ball ball in Ball.Balls)
            {
                Ball.RemoveBall(ball);
            }
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        float maxTotalMovement = 0;
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            // Allows the game to exit
            if (keyboardState.IsKeyDown(Keys.Escape))
                this.Exit();

            if (keyboardState.IsKeyDown(Keys.S))
            {
                foreach (Ball ball in Ball.Balls)
                    ball.Stop();
            }
            else if (keyboardState.IsKeyDown(Keys.R))
            {
                removeBalls();
                makeBalls();
            }
            else if (keyboardState.IsKeyDown(Keys.OemPlus))
            {
                foreach (Ball ball in Ball.Balls)
                {
                    float angle = (float)Math.Atan2(ball.MoveVector.Y, ball.MoveVector.X);
                    ball.Push(5 * (1 + Ball.Friction * ball.MoveVector.Length() / 100) * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)));
                }
            }
            else if (keyboardState.IsKeyDown(Keys.OemMinus))
            {
                foreach (Ball ball in Ball.Balls)
                {
                    if (ball.MoveVector.Length() < 5)
                    {
                        ball.Stop();
                        continue;
                    }
                    float angle = (float)Math.Atan2(ball.MoveVector.Y, ball.MoveVector.X) + MathHelper.Pi;
                    ball.Push(5 * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)));
                }
            }

            //update fps
            fpsElapsedTime += gameTime.ElapsedGameTime;
            if (fpsElapsedTime > TimeSpan.FromSeconds(1))
            {
                //Game1.Game.Window.Title = "FPS: " + (frameCounter > 2 ? frameCounter.ToString() : "COOL");
                fpsMessage = "FPS: " + (frameCounter > 2 ? frameCounter.ToString() : "COOL");
                fpsElapsedTime -= TimeSpan.FromSeconds(1);
                frameCounter = 0;
            }

            checkForClick();

            Ball.UpdateBalls(gameTime);

            /*foreach (Ball ball in Ball.Balls)
            {
                //if (polygon.ContainsPoint(ball.Position))
                if (ball.Intersects(polygon))
                    ball.Stop();
            }*/

            float totalMoveVectorLength = 0;
            foreach (Ball ball in Ball.Balls)
                totalMoveVectorLength += (float)Math.Sqrt(ball.MoveVector.Length() * ball.Weight);

            if (totalMoveVectorLength > maxTotalMovement)
                maxTotalMovement = totalMoveVectorLength;

            /*Vector2 totalMomentum = Vector2.Zero;
            foreach (Ball ball in Ball.Balls)
                totalMomentum += ball.MoveVector * ball.Weight;

            totalMoveVectorLength = totalMomentum.Length();*/

            Window.Title = fpsMessage + " - " + totalMoveVectorLength.ToString() + " - Max: " + maxTotalMovement;

            base.Update(gameTime);
        }

        void checkForClick()
        {
            MouseState mouseState = Mouse.GetState();
            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                foreach (Ball ball in Ball.Balls)
                {
                    if (ball.Contains(mousePosition))
                    {
                        float angle = (float)Math.Atan2(ball.Position.Y - mousePosition.Y, ball.Position.X - mousePosition.X);
                        ball.Push(new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 50);
                    }
                }
            }

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                foreach (Ball ball in Ball.Balls)
                {
                    if (ball.Contains(mousePosition))
                    {
                        ball.Stop();
                        ball.Position = mousePosition;
                    }
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            frameCounter++;

            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            foreach (Ball ball in Ball.Balls)
                spriteBatch.Draw(ballTexture, ball.Rectangle, Color.White * .95f);

            foreach (Polygon polygon in Polygon.Polygons)
                polygon.Render(spriteBatch);

            foreach (Ball ball in Ball.Balls)
            {
                /*foreach (LineSegment edge in polygon.Sides)
                {
                    Vector2 point = Geometry.ClosestPointOnEdge(ball.Position, edge);

                    spriteBatch.Draw(redTexture, new Rectangle((int)point.X - 2, (int)point.Y - 2, 5, 5), Color.White * .8f);
                }*/
                foreach (Polygon polygon in Polygon.Polygons)
                {
                    //Vector2 point = Geometry.ClosestPointOnPolygon(ball.Position, polygon);

                    //spriteBatch.Draw(redTexture, new Rectangle((int)point.X - 2, (int)point.Y - 2, 5, 5), Color.White * .8f);

                    string str = polygon.Vertices.Length.ToString();
                    Vector2 strSize = font1.MeasureString(str);
                    spriteBatch.DrawString(font1, str, new Vector2((int)(polygon.CenterPoint.X - strSize.X / 2), (int)(polygon.CenterPoint.Y - strSize.Y / 2)), Color.Black);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
