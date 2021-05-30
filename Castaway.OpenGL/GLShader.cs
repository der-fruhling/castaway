using System;
using Castaway.Rendering;

namespace Castaway.OpenGL
{
    public class GLShader : IShader, IOpenGLObject
    {
        public bool IsValid => Validate();
        public uint Number { get; set; }
        public OpenGLType Type => OpenGLType.Shader;
        
        public ShaderStage Stage => GL.ValueEnum<GL.ShaderStage>((uint) GL.GetShader(Number, GL.ShaderQuery.ShaderType)) switch
        {
            GL.ShaderStage.VertexShader => ShaderStage.Vertex,
            GL.ShaderStage.FragmentShader => ShaderStage.Fragment,
            _ => throw new ArgumentOutOfRangeException()
        };

        public GLShader(uint number)
        {
            Number = number;
        }

        public bool Validate() => GL.IsShader(Number);
        
        
        public bool IsVertexShader() => Stage == ShaderStage.Vertex;
        public bool IsFragmentShader() => Stage == ShaderStage.Fragment;
    }
}