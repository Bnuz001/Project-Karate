using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Karate
{
    /// <summary>
    /// Class Player for Karategame
    /// </summary>
    class Player
    {
        /* Deklara alla variabler i klassen */
        public Texture2D Texture;
        public Vector2 Position;
        public Vector2 Velocity;
        public bool isJumping;
        public bool isCrouching;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        public Player(Texture2D texture, Vector2 position, Vector2 velocity)
        {
            /* Konstruktorn av klassen 
             * Denna körs när vi skapar ett objekt av klassen Player
             */
            Texture = texture;
            Position = position;
            Velocity = velocity;
            isJumping = false;
            isCrouching = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="StartY"></param>
        public void Update(int StartY)
        {
            /* Sköter logiken på Player
             */
            Position += Velocity;

            if (Position.Y > StartY)
            {
                Position = new Vector2(Position.X, StartY);
                Velocity = Vector2.Zero;
                isJumping = false;
                //Hej
            }

            Velocity += new Vector2(0, 0.2f);

        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            /* Ritar ut spelaren på skärmen */
            _spriteBatch.Draw(Texture, Position, Color.White);
        }

        public Rectangle Hitbox()
        {
            /* Används för kollisionshantering! Djupdyker i denna senare! */
            return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
        }
    }
}
