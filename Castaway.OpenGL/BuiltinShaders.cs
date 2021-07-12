using System.IO;
using System.Reflection;
using Castaway.Base;
using Castaway.Rendering;
using Serilog;

namespace Castaway.OpenGL
{
    [ProvidesShadersFor(typeof(OpenGLImpl)),
     ProvidesShadersFor(typeof(OpenGL33)),
     ProvidesShadersFor(typeof(OpenGL40)),
     ProvidesShadersFor(typeof(OpenGL41)),
     ProvidesShadersFor(typeof(OpenGL42))]
    internal class BuiltinShaders : IShaderProvider
    {
        private static readonly ILogger Logger = CastawayGlobal.GetLogger();

        public ShaderObject CreateDefault(Graphics g) => ReadShader("default.normal.shdr");
        public ShaderObject CreateDefaultTextured(Graphics g) => ReadShader("default.textured.shdr");
        public ShaderObject CreateDirect(Graphics g) => ReadShader("direct.normal.shdr");
        public ShaderObject CreateDirectTextured(Graphics g) => ReadShader("direct.textured.shdr");
        public ShaderObject CreateUIScaled(Graphics g) => ReadShader("ui.scaled.normal.shdr");
        public ShaderObject CreateUIScaledTextured(Graphics g) => ReadShader("ui.scaled.textured.shdr");
        public ShaderObject CreateUIUnscaled(Graphics g) => ReadShader("ui.unscaled.normal.shdr");
        public ShaderObject CreateUIUnscaledTextured(Graphics g) => ReadShader("ui.unscaled.textured.shdr");

        private static ShaderObject ReadShader(string path)
        {
            Logger.Verbose("Searching manifest for {Path}", path);
            var asm = Assembly.GetExecutingAssembly();
            using var stream = asm.GetManifestResourceStream($"Castaway.OpenGL._shaders.{path}");
            var reader = new StreamReader(stream!);
            return ShaderAssetType.LoadOpenGL(reader.ReadToEnd(),
                $"manifest:Castaway.OpenGL:Castaway.OpenGL._shaders.{path}");
        }
    }
}