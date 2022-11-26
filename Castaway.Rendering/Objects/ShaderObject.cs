using System;
using System.Collections.Generic;
using System.Linq;
using Castaway.Rendering.Shaders;

namespace Castaway.Rendering.Objects;

public abstract class ShaderObject : RenderObject, IValidatable, IBindable
{
	private readonly Dictionary<string, VertexInputType> _inputs = new();
	private readonly Dictionary<string, uint> _outputs = new();
	private readonly Dictionary<string, UniformType> _uniforms = new();

	protected ShaderObject(SeparatedShaderObject[] shaders)
	{
		Shaders = shaders;
	}

	public SeparatedShaderObject[] Shaders { get; }

	public abstract void Bind();
	public abstract void Unbind();

	public abstract bool Valid { get; }

	public virtual void RegisterInput(string name, VertexInputType input)
	{
		_inputs.Add(name, input);
	}

	public virtual void RegisterOutput(string name, uint colorNumber)
	{
		_outputs.Add(name, colorNumber);
	}

	public virtual void RegisterUniform(string name, UniformType type)
	{
		_uniforms.Add(name, type);
	}

	public string[] GetInputs()
	{
		return _inputs.Keys.ToArray();
	}

	public string[] GetOutputs()
	{
		return _outputs.Keys.ToArray();
	}

	public string[] GetRegisteredUniforms()
	{
		return _uniforms.Keys.ToArray();
	}

	public VertexInputType GetInput(string name)
	{
		return _inputs.ContainsKey(name)
			? _inputs[name]
			: throw new InvalidOperationException($"Input {name} not registered.");
	}

	public uint GetOutput(string name)
	{
		return _outputs.ContainsKey(name)
			? _outputs[name]
			: throw new InvalidOperationException($"Output {name} not registered.");
	}

	public UniformType GetUniform(string name)
	{
		return _uniforms.ContainsKey(name)
			? _uniforms[name]
			: throw new InvalidOperationException($"Output {name} not registered.");
	}

	public abstract void Link();

	public string? GetUniform(UniformType type)
	{
		return _uniforms.Keys.SingleOrDefault(n => GetUniform(n) == type);
	}

	public string? GetUniform(UniformType type, int i)
	{
		return GetUniform(type)?.Replace("$INDEX", i.ToString());
	}
}