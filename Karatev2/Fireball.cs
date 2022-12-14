using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Karate
{
    class Fireball
    {
        private Vector2 Position;
        private Vector2 Velocity;
        private Texture2D Texture;  //Texture in fireball

        public Fireball(Vector2 position, Vector2 velocity, Texture2D texture)  //Texture in fireball
        {
            // Konstruktorn för klassen fireball
            Position = position;
            Velocity = velocity;
            Texture = texture;
        }

        public void Update()
        {
            // Logiska uppdateringen
            Position += Velocity;
        }

        public void Draw(SpriteBatch _spriteBatch)  //Texture in fireball
        {
            _spriteBatch.Draw(Texture, Position, Color.White);
        }

        public Rectangle Hitbox()  //Texture in fireball
        {
            return new Rectangle((int)Position.X, (int)Position.Y, 
                Texture.Width, Texture.Height);
        }
    }
}
