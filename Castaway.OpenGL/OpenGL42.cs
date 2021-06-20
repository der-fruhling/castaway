using System;
using Castaway.OpenGL.Native;
using Castaway.Rendering;

namespace Castaway.OpenGL
{
    [Implements("OpenGL-4.2")]
    public class OpenGL42 : OpenGL41
    {
        public override string Name => "OpenGL-4.2";
        
        public override void PutImage(uint image, TextureObject texture)
        {
            if (texture is not Texture t) throw new InvalidOperationException("Must only use OpenGL types.");
            GL.BindImageTexture(image, t.Number, 0, false, 0, GLC.GL_READ_WRITE, GLC.GL_RGBA32F);
        }
    }
}