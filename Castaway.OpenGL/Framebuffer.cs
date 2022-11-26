using System;
using Castaway.OpenGL.Native;
using Castaway.Rendering;
using Castaway.Rendering.Objects;
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
	public uint Number { get; set; }
	public override string Name => $"{Number}({Valid})";

	public override bool Valid =>
		(Color?.Valid ?? true) &&
		(Stencil?.Valid ?? true) &&
		(Depth?.Valid ?? true) &&
		!Destroyed;

	public override void Bind()
	{
		if (Graphics.Current is not OpenGLImpl gl) throw new InvalidOperationException("Need OpenGL >= 3.2");
		gl.BindFramebuffer(Number);
		gl.BoundFramebuffer = this;
	}

	public override void Unbind()
	{
		if (Graphics.Current is not OpenGLImpl gl) throw new InvalidOperationException("Need OpenGL >= 3.2");
		gl.UnbindFramebuffer();
		gl.BoundFramebuffer = null;
	}

	public override void Dispose()
	{
		GL.DeleteFramebuffers(1, Number);
	}

	private void New(int w, int h)
	{
		GL.GenFramebuffers(1, out var a);
		Number = a[0];
		GL.BindFramebuffer(GLC.GL_FRAMEBUFFER, Number);

		GL.GenTextures(1, out a);

		GL.BindTexture(GLC.GL_TEXTURE_2D, a[0]);
		GL.TexParameter(GLC.GL_TEXTURE_2D, GLC.GL_TEXTURE_WRAP_S, (int)GLC.GL_REPEAT);
		GL.TexParameter(GLC.GL_TEXTURE_2D, GLC.GL_TEXTURE_WRAP_T, (int)GLC.GL_REPEAT);
		GL.TexParameter(GLC.GL_TEXTURE_2D, GLC.GL_TEXTURE_MAG_FILTER, (int)GLC.GL_NEAREST);
		GL.TexParameter(GLC.GL_TEXTURE_2D, GLC.GL_TEXTURE_MIN_FILTER, (int)GLC.GL_NEAREST);
		GL.TexImage2D(GLC.GL_TEXTURE_2D, GLC.GL_ZERO, GLC.GL_RGB, w, h, GLC.GL_RGB, GLC.GL_FLOAT, null);
		GL.FramebufferTexture2D(GLC.GL_FRAMEBUFFER, GLC.GL_COLOR_ATTACHMENT0, GLC.GL_TEXTURE_2D, a[0], 0);

		GL.GenRenderbuffers(1, out a);
		GL.BindRenderbuffer(GLC.GL_RENDERBUFFER, a[0]);
		GL.RenderbufferStorage(GLC.GL_RENDERBUFFER, GLC.GL_DEPTH24_STENCIL8, w, h);
		GL.FramebufferRenderbuffer(GLC.GL_FRAMEBUFFER, GLC.GL_DEPTH_STENCIL_ATTACHMENT, GLC.GL_RENDERBUFFER, a[0]);

		Color = new Texture(a[0]);

		GL.BindFramebuffer(GLC.GL_FRAMEBUFFER, 0);
		Log.Verbose("Updated framebuffer {Number} to size {Width}x{Height}", Number, w, h);
	}

	public void Update(int w, int h)
	{
		Dispose();
		New(w, h);
	}
}