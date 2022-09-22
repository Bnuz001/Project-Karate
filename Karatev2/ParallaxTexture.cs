using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Karatev2
{
    class ParallaxTexture
    {
        private Texture2D _texture;
        private int _positionY;

        public double OffsetX { get; set; }

        public ParallaxTexture(Texture2D Texture, int PositionY)
        {
            _texture = Texture;
            _positionY = PositionY;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int width = spriteBatch.GraphicsDevice.Adapter.CurrentDisplayMode.Width;

            //int startX = OffsetX % _texture.Width;

            int textureWidth = _texture.Width;


        }
    }
}
