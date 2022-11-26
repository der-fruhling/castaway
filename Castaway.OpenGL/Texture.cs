using System;
using System.ComponentModel.DataAnnotations;
using Castaway.Rendering;
using Castaway.Rendering.Objects;
using OpenTK.Graphics.OpenGL;

namespace Castaway.OpenGL;

public sealed class Texture : TextureObject
{
	public Texture(int number)
	{
		Number = number;
	}

	// TODO Move this into a method
	public Texture(int width, int height, float[]? data)
	{
		GL.GenTextures(1, out int t);
		GL.BindTexture(TextureTarget.Texture2D, t);
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
			(int)TextureMinFilter.NearestMipmapNearest);
		GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb,
			PixelType.Float, data);
		GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
		Number = t;
	}

	public bool Destroyed { get; set; }
	public int Number { get; set; }
	public override string Name => $"{Number}({Valid})";
	public override bool Valid => GL.IsTexture(Number);

	public uint BindingPoint { get; internal set; }

	[Obsolete("wtf")]
	public static implicit operator int(Texture t)
	{
		return t.Number;
	}

	public bool Equals(Texture other)
	{
		return Destroyed == other.Destroyed && Number == other.Number;
	}

	public override bool Equals(object? obj)
	{
		return obj is Texture other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Destroyed, Number);
	}

	public static bool operator ==(Texture left, Texture right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(Texture left, Texture right)
	{
		return !left.Equals(right);
	}

	public override void Dispose()
	{
		GL.DeleteTextures(1, new[] { Number });
	}

	public override void Bind([Range(0, 31)] int slot)
	{
		if (Graphics.Current is not OpenGLImpl gl) throw new InvalidOperationException("Need OpenGL >= 3.2");
		GL.ActiveTexture(TextureUnit.Texture0 + slot);
		GL.BindTexture(TextureTarget.Texture2D, Number);
		gl.BoundTextures[slot] = this;
	}

	public override void Unbind([Range(0, 31)] int slot)
	{
		if (Graphics.Current is not OpenGLImpl gl) throw new InvalidOperationException("Need OpenGL >= 3.2");
		GL.ActiveTexture(TextureUnit.Texture0 + slot);
		GL.BindTexture(TextureTarget.Texture2D, 0);
		gl.BoundTextures[slot] = this;
	}
}