using System;
using System.Linq;
using Castaway.Assets;
using Castaway.Native.GL;
using Castaway.Rendering;

namespace Castaway.OpenGL
{
    public sealed class ShaderPart : SeparatedShaderObject
    {
        public bool Destroyed { get; set; }
        public uint Number { get; set; }
        public bool CompileSuccess => GL.GetShader(Number, GL.ShaderQuery.CompileStatus) == 1;
        public override string Name => $"{Number}->{Stage}({Valid})";
        public override bool Valid => CompileSuccess && !Destroyed;

        public string CompileLog
        {
            get
            {
                GL.GetShaderInfoLog(Number, out _, out var ret);
                return ret;
            }
        }

        public ShaderPart(ShaderStage stage, string sourceCode, string sourceLocation) : base(stage, sourceCode, sourceLocation)
        {
            Number = (Graphics.Current as OpenGL32)?.NewShader(stage) ?? throw new InvalidOperationException($"Bad shader stage {stage} for OpenGL");
            
            GL.ShaderSource(Number, sourceCode);
            GL.CompileShader(Number);
            
            GL.GetShaderInfoLog(Number, out _, out var log);
            if(log.Any()) Console.Error.WriteLine(log);
            if (!CompileSuccess) throw new GraphicsException($"Failed to compile {stage} shader");
        }

        public ShaderPart(ShaderStage stage, Asset asset) : this(stage, asset.To<string>(), asset.Index)
        {
            
        }

        public override void Dispose()
        {
            GL.DeleteShader(Number);
        }
    }
}