using System;
using Castaway.Rendering;
using Castaway.Rendering.Objects;
using OpenTK.Graphics.OpenGL;

namespace Castaway.OpenGL;

[Implements("OpenGL-4.2")]
public class OpenGL42 : OpenGL41
{
	public override string Name => "OpenGL-4.2";

	public override void PutImage(int image, TextureObject texture)
	{
		BindWindow();
		if (texture is not Texture t) throw new InvalidOperationException("Must only use OpenGL types.");
		GL.BindImageTexture(image, t.Number, 0, false, 0, TextureAccess.ReadWrite, SizedInternalFormat.Rgba32f);
	}
}