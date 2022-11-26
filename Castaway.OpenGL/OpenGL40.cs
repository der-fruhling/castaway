using System;
using Castaway.Math;
using Castaway.Rendering;
using Castaway.Rendering.Objects;
using OpenTK.Graphics.OpenGL;

namespace Castaway.OpenGL;

[Implements("OpenGL-4.0")]
public class OpenGL40 : OpenGL33
{
	public override string Name => "OpenGL-4.0";

	public override void SetDoubleUniform(ShaderObject p, string name, double i)
	{
		BindWindow();
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.Uniform1(GL.GetUniformLocation(s.Number, name), 1, new[] { i });
	}

	public override void SetDoubleUniform(ShaderObject p, string name, double x, double y)
	{
		BindWindow();
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.Uniform2(GL.GetUniformLocation(s.Number, name), 1, new[] { x, y });
	}

	public override void SetDoubleUniform(ShaderObject p, string name, double x, double y, double z)
	{
		BindWindow();
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.Uniform3(GL.GetUniformLocation(s.Number, name), 1, new[] { x, y, z });
	}

	public override void SetDoubleUniform(ShaderObject p, string name, double x, double y, double z, double w)
	{
		BindWindow();
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.Uniform3(GL.GetUniformLocation(s.Number, name), 1, new[] { x, y, z, w });
	}

	public override void SetDoubleUniform(ShaderObject p, string name, Matrix2 m)
	{
		BindWindow();
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.UniformMatrix2(GL.GetUniformLocation(s.Number, name), 1, false, m.Array);
	}

	public override void SetDoubleUniform(ShaderObject p, string name, Matrix3 m)
	{
		BindWindow();
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.UniformMatrix3(GL.GetUniformLocation(s.Number, name), 1, false, m.Array);
	}

	public override void SetDoubleUniform(ShaderObject p, string name, Matrix4 m)
	{
		BindWindow();
		if (p is not Shader s)
			throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
		GL.UniformMatrix4(GL.GetUniformLocation(s.Number, name), 1, false, m.Array);
	}
}