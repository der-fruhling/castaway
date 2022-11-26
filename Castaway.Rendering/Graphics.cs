#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Threading;
using Castaway.Base;
using Castaway.Math;
using Castaway.Rendering.Objects;
using Castaway.Rendering.Shaders;
using Castaway.Rendering.Structures;

namespace Castaway.Rendering;

[SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
public abstract class Graphics : RenderObject
{
	private static readonly ThreadLocal<Window?> CurrentlyBound = new(() => null, true);

	public readonly Dictionary<BufferTarget, BufferObject?> BoundBuffers = new()
	{
		[BufferTarget.VertexArray] = null,
		[BufferTarget.ElementArray] = null
	};

	public readonly TextureObject?[] BoundTextures = new TextureObject?[32];
	public double ExpectedFramerate = 60.0;

	public List<float> FrameTimes = new();
	public Window? Window;

	public static Window[] BoundWindows => CurrentlyBound.Values.Where(w => w != null).ToArray()!;
	public double FrameChange => CastawayGlobal.Framerate / ExpectedFramerate;

	public static Graphics Current => CurrentlyBound.Value!.GL;

	public ShaderObject? BoundShader { get; set; }
	public FramebufferObject? BoundFramebuffer { get; set; }

	protected internal static void BindWindow(Window window)
	{
		if (CurrentlyBound.Value == window) return;
		window.IBind();
		CurrentlyBound.Value = window;
	}

	protected void BindWindow()
	{
		BindWindow(Window!);
	}

	public virtual void WindowInit(Window window)
	{
		throw new NotSupportedException();
	}

	public virtual BufferObject NewBuffer(BufferTarget target, float[]? data = null)
	{
		throw new NotSupportedException();
	}

	public virtual TextureObject NewTexture(int width, int height, Color color)
	{
		throw new NotSupportedException();
	}

	public virtual TextureObject NewTexture(int width, int height, float[]? data = null)
	{
		throw new NotSupportedException();
	}

	public virtual TextureObject NewTexture(Bitmap bitmap)
	{
		throw new NotSupportedException();
	}

	public virtual SeparatedShaderObject NewSepShader(ShaderStage stage, string source)
	{
		throw new NotSupportedException();
	}

	public virtual ShaderObject NewShader(params SeparatedShaderObject[] objects)
	{
		throw new NotSupportedException();
	}

	public virtual FramebufferObject NewFramebuffer()
	{
		throw new NotSupportedException();
	}

	public virtual Drawable NewDrawable(Mesh mesh)
	{
		throw new NotSupportedException();
	}

	public virtual void UnbindBuffer(BufferTarget target)
	{
		throw new NotSupportedException();
	}

	public virtual void UnbindTexture(int number)
	{
		throw new NotSupportedException();
	}

	public virtual void UnbindShader()
	{
		throw new NotSupportedException();
	}

	public virtual void UnbindFramebuffer()
	{
		throw new NotSupportedException();
	}

	public virtual object NativeRepresentation(RenderObject renderObject)
	{
		throw new NotSupportedException();
	}

	public virtual void FinishFrame(Window window)
	{
		throw new NotSupportedException();
	}

	public virtual void StartFrame()
	{
		throw new NotSupportedException();
	}

	public virtual void Draw(ShaderObject @object, Drawable buffer)
	{
		throw new NotSupportedException();
	}

	public virtual void SetFloatUniform(ShaderObject p, string name, float f)
	{
		throw new NotSupportedException();
	}

	public virtual void SetFloatUniform(ShaderObject p, string name, float x, float y)
	{
		throw new NotSupportedException();
	}

	public virtual void SetFloatUniform(ShaderObject p, string name, float x, float y, float z)
	{
		throw new NotSupportedException();
	}

	public virtual void SetFloatUniform(ShaderObject p, string name, float x, float y, float z, float w)
	{
		throw new NotSupportedException();
	}

	public virtual void SetIntUniform(ShaderObject p, string name, int i)
	{
		throw new NotSupportedException();
	}

	public virtual void SetIntUniform(ShaderObject p, string name, int x, int y)
	{
		throw new NotSupportedException();
	}

	public virtual void SetIntUniform(ShaderObject p, string name, int x, int y, int z)
	{
		throw new NotSupportedException();
	}

	public virtual void SetIntUniform(ShaderObject p, string name, int x, int y, int z, int w)
	{
		throw new NotSupportedException();
	}

	public virtual void SetUIntUniform(ShaderObject p, string name, uint i)
	{
		throw new NotSupportedException();
	}

	public virtual void SetUIntUniform(ShaderObject p, string name, uint x, uint y)
	{
		throw new NotSupportedException();
	}

	public virtual void SetUIntUniform(ShaderObject p, string name, uint x, uint y, uint z)
	{
		throw new NotSupportedException();
	}

	public virtual void SetUIntUniform(ShaderObject p, string name, uint x, uint y, uint z, uint w)
	{
		throw new NotSupportedException();
	}

	public virtual void SetDoubleUniform(ShaderObject p, string name, double i)
	{
		throw new NotSupportedException();
	}

	public virtual void SetDoubleUniform(ShaderObject p, string name, double x, double y)
	{
		throw new NotSupportedException();
	}

	public virtual void SetDoubleUniform(ShaderObject p, string name, double x, double y, double z)
	{
		throw new NotSupportedException();
	}

	public virtual void SetDoubleUniform(ShaderObject p, string name, double x, double y, double z, double w)
	{
		throw new NotSupportedException();
	}

	public virtual void SetFloatUniform(ShaderObject p, string name, Vector2 v)
	{
		SetFloatUniform(p, name, (float)v.X, (float)v.Y);
	}

	public virtual void SetFloatUniform(ShaderObject p, string name, Vector3 v)
	{
		SetFloatUniform(p, name, (float)v.X, (float)v.Y, (float)v.Z);
	}

	public virtual void SetFloatUniform(ShaderObject p, string name, Vector4 v)
	{
		SetFloatUniform(p, name, (float)v.X, (float)v.Y, (float)v.Z, (float)v.W);
	}

	public virtual void SetFloatUniform(ShaderObject p, string name, Matrix2 m)
	{
		throw new NotSupportedException();
	}

	public virtual void SetFloatUniform(ShaderObject p, string name, Matrix3 m)
	{
		throw new NotSupportedException();
	}

	public virtual void SetFloatUniform(ShaderObject p, string name, Matrix4 m)
	{
		throw new NotSupportedException();
	}

	public virtual void SetDoubleUniform(ShaderObject p, string name, Vector2 v)
	{
		SetDoubleUniform(p, name, v.X, v.Y);
	}

	public virtual void SetDoubleUniform(ShaderObject p, string name, Vector3 v)
	{
		SetDoubleUniform(p, name, v.X, v.Y, v.Z);
	}

	public virtual void SetDoubleUniform(ShaderObject p, string name, Vector4 v)
	{
		SetDoubleUniform(p, name, v.X, v.Y, v.Z, v.W);
	}

	public virtual void SetDoubleUniform(ShaderObject p, string name, Matrix2 m)
	{
		throw new NotSupportedException();
	}

	public virtual void SetDoubleUniform(ShaderObject p, string name, Matrix3 m)
	{
		throw new NotSupportedException();
	}

	public virtual void SetDoubleUniform(ShaderObject p, string name, Matrix4 m)
	{
		throw new NotSupportedException();
	}

	public virtual void SetIntUniform(ShaderObject p, string name, Vector2 v)
	{
		SetIntUniform(p, name, (int)v.X, (int)v.Y);
	}

	public virtual void SetIntUniform(ShaderObject p, string name, Vector3 v)
	{
		SetIntUniform(p, name, (int)v.X, (int)v.Y, (int)v.Z);
	}

	public virtual void SetIntUniform(ShaderObject p, string name, Vector4 v)
	{
		SetIntUniform(p, name, (int)v.X, (int)v.Y, (int)v.Z, (int)v.W);
	}

	public virtual void SetIntUniform(ShaderObject p, string name, Matrix2 m)
	{
		throw new NotSupportedException();
	}

	public virtual void SetIntUniform(ShaderObject p, string name, Matrix3 m)
	{
		throw new NotSupportedException();
	}

	public virtual void SetIntUniform(ShaderObject p, string name, Matrix4 m)
	{
		throw new NotSupportedException();
	}

	public virtual void SetUIntUniform(ShaderObject p, string name, Vector2 v)
	{
		SetUIntUniform(p, name, (uint)v.X, (uint)v.Y);
	}

	public virtual void SetUIntUniform(ShaderObject p, string name, Vector3 v)
	{
		SetUIntUniform(p, name, (uint)v.X, (uint)v.Y, (uint)v.Z);
	}

	public virtual void SetUIntUniform(ShaderObject p, string name, Vector4 v)
	{
		SetUIntUniform(p, name, (uint)v.X, (uint)v.Y, (uint)v.Z, (uint)v.W);
	}

	public virtual void SetUIntUniform(ShaderObject p, string name, Matrix2 m)
	{
		throw new NotSupportedException();
	}

	public virtual void SetUIntUniform(ShaderObject p, string name, Matrix3 m)
	{
		throw new NotSupportedException();
	}

	public virtual void SetUIntUniform(ShaderObject p, string name, Matrix4 m)
	{
		throw new NotSupportedException();
	}

	public virtual void SetSamplerUniform(ShaderObject p, string name, TextureObject t)
	{
		throw new NotSupportedException();
	}

	public virtual void SetSamplerUniform(ShaderObject p, string name, FramebufferObject t)
	{
		throw new NotSupportedException();
	}

	public virtual void SetFloatUniform(ShaderObject p, UniformType name, float f)
	{
		SetFloatUniform(p, p.GetUniform(name) ?? "", f);
	}

	public virtual void SetFloatUniform(ShaderObject p, UniformType name, float x, float y)
	{
		SetFloatUniform(p, p.GetUniform(name) ?? "", x, y);
	}

	public virtual void SetFloatUniform(ShaderObject p, UniformType name, float x, float y, float z)
	{
		SetFloatUniform(p, p.GetUniform(name) ?? "", x, y, z);
	}

	public virtual void SetFloatUniform(ShaderObject p, UniformType name, float x, float y, float z, float w)
	{
		SetFloatUniform(p, p.GetUniform(name) ?? "", x, y, z, w);
	}

	public virtual void SetIntUniform(ShaderObject p, UniformType name, int i)
	{
		SetIntUniform(p, p.GetUniform(name) ?? "", i);
	}

	public virtual void SetIntUniform(ShaderObject p, UniformType name, int x, int y)
	{
		SetIntUniform(p, p.GetUniform(name) ?? "", x, y);
	}

	public virtual void SetIntUniform(ShaderObject p, UniformType name, int x, int y, int z)
	{
		SetIntUniform(p, p.GetUniform(name) ?? "", x, y, z);
	}

	public virtual void SetIntUniform(ShaderObject p, UniformType name, int x, int y, int z, int w)
	{
		SetIntUniform(p, p.GetUniform(name) ?? "", x, y, z, w);
	}

	public virtual void SetUIntUniform(ShaderObject p, UniformType name, uint i)
	{
		SetUIntUniform(p, p.GetUniform(name) ?? "", i);
	}

	public virtual void SetUIntUniform(ShaderObject p, UniformType name, uint x, uint y)
	{
		SetUIntUniform(p, p.GetUniform(name) ?? "", x, y);
	}

	public virtual void SetUIntUniform(ShaderObject p, UniformType name, uint x, uint y, uint z)
	{
		SetUIntUniform(p, p.GetUniform(name) ?? "", x, y, z);
	}

	public virtual void SetUIntUniform(ShaderObject p, UniformType name, uint x, uint y, uint z, uint w)
	{
		SetUIntUniform(p, p.GetUniform(name) ?? "", x, y, z, w);
	}

	public virtual void SetIntUniform(ShaderObject p, UniformType name, double i)
	{
		SetDoubleUniform(p, p.GetUniform(name) ?? "", i);
	}

	public virtual void SetIntUniform(ShaderObject p, UniformType name, double x, double y)
	{
		SetDoubleUniform(p, p.GetUniform(name) ?? "", x, y);
	}

	public virtual void SetIntUniform(ShaderObject p, UniformType name, double x, double y, double z)
	{
		SetDoubleUniform(p, p.GetUniform(name) ?? "", x, y, z);
	}

	public virtual void SetIntUniform(ShaderObject p, UniformType name, double x, double y, double z, double w)
	{
		SetDoubleUniform(p, p.GetUniform(name) ?? "", x, y, z, w);
	}

	public virtual void SetFloatUniform(ShaderObject p, UniformType name, Vector2 v)
	{
		SetFloatUniform(p, p.GetUniform(name) ?? "", v);
	}

	public virtual void SetFloatUniform(ShaderObject p, UniformType name, Vector3 v)
	{
		SetFloatUniform(p, p.GetUniform(name) ?? "", v);
	}

	public virtual void SetFloatUniform(ShaderObject p, UniformType name, Vector4 v)
	{
		SetFloatUniform(p, p.GetUniform(name) ?? "", v);
	}

	public virtual void SetFloatUniform(ShaderObject p, UniformType name, Matrix2 m)
	{
		SetFloatUniform(p, p.GetUniform(name) ?? "", m);
	}

	public virtual void SetFloatUniform(ShaderObject p, UniformType name, Matrix3 m)
	{
		SetFloatUniform(p, p.GetUniform(name) ?? "", m);
	}

	public virtual void SetFloatUniform(ShaderObject p, UniformType name, Matrix4 m)
	{
		SetFloatUniform(p, p.GetUniform(name) ?? "", m);
	}

	public virtual void SetDoubleUniform(ShaderObject p, UniformType name, Vector2 v)
	{
		SetDoubleUniform(p, p.GetUniform(name) ?? "", v);
	}

	public virtual void SetDoubleUniform(ShaderObject p, UniformType name, Vector3 v)
	{
		SetDoubleUniform(p, p.GetUniform(name) ?? "", v);
	}

	public virtual void SetDoubleUniform(ShaderObject p, UniformType name, Vector4 v)
	{
		SetDoubleUniform(p, p.GetUniform(name) ?? "", v);
	}

	public virtual void SetDoubleUniform(ShaderObject p, UniformType name, Matrix2 m)
	{
		SetDoubleUniform(p, p.GetUniform(name) ?? "", m);
	}

	public virtual void SetDoubleUniform(ShaderObject p, UniformType name, Matrix3 m)
	{
		SetDoubleUniform(p, p.GetUniform(name) ?? "", m);
	}

	public virtual void SetDoubleUniform(ShaderObject p, UniformType name, Matrix4 m)
	{
		SetDoubleUniform(p, p.GetUniform(name) ?? "", m);
	}

	public virtual void SetIntUniform(ShaderObject p, UniformType name, Vector2 v)
	{
		SetIntUniform(p, p.GetUniform(name) ?? "", v);
	}

	public virtual void SetIntUniform(ShaderObject p, UniformType name, Vector3 v)
	{
		SetIntUniform(p, p.GetUniform(name) ?? "", v);
	}

	public virtual void SetIntUniform(ShaderObject p, UniformType name, Vector4 v)
	{
		SetIntUniform(p, p.GetUniform(name) ?? "", v);
	}

	public virtual void SetIntUniform(ShaderObject p, UniformType name, Matrix2 m)
	{
		SetIntUniform(p, p.GetUniform(name) ?? "", m);
	}

	public virtual void SetIntUniform(ShaderObject p, UniformType name, Matrix3 m)
	{
		SetIntUniform(p, p.GetUniform(name) ?? "", m);
	}

	public virtual void SetIntUniform(ShaderObject p, UniformType name, Matrix4 m)
	{
		SetIntUniform(p, p.GetUniform(name) ?? "", m);
	}

	public virtual void SetUIntUniform(ShaderObject p, UniformType name, Vector2 v)
	{
		SetUIntUniform(p, p.GetUniform(name) ?? "", v);
	}

	public virtual void SetUIntUniform(ShaderObject p, UniformType name, Vector3 v)
	{
		SetUIntUniform(p, p.GetUniform(name) ?? "", v);
	}

	public virtual void SetUIntUniform(ShaderObject p, UniformType name, Vector4 v)
	{
		SetUIntUniform(p, p.GetUniform(name) ?? "", v);
	}

	public virtual void SetUIntUniform(ShaderObject p, UniformType name, Matrix2 m)
	{
		SetUIntUniform(p, p.GetUniform(name) ?? "", m);
	}

	public virtual void SetUIntUniform(ShaderObject p, UniformType name, Matrix3 m)
	{
		SetUIntUniform(p, p.GetUniform(name) ?? "", m);
	}

	public virtual void SetUIntUniform(ShaderObject p, UniformType name, Matrix4 m)
	{
		SetUIntUniform(p, p.GetUniform(name) ?? "", m);
	}

	public virtual void SetSamplerUniform(ShaderObject p, UniformType name, TextureObject t)
	{
		SetSamplerUniform(p, p.GetUniform(name) ?? "", t);
	}

	public virtual void SetSamplerUniform(ShaderObject p, UniformType name, FramebufferObject t)
	{
		SetSamplerUniform(p, p.GetUniform(name) ?? "", t);
	}

	[Obsolete("Indexed uniform methods will soon be removed.")]
	public virtual void SetUniform(ShaderObject p, int i, UniformType name, float f)
	{
		SetFloatUniform(p, p.GetUniform(name, i)!, f);
	}

	[Obsolete("Indexed uniform methods will soon be removed.")]
	public virtual void SetUniform(ShaderObject p, int i, UniformType name, float x, float y)
	{
		SetFloatUniform(p, p.GetUniform(name, i)!, x, y);
	}

	[Obsolete("Indexed uniform methods will soon be removed.")]
	public virtual void SetUniform(ShaderObject p, int i, UniformType name, float x, float y, float z)
	{
		SetFloatUniform(p, p.GetUniform(name, i)!, x, y, z);
	}

	[Obsolete("Indexed uniform methods will soon be removed.")]
	public virtual void SetUniform(ShaderObject p, int i, UniformType name, float x, float y, float z, float w)
	{
		SetFloatUniform(p, p.GetUniform(name, i)!, x, y, z, w);
	}

	[Obsolete("Indexed uniform methods will soon be removed.")]
	public virtual void SetUniform(ShaderObject p, int j, UniformType name, int i)
	{
		SetIntUniform(p, p.GetUniform(name, j)!, i);
	}

	[Obsolete("Indexed uniform methods will soon be removed.")]
	public virtual void SetUniform(ShaderObject p, int i, UniformType name, int x, int y)
	{
		SetIntUniform(p, p.GetUniform(name, i)!, x, y);
	}

	[Obsolete("Indexed uniform methods will soon be removed.")]
	public virtual void SetUniform(ShaderObject p, int i, UniformType name, int x, int y, int z)
	{
		SetIntUniform(p, p.GetUniform(name, i)!, x, y, z);
	}

	[Obsolete("Indexed uniform methods will soon be removed.")]
	public virtual void SetUniform(ShaderObject p, int i, UniformType name, int x, int y, int z, int w)
	{
		SetIntUniform(p, p.GetUniform(name, i)!, x, y, z, w);
	}

	[Obsolete("Indexed uniform methods will soon be removed.")]
	public virtual void SetUniform(ShaderObject p, int i, UniformType name, Vector2 v)
	{
		SetFloatUniform(p, p.GetUniform(name, i)!, v);
	}

	[Obsolete("Indexed uniform methods will soon be removed.")]
	public virtual void SetUniform(ShaderObject p, int i, UniformType name, Vector3 v)
	{
		SetFloatUniform(p, p.GetUniform(name, i)!, v);
	}

	[Obsolete("Indexed uniform methods will soon be removed.")]
	public virtual void SetUniform(ShaderObject p, int i, UniformType name, Vector4 v)
	{
		SetFloatUniform(p, p.GetUniform(name, i)!, v);
	}

	[Obsolete("Indexed uniform methods will soon be removed.")]
	public virtual void SetUniform(ShaderObject p, int i, UniformType name, Matrix2 m)
	{
		SetFloatUniform(p, p.GetUniform(name, i)!, m);
	}

	[Obsolete("Indexed uniform methods will soon be removed.")]
	public virtual void SetUniform(ShaderObject p, int i, UniformType name, Matrix3 m)
	{
		SetFloatUniform(p, p.GetUniform(name, i)!, m);
	}

	[Obsolete("Indexed uniform methods will soon be removed.")]
	public virtual void SetUniform(ShaderObject p, int i, UniformType name, Matrix4 m)
	{
		SetFloatUniform(p, p.GetUniform(name, i)!, m);
	}

	[Obsolete("Indexed uniform methods will soon be removed.")]
	public virtual void SetUniform(ShaderObject p, int i, UniformType name, TextureObject t)
	{
		SetSamplerUniform(p, p.GetUniform(name, i)!, t);
	}

	[Obsolete("Indexed uniform methods will soon be removed.")]
	public virtual void SetUniform(ShaderObject p, int i, UniformType name, FramebufferObject t)
	{
		SetSamplerUniform(p, p.GetUniform(name, i)!, t);
	}

	public virtual void Clear()
	{
		throw new NotSupportedException();
	}

	public virtual void SetClearColor(float r, float g, float b)
	{
		throw new NotSupportedException();
	}

	public virtual void PutImage(int i, TextureObject texture)
	{
		throw new NotSupportedException();
	}

	public override void Dispose()
	{
		GC.SuppressFinalize(this);
		throw new NotSupportedException();
	}
}