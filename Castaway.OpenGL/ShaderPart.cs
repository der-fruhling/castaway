using System;
using System.Linq;
using Castaway.Assets;
using Castaway.Base;
using Castaway.OpenGL.Native;
using Castaway.Rendering;
using Serilog;

namespace Castaway.OpenGL
{
    internal sealed class ShaderPart : SeparatedShaderObject
    {
        public bool Destroyed { get; set; }
        public uint Number { get; set; }
        public bool CompileSuccess => GL.GetShader(Number, GL.ShaderQuery.CompileStatus) == 1;
        public override string Name => $"{Number}->{Stage}({Valid})";
        public override bool Valid => CompileSuccess && !Destroyed;
        private static readonly ILogger Logger = CastawayGlobal.GetLogger();

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
            Number = (Graphics.Current as OpenGLImpl)?.NewShader(stage) ?? throw new InvalidOperationException($"Bad shader stage {stage} for OpenGL");
            
            GL.ShaderSource(Number, sourceCode);
            GL.CompileShader(Number);
            
            GL.GetShaderInfoLog(Number, out _, out var log);
            if (log.Any())
            {
                Logger.Warning("Shader Log ({Stage} @ {Location})", stage, sourceLocation);
                var lines = log.Split('\n');
                foreach(var l in lines) Logger.Warning("{Line}", l.Trim());
            }
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