using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Castaway.Rendering;

namespace Castaway.OpenGL
{
    public class GLProgram : IProgram, IOpenGLObject
    {
        internal Dictionary<string, VertexInputType> VertexInputsI = new();
        internal Dictionary<string, uint> FragmentOutputsI = new();
        internal Dictionary<string, UniformType> UniformsI = new();
        internal Dictionary<string, int> UniformLocations = new();

        public bool IsValid => Validate();
        public IShader[] Shaders { get; internal set; }

        public ReadOnlyDictionary<string, VertexInputType> VertexInputs => new(VertexInputsI);
        public ReadOnlyDictionary<string, uint> FragmentOutputs => new(FragmentOutputsI);
        public ReadOnlyDictionary<string, UniformType> Uniforms => new(UniformsI);
        public uint Number { get; set; }
        public OpenGLType Type => OpenGLType.ShaderProgram;

        public bool IsLinked { get; internal set; }
        public bool HasVertexShader => Shaders.Any(s => s.IsVertexShader());
        public bool HasFragmentShader => Shaders.Any(s => s.IsFragmentShader());

        private bool Revalidate = true;
        private bool LastValidate;

        public GLProgram(uint number)
        {
            Number = number;
        }

        public bool Validate()
        {
            if (!Revalidate) return LastValidate;
            Revalidate = false;
            return LastValidate = GL.IsProgram(Number);
        }

        public void MarkDirty()
        {
            Revalidate = true;
        }
    }
}