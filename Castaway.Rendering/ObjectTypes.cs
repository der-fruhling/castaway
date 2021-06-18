using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Castaway.Rendering
{
    public class RenderObjectInvalidException : ApplicationException
    {
        public RenderObjectInvalidException(MemberInfo type, string name) : base($"{type.Name}:{name}") { }
    }
    
    public abstract class RenderObject : IDisposable
    {
        public abstract void Dispose();
        public abstract string Name { get; }
        public abstract bool Valid { get; }

        public void CheckValid()
        {
            if (!Valid) throw new RenderObjectInvalidException(GetType(), Name);
        }

        public abstract void Bind();
        public abstract void Unbind();
    }

    public abstract class SeparatedShaderObject : RenderObject
    {
        public readonly ShaderStage Stage;
        public readonly string SourceCode;
        public readonly string SourceLocation;

        protected SeparatedShaderObject(ShaderStage stage, string sourceCode, string sourceLocation)
        {
            Stage = stage;
            SourceCode = sourceCode;
            SourceLocation = sourceLocation;
        }

        public sealed override void Bind() => throw new NotSupportedException();
        public sealed override void Unbind() => throw new NotSupportedException();
    }

    public abstract class ShaderObject : RenderObject
    {
        public SeparatedShaderObject[] Shaders { get; }

        private readonly Dictionary<string, VertexInputType> _inputs = new();
        private readonly Dictionary<string, uint> _outputs = new();
        private readonly Dictionary<string, UniformType> _uniforms = new();

        protected ShaderObject(SeparatedShaderObject[] shaders)
        {
            Shaders = shaders;
        }

        public virtual void RegisterInput(string name, VertexInputType input) => _inputs.Add(name, input);
        public virtual void RegisterOutput(string name, uint colorNumber) => _outputs.Add(name, colorNumber);
        public virtual void RegisterUniform(string name, UniformType type) => _uniforms.Add(name, type);

        public string[] GetInputs() => _inputs.Keys.ToArray();
        public string[] GetOutputs() => _outputs.Keys.ToArray();
        public string[] GetRegisteredUniforms() => _uniforms.Keys.ToArray();

        public VertexInputType GetInput(string name) => _inputs.ContainsKey(name)
            ? _inputs[name]
            : throw new InvalidOperationException($"Input {name} not registered.");

        public uint GetOutput(string name) => _outputs.ContainsKey(name)
            ? _outputs[name]
            : throw new InvalidOperationException($"Output {name} not registered.");

        public UniformType GetUniform(string name) => _uniforms.ContainsKey(name)
            ? _uniforms[name]
            : throw new InvalidOperationException($"Output {name} not registered.");

        public abstract void Link();

        public string? GetUniform(UniformType type) => _uniforms.Keys.SingleOrDefault(n => GetUniform(n) == type);
        public string? GetUniform(UniformType type, int i) => GetUniform(type)?.Replace("$INDEX", i.ToString());
    }

    public abstract class BufferObject : RenderObject
    {
        public abstract void Upload(IEnumerable<byte> bytes);

        protected IEnumerable<byte>? RealData = null;

        public IEnumerable<byte> Data
        {
            get => RealData ?? ImmutableArray<byte>.Empty;
            set => Upload(value);
        }

        public virtual void Upload(IEnumerable<uint> uints)
        {
            Upload(BitConverter.IsLittleEndian
                ? uints.SelectMany(BitConverter.GetBytes)
                : uints.SelectMany(BitConverter.GetBytes).Reverse());
        }

        public virtual void Upload(IEnumerable<int> ints)
        {
            Upload(BitConverter.IsLittleEndian
                ? ints.SelectMany(BitConverter.GetBytes)
                : ints.SelectMany(BitConverter.GetBytes).Reverse());
        }

        public virtual void Upload(IEnumerable<float> floats)
        {
            Upload(BitConverter.IsLittleEndian
                ? floats.SelectMany(BitConverter.GetBytes)
                : floats.SelectMany(BitConverter.GetBytes).Reverse());
        }

        public virtual void Upload(IEnumerable<double> doubles)
        {
            Upload(BitConverter.IsLittleEndian
                ? doubles.SelectMany(BitConverter.GetBytes)
                : doubles.SelectMany(BitConverter.GetBytes).Reverse());
        }

        public virtual void Upload(params int[] ints) => Upload(ints as IEnumerable<int>);
        public virtual void Upload(params uint[] uints) => Upload(uints as IEnumerable<uint>);
        public virtual void Upload(params float[] floats) => Upload(floats as IEnumerable<float>);
        public virtual void Upload(params double[] doubles) => Upload(doubles as IEnumerable<double>);
        public virtual void Upload(params int[][] intses) => Upload(intses.SelectMany(a => a));
        public virtual void Upload(params uint[][] uintses) => Upload(uintses.SelectMany(a => a));
        public virtual void Upload(params float[][] floatses) => Upload(floatses.SelectMany(a => a));
        public virtual void Upload(params double[][] doubleses) => Upload(doubleses.SelectMany(a => a));
    }

    public abstract class TextureObject : RenderObject
    {
        public abstract void Bind(int slot);
        public abstract void Unbind(int slot);

        public sealed override void Bind() => throw new NotSupportedException();
        public sealed override void Unbind() => throw new NotSupportedException();
    }

    public abstract class FramebufferObject : RenderObject
    {
        public TextureObject? Color;
        public TextureObject? Stencil;
        public TextureObject? Depth;
    }
}