using System;
using Castaway.Native;

namespace Castaway.Render
{
    public class DrawContext : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Draw(DrawObject o)
        {
            o.Adjust(Shader.Active);
            CawNative.cawDraw(o.Buffer, o.Count);
        }
    }
}