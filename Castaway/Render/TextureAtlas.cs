using System.Drawing;
using Castaway.Math;

namespace Castaway.Render
{
    public class TextureAtlas
    {
        public readonly struct Bounds
        {
            public readonly Vector2 Position, Size;
            // ReSharper disable InconsistentNaming
            public Vector2 TL => Position;
            public Vector2 TR => Position + new Vector2(Size.X, 0);
            public Vector2 BL => Position + new Vector2(0, Size.Y);
            public Vector2 BR => Position + Size;
            // ReSharper restore InconsistentNaming

            public Bounds(Vector2 position, Vector2 size)
            {
                Position = position;
                Size = size;
            }
        }

        private readonly int _tileWidth, _tileHeight;
        public readonly Texture Texture;
        
        public TextureAtlas(Texture texture, int tileWidth, int tileHeight)
        {
            _tileWidth = tileWidth;
            _tileHeight = tileHeight;
            Texture = texture;
        }
        
        public Bounds GetBounds(int x, int y)
        {
            var w = (float)Texture.Image.Width;
            var h = (float)Texture.Image.Height;
            var tx = x * _tileWidth / w;
            var ty = y * _tileHeight / h;
            
            return new Bounds(new Vector2(tx, ty), 
                new Vector2(_tileWidth / w, _tileHeight / h));
        }
    }
}