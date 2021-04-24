using System.Collections.Generic;
using System.IO;
using Castaway.Assets;
using Castaway.Render;

namespace PirateSLC
{
    // ReSharper disable once InconsistentNaming
    public class PirateSLAssetLoader : IAssetLoader
    {
        public IEnumerable<string> FileExtensions { get; } = new[] {"psl"};
        
        public object LoadFile(string path)
        {
            Compiler.Compile(File.ReadAllText(path), out var vertSrc, out var fragSrc, out var confSrc);
            
            File.WriteAllText("output_vert.glsl", vertSrc);
            File.WriteAllText("output_frag.glsl", fragSrc);
            File.WriteAllText("output_conf.csh", confSrc);
            
            var vertAttrs = new Dictionary<string, VertexAttribInfo.AttribValue>();
            var fragOutputs = new Dictionary<string, uint>();
            var model = "";
            var view = "";
            var projection = "";
            var properties = new Dictionary<string, string>();
            
            GLSLShaderAssetLoader.Configure(confSrc, vertAttrs, fragOutputs, ref model, ref view, ref projection, properties);
            return new LoadedShader(vertAttrs, fragOutputs, vertSrc, fragSrc, model, view, projection, properties);
        }
    }
}