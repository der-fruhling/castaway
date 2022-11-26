using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using Castaway.Base;
using Castaway.Input;
using Castaway.Math;
using Castaway.OpenGL.Native;
using Castaway.Rendering;
using Castaway.Rendering.Objects;
using Castaway.Rendering.Shaders;
using Castaway.Rendering.Structures;
using GLFW;
using Serilog;
using Graphics = Castaway.Rendering.Graphics;
using Window = Castaway.Rendering.Window;

namespace Castaway.OpenGL;

[Implements("OpenGL-3.2")]
public class OpenGLImpl : Graphics
{
	private static readonly ILogger Logger = CastawayGlobal.GetLogger();

	private readonly Stopwatch _stopwatch = new();

	public OpenGLImpl()
	{
		GL.Init();
	}

	public override string Name => "OpenGL-3.2";

	public override void Dispose()
	{
		GC.SuppressFinalize(this);
	}

	public override void WindowInit(Window window)
	{
		InputSystem.Init();
		GL.Enable(GLC.GL_DEPTH_TEST);
		GL.Enable(GLC.GL_CULL_FACE);
		window.WindowResize += (w, h) => GL.Viewport(0, 0, w, h);
	}

	public override BufferObject NewBuffer(BufferTarget target, float[]? data = null)
	{
		BindWindow();
		return new Buffer(target, data ?? Array.Empty<float>());
	}

	public override TextureObject NewTexture(int width, int height, Color color)
	{
		BindWindow();
		var data = new float[width * height * 3];
		var r = color.R / (float)byte.MaxValue;
		var g = color.G / (float)byte.MaxValue;
		var b = color.B / (float)byte.MaxValue;

		for (var i = 0; i < width * height * 3; i += 3)
		{
			data[i + 0] = r;
			data[i + 1] = g;
			data[i + 2] = b;
		}

		return new Texture(width, height, data);
	}

	public override TextureObject NewTexture(int width, int height, float[]? data = null)
	{
		BindWindow();
		return new Texture(width, height, data);
	}

	public override TextureObject NewTexture(Bitmap bitmap)
	{
		BindWindow();
		var data = new float[bitmap.Width * bitmap.Height * 3];

		for (var i = 0; i < bitmap.Width; i++)
		for (var j = 0; j < bitmap.Height; j++)
		{
			var k = i * j * 3;
			var p = bitmap.GetPixel(i, j);
			data[k + 0] = p.R / (float)byte.MaxValue;
			data[k + 1] = p.G / (float)byte.MaxValue;
			data[k + 2] = p.B / (float)byte.MaxValue;
		}

		return new Texture(bitmap.Width, bitmap.Height, data);
	}

	public override SeparatedShaderObject NewSepShader(ShaderStage stage, string source)
	{
		BindWindow();
		return new ShaderPart(stage, source, "???");
	}

	public override ShaderObject NewShader(params SeparatedShaderObject[] objects)
	{
		BindWindow();
		return new Shader(objects);
	}

	public override FramebufferObject NewFramebuffer()
	{
		BindWindow();
		var f = new Framebuffer();
		Window!.WindowResize += f.Update;
		return f;
	}

	public override Drawable NewDrawable(Mesh mesh)
	{
		BindWindow();
		return mesh.ConstructFor(BoundShader!);
	}

	public override object NativeRepresentation(RenderObject renderObject)
	{
		BindWindow();
		dynamic d = renderObject;
		try
		{
			return d.Number is uint u
				? u
				: throw new InvalidOperationException($"{renderObject.GetType().Name} is not an OpenGL type.");
		}
		catch (MissingMemberException e)
		{
			if (renderObject is not Buffer or Texture or ShaderPart or Shader or Framebuffer)
				throw new InvalidOperationException($"{renderObject.GetType().Name} is not an OpenGL type.", e);
			throw;
		}
	}

	public override void FinishFrame(Window window)
	{
		window.SwapBuffers();
		InputSystem.Clear();
		_stopwatch.Stop();
		FrameTimes.Insert(0, (float)_stopwatch.Elapsed.TotalSeconds);
		const int MaxTimes = 60;
		if (FrameTimes.Count > MaxTimes) FrameTimes.RemoveRange(MaxTimes, FrameTimes.Count - MaxTimes);
	}

    /// <summary>
    ///     Should be called at the start of the frame.
    /// </summary>
    public override void StartFrame()
	{
		BindWindow();
		_stopwatch.Restart();
		Glfw.PollEvents();
		if (InputSystem.Keyboard.WasJustPressed(Keys.F11))
			Window!.Fullscreen = !Window!.Fullscreen;
		Clear();
		if (InputSystem.Gamepad.Valid) InputSystem.Gamepad.Read();
	}

	public override void Draw(ShaderObject shader, Drawable buffer)
	{
		BindWindow();
		if (buffer.VertexArray == null) throw new InvalidOperationException("Drawables must have a vertex array.");
		if (buffer.VertexArray is not Buffer v)
			throw new InvalidOperationException(
				$"Cannot use vertex buffer of type {buffer.VertexArray?.GetType()}");
		if (shader is not Shader s)
			throw new InvalidOperationException($"Cannot use shader of type {shader.GetType().FullName}");

		if (buffer is VertexArrayDrawable vaoDraw)
		{
			BindVAO(vaoDraw.VAO);
			if (!vaoDraw.SetUp)
			{
				buffer.VertexArray.Bind();
				s.Binder!.Apply(v);
				vaoDraw.SetUp = true;
			}

			if (vaoDraw.ElementArray != null)
				GL.DrawElements(GLC.GL_TRIANGLES, buffer.VertexCount, GLC.GL_UNSIGNED_INT, 0);
			else
				GL.DrawArrays(GLC.GL_TRIANGLES, 0, buffer.VertexCount);
			UnbindVAO();
		}
		else
		{
			var vao = CreateVAO();
			BindVAO(vao);
			buffer.VertexArray.Bind();
			s.Binder!.Apply(v);
			if (buffer.ElementArray != null)
			{
				if (buffer.ElementArray is not Buffer)
					throw new InvalidOperationException(
						$"Cannot use element buffer of type {buffer.ElementArray?.GetType()}");
				buffer.ElementArray.Bind();
				GL.DrawElements(GLC.GL_TRIANGLES, buffer.VertexCount, GLC.GL_UNSIGNED_INT, 0);
			}
			else
			{
				GL.DrawArrays(GLC.GL_TRIANGLES, 0, buffer.VertexCount);
			}

			DeleteVAOs(vao);
		}
	}

	public override void SetFloatUniform(ShaderObject p, string name, float f)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.SetUniform(GL.GetUniformLocation(s.Number, name), 1, new[] { f });
	}

	public override void SetFloatUniform(ShaderObject p, string name, float x, float y)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.SetUniformVector2(GL.GetUniformLocation(s.Number, name), 1, new[] { x, y });
	}

	public override void SetFloatUniform(ShaderObject p, string name, float x, float y, float z)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.SetUniformVector3(GL.GetUniformLocation(s.Number, name), 1, new[] { x, y, z });
	}

	public override void SetFloatUniform(ShaderObject p, string name, float x, float y, float z, float w)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.SetUniformVector4(GL.GetUniformLocation(s.Number, name), 1, new[] { x, y, z, w });
	}

	public override void SetIntUniform(ShaderObject p, string name, int i)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.SetUniform(GL.GetUniformLocation(s.Number, name), 1, new[] { i });
	}

	public override void SetIntUniform(ShaderObject p, string name, int x, int y)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.SetUniformVector2(GL.GetUniformLocation(s.Number, name), 1, new[] { x, y });
	}

	public override void SetIntUniform(ShaderObject p, string name, int x, int y, int z)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.SetUniformVector3(GL.GetUniformLocation(s.Number, name), 1, new[] { x, y, z });
	}

	public override void SetIntUniform(ShaderObject p, string name, int x, int y, int z, int w)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.SetUniformVector4(GL.GetUniformLocation(s.Number, name), 1, new[] { x, y, z, w });
	}

	public override void SetUIntUniform(ShaderObject p, string name, uint i)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.SetUniform(GL.GetUniformLocation(s.Number, name), 1, new[] { i });
	}

	public override void SetUIntUniform(ShaderObject p, string name, uint x, uint y)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.SetUniformVector2(GL.GetUniformLocation(s.Number, name), 1, new[] { x, y });
	}

	public override void SetUIntUniform(ShaderObject p, string name, uint x, uint y, uint z)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.SetUniformVector3(GL.GetUniformLocation(s.Number, name), 1, new[] { x, y, z });
	}

	public override void SetUIntUniform(ShaderObject p, string name, uint x, uint y, uint z, uint w)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.SetUniformVector4(GL.GetUniformLocation(s.Number, name), 1, new[] { x, y, z, w });
	}

	public override void SetFloatUniform(ShaderObject p, string name, Vector2 v)
	{
		BindWindow();
		if (name.Length == 0) return;
		SetFloatUniform(p, name, (float)v.X, (float)v.Y);
	}

	public override void SetFloatUniform(ShaderObject p, string name, Vector3 v)
	{
		BindWindow();
		if (name.Length == 0) return;
		SetFloatUniform(p, name, (float)v.X, (float)v.Y, (float)v.Z);
	}

	public override void SetFloatUniform(ShaderObject p, string name, Vector4 v)
	{
		BindWindow();
		if (name.Length == 0) return;
		SetFloatUniform(p, name, (float)v.X, (float)v.Y, (float)v.Z, (float)v.W);
	}

	public override void SetFloatUniform(ShaderObject p, string name, Matrix2 m)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.SetUniformMatrix2(GL.GetUniformLocation(s.Number, name), 1, false, m.ArrayF);
	}

	public override void SetFloatUniform(ShaderObject p, string name, Matrix3 m)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.SetUniformMatrix3(GL.GetUniformLocation(s.Number, name), 1, false, m.ArrayF);
	}

	public override void SetFloatUniform(ShaderObject p, string name, Matrix4 m)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.SetUniformMatrix4(GL.GetUniformLocation(s.Number, name), 1, false, m.ArrayF);
	}

	public override void SetSamplerUniform(ShaderObject p, string name, TextureObject t)
	{
		BindWindow();
		if (t is not Texture tex) throw new InvalidOperationException("Need to use OpenGL objects only");
		SetIntUniform(p, name, (int)tex.BindingPoint);
	}

	public override void SetSamplerUniform(ShaderObject p, string name, FramebufferObject t)
	{
		BindWindow();
		if (t.Color is not Texture tex) throw new InvalidOperationException("Need to use OpenGL objects only");
		SetIntUniform(p, name, (int)tex.BindingPoint);
	}

    /// <summary>
    ///     Clears the color, depth, and stencil buffers in the current render
    ///     target.
    /// </summary>
    public override void Clear()
	{
		BindWindow();
		GL.Clear();
	}

    /// <summary>
    ///     Sets the color that <see cref="Color" /> clears to.
    /// </summary>
    /// <param name="r">Red</param>
    /// <param name="g">Green</param>
    /// <param name="b">Blue</param>
    public override void SetClearColor(float r, float g, float b)
	{
		BindWindow();
		GL.ClearColor(r, g, b, 1);
	}

	internal virtual uint[] NewBuffers(int count)
	{
		BindWindow();
		GL.GenBuffers(count, out var a);
		return a;
	}

	internal virtual uint[] NewTextures(int count)
	{
		BindWindow();
		GL.GenTextures(count, out var a);
		return a;
	}

	internal virtual uint NewShader(ShaderStage stage)
	{
		BindWindow();
		return GL.CreateShader(stage switch
		{
			ShaderStage.Vertex => GL.ShaderStage.VertexShader,
			ShaderStage.Fragment => GL.ShaderStage.FragmentShader,
			_ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
		});
	}

	internal virtual uint NewProgram()
	{
		BindWindow();
		return GL.CreateProgram();
	}

	internal virtual void BindBuffer(BufferTarget target, uint b)
	{
		BindWindow();
		GL.BindBuffer(target switch
		{
			BufferTarget.VertexArray => GL.BufferTarget.ArrayBuffer,
			BufferTarget.ElementArray => GL.BufferTarget.ElementArrayBuffer,
			_ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
		}, b);
	}

	public override void UnbindBuffer(BufferTarget target)
	{
		BindWindow();
		BindBuffer(target, 0);
	}

	internal virtual void MakeActiveTexture([Range(0, 31)] int number)
	{
		BindWindow();
		GL.ActiveTexture(GLC.GL_TEXTURE0 + number);
	}

	internal virtual void BindTexture(uint t)
	{
		BindWindow();
		GL.BindTexture(GLC.GL_TEXTURE_2D, t);
	}

	internal virtual void UnbindTexture()
	{
		BindWindow();
		GL.BindTexture(GLC.GL_TEXTURE_2D, 0);
	}

	internal virtual void BindTexture(int number, uint t)
	{
		MakeActiveTexture(number);
		BindTexture(t);
	}

	public override void UnbindTexture(int number)
	{
		MakeActiveTexture(number);
		UnbindTexture();
	}

	internal virtual void BindShader(uint p)
	{
		BindWindow();
		GL.UseProgram(p);
	}

	internal virtual void BindFramebuffer(uint number)
	{
		BindWindow();
		GL.BindFramebuffer(GLC.GL_FRAMEBUFFER, number);
	}

	public override void UnbindFramebuffer()
	{
		BindWindow();
		GL.BindFramebuffer(GLC.GL_FRAMEBUFFER, 0);
	}

	public override void UnbindShader()
	{
		BindWindow();
		GL.UseProgram(0);
	}

	internal virtual uint CreateVAO()
	{
		BindWindow();
		GL.GenVertexArrays(1, out var a);
		return a[0];
	}

	internal virtual uint[] CreateVAOs(int count)
	{
		BindWindow();
		GL.GenVertexArrays(count, out var a);
		return a;
	}

	internal virtual void BindVAO(uint n)
	{
		BindWindow();
		GL.BindVertexArray(n);
	}

	internal virtual void UnbindVAO()
	{
		BindWindow();
		GL.BindVertexArray(0);
	}

	internal virtual void DeleteVAOs(params uint[] vaos)
	{
		BindWindow();
		GL.DeleteVertexArrays(vaos);
	}
}