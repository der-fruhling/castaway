using Castaway.Rendering;
using static Castaway.Rendering.IGraphicsObject;

namespace Castaway.OpenGL
{
    public struct Shader : IOpenGLObject
    {
        public ObjectType Type => ObjectType.Shader;
        public bool Destroyed { get; set; }
        public uint Number { get; set; }

        public ShaderStage Stage;
        public string SourceCode { get; internal set; }
        public string[] SourceLines => SourceCode.Split('\n');
        public bool CompileSuccess => GL.GetShader(Number, GL.ShaderQuery.CompileStatus) == 1;

        public string CompileLog
        {
            get
            {
                GL.GetShaderInfoLog(Number, out _, out var ret);
                return ret;
            }
        }
    }
}