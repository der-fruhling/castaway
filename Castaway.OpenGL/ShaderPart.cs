using System;
using System.Linq;
using Castaway.Assets;
using Castaway.Base;
using Castaway.Rendering;
using Castaway.Rendering.Objects;
using Castaway.Rendering.Shaders;
using OpenTK.Graphics.OpenGL;
using Serilog;

namespace Castaway.OpenGL;

internal sealed class ShaderPart : SeparatedShaderObject
{
	private static readonly ILogger Logger = CastawayGlobal.GetLogger();

	public ShaderPart(ShaderStage stage, string sourceCode, string sourceLocation) : base(stage, sourceCode,
		sourceLocation)
	{
		Number = GL.CreateShader(stage switch
		{
			ShaderStage.Vertex => ShaderType.VertexShader,
			ShaderStage.Fragment => ShaderType.FragmentShader,
			_ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
		});

		GL.ShaderSource(Number, sourceCode);
		GL.CompileShader(Number);

		GL.GetShaderInfoLog(Number, out var log);
		if (log!.Any())
		{
			Logger.Warning("Shader Log ({Stage} @ {Location})", stage, sourceLocation);
			var lines = log!.Split('\n');
			foreach (var l in lines) Logger.Warning("{Line}", l.Trim());
		}

		if (!CompileSuccess) throw new GraphicsException($"Failed to compile {stage} shader");
	}

	public ShaderPart(ShaderStage stage, Asset asset) : this(stage, asset.Read<string>(), asset.Index)
	{
	}

	public bool Destroyed { get; set; }
	public int Number { get; set; }

	public bool CompileSuccess
	{
		get
		{
			GL.GetShader(Number, ShaderParameter.CompileStatus, out var status);
			return status == 1;
		}
	}

	public override string Name => $"{Number}->{Stage}({Valid})";
	public override bool Valid => CompileSuccess && !Destroyed;

	public string CompileLog
	{
		get
		{
			GL.GetShaderInfoLog(Number, out var ret);
			return ret!;
		}
	}

	public override void Dispose()
	{
		GL.DeleteShader(Number);
	}
}