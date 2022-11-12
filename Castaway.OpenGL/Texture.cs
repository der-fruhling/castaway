using System;
using Castaway.OpenGL.Native;
using Castaway.Rendering;
using Castaway.Rendering.Objects;

namespace Castaway.OpenGL;

public sealed class Texture : TextureObject
{
    public Texture(uint number)
    {
        Number = number;
    }

    public Texture(int width, int height, float[]? data)
    {
        GL.GenTextures(1, out var at);
        var t = at[0];
        GL.BindTexture(GLC.GL_TEXTURE_2D, t);
        GL.TexParameter(GLC.GL_TEXTURE_2D, GLC.GL_TEXTURE_WRAP_S, (int) GLC.GL_REPEAT);
        GL.TexParameter(GLC.GL_TEXTURE_2D, GLC.GL_TEXTURE_WRAP_T, (int) GLC.GL_REPEAT);
        GL.TexParameter(GLC.GL_TEXTURE_2D, GLC.GL_TEXTURE_MAG_FILTER, (int) GLC.GL_NEAREST_MIPMAP_NEAREST);
        GL.TexParameter(GLC.GL_TEXTURE_2D, GLC.GL_TEXTURE_MIN_FILTER, (int) GLC.GL_NEAREST_MIPMAP_NEAREST);
        GL.TexImage2D(GLC.GL_TEXTURE_2D, 0, GLC.GL_RGB, width, height, GLC.GL_ZERO, GLC.GL_FLOAT, data);
        GL.GenerateMipmap(GLC.GL_TEXTURE_2D);
        Number = t;
    }

    public bool Destroyed { get; set; }
    public uint Number { get; set; }
    public override string Name => $"{Number}({Valid})";
    public override bool Valid => GL.IsTexture(Number);

    public uint BindingPoint { get; internal set; }

    public static implicit operator uint(Texture t)
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
        GL.DeleteTextures(1, Number);
    }

    public override void Bind(int slot)
    {
        if (Graphics.Current is not OpenGLImpl gl) throw new InvalidOperationException("Need OpenGL >= 3.2");
        gl.BindTexture(slot, Number);
        gl.BoundTextures[slot] = this;
    }

    public override void Unbind(int slot)
    {
        if (Graphics.Current is not OpenGLImpl gl) throw new InvalidOperationException("Need OpenGL >= 3.2");
        gl.UnbindTexture(slot);
        gl.BoundTextures[slot] = this;
    }
}