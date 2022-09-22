using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace Karatev2
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D normalTexture;
        private Texture2D jumpingTexture;
        private Texture2D crouchTexture;
        private Texture2D fireballTexture;
        private Texture2D currentTexture;

        private Texture2D backgroundTexture;
        private Texture2D layer1Texture;
        private Texture2D layer2Texture;
        private Texture2D layer3Texture;

        private SpriteFont font;

        private Vector2 position;
        private Vector2 velocity;
        private bool isJumping;
        private bool isCrouching;
        private bool hit;
        private bool isPlaying;

        private List<Vector2> fireballs;
        private int fireballTimer = 120;
        private Random rnd;
        private double score = 0;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            position = new Vector2(300, 200);
            fireballs = new List<Vector2>();
            rnd = new Random();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            normalTexture = Content.Load<Texture2D>("normal");
            jumpingTexture = Content.Load<Texture2D>("jump");
            crouchTexture = Content.Load<Texture2D>("crouch");
            fireballTexture = Content.Load<Texture2D>("fireball");
            font = Content.Load<SpriteFont>("font");

            backgroundTexture = Content.Load<Texture2D>("background");
            layer1Texture = Content.Load<Texture2D>("layer1");
            layer2Texture = Content.Load<Texture2D>("layer2");
            layer3Texture = Content.Load<Texture2D>("layer3");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Enter) && !isPlaying)
            {
                Reset();
                isPlaying = true;
            }

            if (!isPlaying)
            {
                return;
            }

            score += gameTime.ElapsedGameTime.TotalSeconds;

            position += velocity;

            if (position.Y > 200)
            {
                position = new Vector2(position.X, 200);
                velocity = Vector2.Zero;
                isJumping = false;
            }

            velocity += new Vector2(0, 0.2f);





            if (state.IsKeyDown(Keys.W) && !isJumping)
            {
                velocity = new Vector2(0, -5.0f);

                isJumping = true;
            }

            if (state.IsKeyDown(Keys.S) && !isJumping)
            {
                isCrouching = true;
            }
            else
            {
                isCrouching = false;
            }

            //Fireballs!!!!!
            fireballTimer--;

            if (fireballTimer == 0)
            {
                fireballTimer = 120;

                if (rnd.Next(2) == 0)
                {
                    fireballs.Add(new Vector2(800, 200));
                }
                else
                {
                    fireballs.Add(new Vector2(800, 240));
                }

            }

            for (int i = 0; i < fireballs.Count; i++)
            {
                fireballs[i] = fireballs[i] + new Vector2(-2, 0);
            }

            if (isJumping)
            {
                currentTexture = jumpingTexture;
            }

            else if (isCrouching)
            {
                currentTexture = crouchTexture;
            }
            else
            {
                currentTexture = normalTexture;
            }


            Rectangle playerBox = new Rectangle((int)position.X, (int)position.Y,
                currentTexture.Width, currentTexture.Height);

            foreach (var fireball in fireballs)
            {
                Rectangle fireballBox = new Rectangle((int)fireball.X, (int)fireball.Y,
                    fireballTexture.Width, fireballTexture.Height);

                //Överlappar vi?
                var kollision = Intersection(playerBox, fireballBox);

                if (kollision.Width > 0 && kollision.Height > 0)
                {
                    Rectangle r1 = Normalize(playerBox, kollision);
                    Rectangle r2 = Normalize(fireballBox, kollision);
                    hit = TestCollision(currentTexture, r1, fireballTexture, r2);

                    if (hit)
                    {
                        isPlaying = false;
                    }
                }
            }




            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);

            if (isPlaying)
            {
                _spriteBatch.DrawString(font, ((int)score).ToString(),
                new Vector2(10, 20), Color.White);

                _spriteBatch.Draw(currentTexture, position, Color.White);

                foreach (var fireball in fireballs)
                {
                    _spriteBatch.Draw(fireballTexture, fireball, Color.White);
                }

            }
            else
            {
                _spriteBatch.DrawString(font, "Press ENTER to start!",
                    new Vector2(350, 200), Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void Reset()
        {
            fireballs.Clear();
            fireballTimer = 120;
            score = 0;
        }

        public static Rectangle Intersection(Rectangle r1, Rectangle r2)
        {
            int x1 = Math.Max(r1.Left, r2.Left);
            int y1 = Math.Max(r1.Top, r2.Top);
            int x2 = Math.Min(r1.Right, r2.Right);
            int y2 = Math.Min(r1.Bottom, r2.Bottom);

            if ((x2 >= x1) && (y2 >= y1))
            {
                return new Rectangle(x1, y1, x2 - x1, y2 - y1);
            }
            return Rectangle.Empty;
        }

        public static Rectangle Normalize(Rectangle reference, Rectangle overlap)
        {
            //Räkna ut en rektangel som kan användas relativt till referensrektangeln
            return new Rectangle(
              overlap.X - reference.X,
              overlap.Y - reference.Y,
              overlap.Width,
              overlap.Height);
        }

        public static bool TestCollision(Texture2D t1, Rectangle r1, Texture2D t2, Rectangle r2)
        {
            //Beräkna hur många pixlar som finns i området som ska undersökas
            int pixelCount = r1.Width * r1.Height;
            uint[] texture1Pixels = new uint[pixelCount];
            uint[] texture2Pixels = new uint[pixelCount];

            //Kopiera ut pixlarna från båda områdena
            t1.GetData(0, r1, texture1Pixels, 0, pixelCount);
            t2.GetData(0, r2, texture2Pixels, 0, pixelCount);

            //Jämför om vi har några pixlar som överlappar varandra i områdena
            for (int i = 0; i < pixelCount; ++i)
            {
                if (((texture1Pixels[i] & 0xff000000) > 0) && ((texture2Pixels[i] & 0xff000000) > 0))
                {
                    return true;
                }
            }
            return false;
        }
    }
}