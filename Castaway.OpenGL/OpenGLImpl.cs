using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Castaway.Base;
using Castaway.Input;
using Castaway.Math;
using Castaway.Rendering;
using Castaway.Rendering.Objects;
using Castaway.Rendering.Shaders;
using Castaway.Rendering.Structures;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Serilog;
using SixLabors.ImageSharp.PixelFormats;
using BufferTarget = OpenTK.Graphics.OpenGL.BufferTarget;
using CBufferTarget = Castaway.Rendering.BufferTarget;
using Color = System.Drawing.Color;
using Image = SixLabors.ImageSharp.Image;
using Window = Castaway.Rendering.Window;

namespace Castaway.OpenGL;

[Implements("OpenGL-3.2")]
public class OpenGLImpl : Graphics
{
	private static readonly ILogger Logger = CastawayGlobal.GetLogger();

	private readonly Stopwatch _stopwatch = new();

	public OpenGLImpl()
	{
		// just in case
#pragma warning disable CS0612
		Native.GL.Init();
#pragma warning restore CS0612
	}

	public override string Name => "OpenGL-3.2";

	public override void Dispose()
	{
		GC.SuppressFinalize(this);
	}

	public override void WindowInit(Window window)
	{
		GL.LoadBindings(new BindingsContext());
		InputSystem.Init();
		GL.Enable(EnableCap.DepthTest);
		GL.Enable(EnableCap.CullFace);
		window.WindowResize += (w, h) => GL.Viewport(0, 0, w, h);
		Logger.Debug("Initialized window context for use with the OpenGL renderer");
	}

	public override BufferObject NewBuffer(CBufferTarget target, float[]? data = null)
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

	public override TextureObject NewTexture(Image image)
	{
		BindWindow();
		var img = image.CloneAs<Rgb24>();
		var data = new float[image.Width * image.Height * 3];

		for (var i = 0; i < image.Width; i++)
		for (var j = 0; j < image.Height; j++)
		{
			var k = i * j * 3;
			var p = img[i, j];
			data[k + 0] = p.R / (float)byte.MaxValue;
			data[k + 1] = p.G / (float)byte.MaxValue;
			data[k + 2] = p.B / (float)byte.MaxValue;
		}

		return new Texture(image.Width, image.Height, data);
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
		const int maxTimes = 60;
		if (FrameTimes.Count > maxTimes) FrameTimes.RemoveRange(maxTimes, FrameTimes.Count - maxTimes);
	}

	/// <summary>
	///     Should be called at the start of the frame.
	/// </summary>
	public override void StartFrame()
	{
		BindWindow();
		_stopwatch.Restart();
		GLFW.PollEvents();
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
			GL.BindVertexArray(vaoDraw.VertexArrayObject);
			if (!vaoDraw.SetUp)
			{
				buffer.VertexArray.Bind();
				s.Binder!.Apply(v);
				vaoDraw.SetUp = true;
			}

			if (vaoDraw.ElementArray != null)
				GL.DrawElements(PrimitiveType.Triangles, buffer.VertexCount, DrawElementsType.UnsignedInt, 0);
			else
				GL.DrawArrays(PrimitiveType.Triangles, 0, buffer.VertexCount);
			GL.BindVertexArray(vaoDraw.VertexArrayObject);
		}
		else
		{
			GL.CreateVertexArrays(1, out uint vao);
			GL.BindVertexArray(vao);
			buffer.VertexArray.Bind();
			s.Binder!.Apply(v);
			if (buffer.ElementArray != null)
			{
				if (buffer.ElementArray is not Buffer)
					throw new InvalidOperationException(
						$"Cannot use element buffer of type {buffer.ElementArray?.GetType()}");
				buffer.ElementArray.Bind();
				GL.DrawElements(PrimitiveType.Triangles, buffer.VertexCount, DrawElementsType.UnsignedInt, 0);
			}
			else
			{
				GL.DrawArrays(PrimitiveType.Triangles, 0, buffer.VertexCount);
			}

			GL.DeleteVertexArray(vao);
		}
	}

	public override void SetFloatUniform(ShaderObject p, string name, float f)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.Uniform1(GL.GetUniformLocation((uint)s.Number, name), 1, new[] { f });
	}

	public override void SetFloatUniform(ShaderObject p, string name, float x, float y)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.Uniform2(GL.GetUniformLocation((uint)s.Number, name), 1, new[] { x, y });
	}

	public override void SetFloatUniform(ShaderObject p, string name, float x, float y, float z)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.Uniform3(GL.GetUniformLocation((uint)s.Number, name), 1, new[] { x, y, z });
	}

	public override void SetFloatUniform(ShaderObject p, string name, float x, float y, float z, float w)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.Uniform4(GL.GetUniformLocation((uint)s.Number, name), 1, new[] { x, y, z, w });
	}

	public override void SetIntUniform(ShaderObject p, string name, int i)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.Uniform1(GL.GetUniformLocation((uint)s.Number, name), 1, new[] { i });
	}

	public override void SetIntUniform(ShaderObject p, string name, int x, int y)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.Uniform2(GL.GetUniformLocation((uint)s.Number, name), 1, new[] { x, y });
	}

	public override void SetIntUniform(ShaderObject p, string name, int x, int y, int z)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.Uniform3(GL.GetUniformLocation((uint)s.Number, name), 1, new[] { x, y, z });
	}

	public override void SetIntUniform(ShaderObject p, string name, int x, int y, int z, int w)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.Uniform4(GL.GetUniformLocation((uint)s.Number, name), 1, new[] { x, y, z, w });
	}

	public override void SetUIntUniform(ShaderObject p, string name, uint i)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.Uniform1(GL.GetUniformLocation((uint)s.Number, name), 1, new[] { i });
	}

	public override void SetUIntUniform(ShaderObject p, string name, uint x, uint y)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.Uniform2(GL.GetUniformLocation((uint)s.Number, name), 1, new[] { x, y });
	}

	public override void SetUIntUniform(ShaderObject p, string name, uint x, uint y, uint z)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.Uniform3(GL.GetUniformLocation((uint)s.Number, name), 1, new[] { x, y, z });
	}

	public override void SetUIntUniform(ShaderObject p, string name, uint x, uint y, uint z, uint w)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.Uniform4(GL.GetUniformLocation((uint)s.Number, name), 1, new[] { x, y, z, w });
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
		GL.UniformMatrix2(GL.GetUniformLocation((uint)s.Number, name), 1, false, m.ArrayF);
	}

	public override void SetFloatUniform(ShaderObject p, string name, Matrix3 m)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.UniformMatrix3(GL.GetUniformLocation((uint)s.Number, name), 1, false, m.ArrayF);
	}

	public override void SetFloatUniform(ShaderObject p, string name, Matrix4 m)
	{
		BindWindow();
		if (name.Length == 0) return;
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.UniformMatrix4(GL.GetUniformLocation((uint)s.Number, name), 1, false, m.ArrayF);
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
	// TODO Maybe don't clear the stencil buffer unless the caller actually wants that?
	public override void Clear()
	{
		BindWindow();
		GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
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

	public override void UnbindBuffer(CBufferTarget target)
	{
		BindWindow();
		GL.BindBuffer(target switch
		{
			CBufferTarget.VertexArray => BufferTarget.ArrayBuffer,
			CBufferTarget.ElementArray => BufferTarget.ElementArrayBuffer,
			_ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
		}, 0);
	}

	public override void UnbindTexture([Range(0, 31)] int number)
	{
		BindWindow();
		GL.ActiveTexture(TextureUnit.Texture0 + number);
		GL.BindTexture(TextureTarget.Texture2D, 0);
	}

	public override void UnbindFramebuffer()
	{
		BindWindow();
		GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}

	public override void UnbindShader()
	{
		BindWindow();
		GL.UseProgram(0);
	}

	private class BindingsContext : IBindingsContext
	{
		public IntPtr GetProcAddress(string procName)
		{
			return GLFW.GetProcAddress(procName);
		}
	}
}