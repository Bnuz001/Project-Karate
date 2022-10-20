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
        /* Deklarerar alla variabler för spelet */
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
        private ParallaxTexture layer1;
        private ParallaxTexture layer2;
        private ParallaxTexture layer3;

        private SpriteFont font;

        private Player player;

        private bool hit;
        private bool isPlaying;
        private double score = 0;

        private List<Fireball> fireballs;
        private int fireballTimer = 120;
        private Random rnd;

        private const int STARTY = 400;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            /* Skapar värden för variabler */
            fireballs = new List<Fireball>();
            
            rnd = new Random();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            /* Laddar allt innehåll */
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

            layer1 = new ParallaxTexture(layer1Texture, 370);
            layer2 = new ParallaxTexture(layer2Texture, 300);
            layer3 = new ParallaxTexture(layer3Texture, 200);

            // Jämför med konstruktorn. Denna anropas här, när vi initierar objektet av klassen Player
            player = new Player(currentTexture, new Vector2(300, STARTY), Vector2.Zero);
        }

        protected override void Update(GameTime gameTime)
        {
            /* Den logiska biten i spelet!
             * Här skall vi inte rita ut något på skärmen!
             * Vi skall bara behandla spellogiken
             */
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Tangenbordshantering
            var state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Enter) && !isPlaying)
            {
                Reset();
                isPlaying = true;
            }

            // Om vi inte spelar, skall Update() inte köras vidare! 
            if (!isPlaying)
            {
                return;
            }

            // Uppdaterar bakgrunden
            layer1.OffsetX += 1.5f;
            layer2.OffsetX += 1.0f;
            layer3.OffsetX += 0.5f;

            //Uppdaterar tiden
            score += gameTime.ElapsedGameTime.TotalSeconds;

            //Uppdaterar spelaren!
            player.Update(STARTY);


            // Fortsätter med tangentbordshantering
            if (state.IsKeyDown(Keys.W) && !player.isJumping)
            {
                player.Velocity = new Vector2(0, -5.0f);

                player.isJumping = true;
            }

            if (state.IsKeyDown(Keys.S) && !player.isJumping)
            {
                player.isCrouching = true;
            }
            else
            {
                player.isCrouching = false;
            }

            // Fireballshantering
            fireballTimer--;

            if (fireballTimer == 0)
            {
                fireballTimer = 120;

                if (rnd.Next(2) == 0)
                {
                    Vector2 startpos = new Vector2(800, STARTY);
                    int velo = rnd.Next(-16, -3);
                    Vector2 fireballVelocity = new Vector2(velo, 0);
                    fireballs.Add(new Fireball(startpos, fireballVelocity, fireballTexture)); //Texture in fireball
                }
                else
                {
                    Vector2 startpos = new Vector2(800, STARTY + 40);
                    int velo = rnd.Next(-16, -3);
                    Vector2 fireballVelocity = new Vector2(velo, 0);
                    fireballs.Add(new Fireball(startpos, fireballVelocity, fireballTexture)); //Texture in fireball
                }
            }

            for (int i = 0; i < fireballs.Count; i++)
            {
                fireballs[i].Update();
            }

            // Uppdaterar rätt texture utifrån spelarens status
            if (player.isJumping)
            {
                player.Texture = jumpingTexture;
            }

            else if (player.isCrouching)
            {
                player.Texture = crouchTexture;
            }
            else
            {
                player.Texture = normalTexture;
            }

            //Kollisionshantering! Senare i kursen!
            Rectangle playerBox = player.Hitbox();

            foreach (var fireball in fireballs)
            {
                Rectangle fireballBox = fireball.Hitbox();  //Texture in fireball

                var kollision = Intersection(playerBox, fireballBox);

                if (kollision.Width > 0 && kollision.Height > 0)
                {
                    Rectangle r1 = Normalize(playerBox, kollision);
                    Rectangle r2 = Normalize(fireballBox, kollision);
                    hit = TestCollision(player.Texture, r1, fireballTexture, r2);

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

            //Ritar ut bakgrund!
            _spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);

            layer3.Draw(_spriteBatch);
            layer2.Draw(_spriteBatch);
            layer1.Draw(_spriteBatch);


            if (isPlaying)
            {
                // Skriver ut poäng i hörnet!
                _spriteBatch.DrawString(font, ((int)score).ToString(),
                new Vector2(10, 20), Color.White);

                // Ritar ut objektet på rätt ställe!
                player.Draw(_spriteBatch);

                // Ritar ut alla fireballs
                foreach (var fireball in fireballs)
                {
                    fireball.Draw(_spriteBatch);
                }
            }
            else
            {
                // Om vi inte spelar just nu, skall denna text skrivas ut!
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