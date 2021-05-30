using System.Collections.ObjectModel;

namespace Castaway.Rendering
{
    public interface IProgram
    {
        bool IsValid { get; }
        IShader[] Shaders { get; }
        ReadOnlyDictionary<string, VertexInputType> VertexInputs { get; }
        ReadOnlyDictionary<string, uint> FragmentOutputs { get; }
        ReadOnlyDictionary<string, UniformType> Uniforms { get; }
        
        bool IsLinked { get; }
        bool HasVertexShader { get; }
        bool HasFragmentShader { get; }
    }
}