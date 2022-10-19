using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Karatev2
{
    class Fireball
    {
        private Vector2 Position;
        private Vector2 Velocity;

        public Fireball(Vector2 position, Vector2 velocity)
        {
            Position = position;
            Velocity = velocity;
        }

        public void Update()
        {
            Position += Velocity;
        }

        public Vector2 GetPosition()
        {
            return Position;
        }

        public void Draw(SpriteBatch _spriteBatch, Texture2D texture)
        {
            _spriteBatch.Draw(texture, Position, Color.White);
        }

        public Rectangle Hitbox(Texture2D texture)
        {
            return new Rectangle((int)Position.X, (int)Position.Y, 
                texture.Width, texture.Height);
        }
    }
}
