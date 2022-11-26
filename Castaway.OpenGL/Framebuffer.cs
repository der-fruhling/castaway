using System;
using Castaway.Rendering;
using Castaway.Rendering.Objects;
using OpenTK.Graphics.OpenGL;
using Serilog;

namespace Castaway.OpenGL;

internal sealed class Framebuffer : FramebufferObject
{
	public Framebuffer()
	{
		var window = Graphics.Current.Window;
		window!.GetFramebufferSize(out var w, out var h);
		New(w, h);
	}

	public bool Destroyed { get; set; }
	public int Number { get; set; }
	public override string Name => $"{Number}({Valid})";

	public override bool Valid =>
		(Color?.Valid ?? true) &&
		(Stencil?.Valid ?? true) &&
		(Depth?.Valid ?? true) &&
		!Destroyed;

	public override void Bind()
	{
		if (Graphics.Current is not OpenGLImpl gl) throw new InvalidOperationException("Need OpenGL >= 3.2");
		GL.BindFramebuffer(FramebufferTarget.Framebuffer, Number);
		gl.BoundFramebuffer = this;
	}

	public override void Unbind()
	{
		if (Graphics.Current is not OpenGLImpl gl) throw new InvalidOperationException("Need OpenGL >= 3.2");
		GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		gl.BoundFramebuffer = null;
	}

	public override void Dispose()
	{
		GL.DeleteFramebuffers(1, new[] { Number });
	}

	private void New(int w, int h)
	{
		GL.GenFramebuffers(1, out int a);
		Number = a;
		GL.BindFramebuffer(FramebufferTarget.Framebuffer, Number);

		GL.GenTextures(1, out a);

		GL.BindTexture(TextureTarget.Texture2D, a);
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Nearest);
		GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, w, h, 0, PixelFormat.Rgb, PixelType.Float,
			IntPtr.Zero);
		GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
			TextureTarget.Texture2D, a, 0);

		GL.GenRenderbuffers(1, out a);
		GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, a);
		GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, w, h);
		GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment,
			RenderbufferTarget.Renderbuffer, a);

		Color = new Texture(a);

		GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		Log.Verbose("Updated framebuffer {Number} to size {Width}x{Height}", Number, w, h);
	}

	public void Update(int w, int h)
	{
		Dispose();
		New(w, h);
	}
}