using System;
using System.Linq;
using Castaway.Rendering;
using Castaway.Rendering.Objects;
using OpenTK.Graphics.OpenGL;

namespace Castaway.OpenGL;

internal sealed class Shader : ShaderObject
{
	public ShaderInputBinder? Binder;

	public Shader(params SeparatedShaderObject[] shaders) : base(shaders)
	{
		Number = GL.CreateProgram();
		foreach (var separatedShaderObject in shaders)
		{
			if (separatedShaderObject is not ShaderPart s) continue;
			GL.AttachShader(Number, s.Number);
			s.Dispose();
		}
	}

	public bool Destroyed { get; set; }
	public int Number { get; }
	public override string Name => $"{Number}({Valid})";
	public override bool Valid => GL.IsProgram(Number) && !Destroyed;

	public bool LinkSuccess
	{
		get
		{
			GL.GetProgram(Number, GetProgramParameterName.LinkStatus, out var status);
			return status == 1;
		}
	}

	public string LinkLog
	{
		get
		{
			GL.GetProgramInfoLog(Number, out var ret);
			return ret!;
		}
	}

	public override void Bind()
	{
		if (Graphics.Current is not OpenGLImpl) throw new InvalidOperationException("Need OpenGL >= 3.2");
		GL.UseProgram(Number);
		Graphics.Current.BoundShader = this;
	}

	public override void Unbind()
	{
		if (Graphics.Current is not OpenGLImpl) throw new InvalidOperationException("Need OpenGL >= 3.2");
		GL.UseProgram(0);
		Graphics.Current.BoundShader = null;
	}

	public override void Dispose()
	{
		GL.DeleteProgram(Number);
	}

	public override void Link()
	{
		foreach (var o in GetOutputs())
		{
			var c = GetOutput(o);
			GL.BindFragDataLocation(Number, (int)c, o);
		}

		GL.LinkProgram(Number);
		var log = LinkLog;
		if (log.Any()) Console.Error.WriteLine(log);
		if (!LinkSuccess) throw new GraphicsException("Failed to link program.");
		Binder = new ShaderInputBinder(this);
	}
}