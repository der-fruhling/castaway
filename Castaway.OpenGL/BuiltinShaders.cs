using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castaway.Base;
using Castaway.Rendering;
using Serilog;

namespace Castaway.OpenGL
{
    [SuppressMessage("ReSharper", "CA2211")]
    public static class BuiltinShaders
    {
        private static readonly ILogger Logger = CastawayGlobal.GetLogger();
        
        // ReSharper disable InconsistentNaming
        public static ShaderObject? Default;
        public static ShaderObject? DefaultTextured;
        public static ShaderObject? Direct;
        public static ShaderObject? DirectTextured;
        public static ShaderObject? UIUnscaled;
        public static ShaderObject? UIScaled;
        public static ShaderObject? UIUnscaledTextured;
        public static ShaderObject? UIScaledTextured;
        // ReSharper restore InconsistentNaming

        public static void Init()
        {
            #region Mess
            var taskList = Assembly.GetExecutingAssembly().GetManifestResourceNames()
                .Where(s => s.StartsWith("Castaway.OpenGL._shaders."))
                .Select(s => s.Replace("Castaway.OpenGL._shaders.", ""))
                .Select(s => (string.Join('/', s.Split('.')[..^1]), ReadShader(s)))
                .Aggregate(
                    new Dictionary<string, Task<string>>(), 
                    (dict, t) =>
                    {
                        var (name, task) = t;
                        dict[name] = task;
                        return dict;
                    });
            #endregion
            
            Logger.Debug("Compiling builtin shaders");
            Default = ShaderAssetType.LoadOpenGL(taskList["default/normal"].Result, "b:default/normal");
            DefaultTextured = ShaderAssetType.LoadOpenGL(taskList["default/textured"].Result, "b:default/textured");
            Direct = ShaderAssetType.LoadOpenGL(taskList["direct/normal"].Result, "b:direct/normal");
            DirectTextured = ShaderAssetType.LoadOpenGL(taskList["direct/textured"].Result, "b:direct/textured");
            UIScaled = ShaderAssetType.LoadOpenGL(taskList["ui/scaled-normal"].Result, "b:ui/scaled-normal");
            UIUnscaled = ShaderAssetType.LoadOpenGL(taskList["ui/unscaled-normal"].Result, "b:ui/unscaled-normal");
            UIScaledTextured = ShaderAssetType.LoadOpenGL(taskList["ui/scaled-textured"].Result, "b:ui/scaled-textured");
            UIUnscaledTextured = ShaderAssetType.LoadOpenGL(taskList["ui/unscaled-textured"].Result, "b:ui/unscaled-textured");
            Logger.Information("Compiled builtin shaders");
        }

        public static void Destroy()
        {
            Logger.Information("Disposing of builtin shaders");
            Default?.Dispose();
            DefaultTextured?.Dispose();
            Direct?.Dispose();
            DirectTextured?.Dispose();
            UIScaled?.Dispose();
            UIUnscaled?.Dispose();
            UIScaledTextured?.Dispose();
            UIUnscaledTextured?.Dispose();
            Logger.Information("Finished disposing of builtin shaders");
        }

        private static async Task<string> ReadShader(string path)
        {
            Logger.Verbose("Reading manifest file at {Path}", path);
            var asm = Assembly.GetExecutingAssembly();
            await using var stream = asm.GetManifestResourceStream($"Castaway.OpenGL._shaders.{path}");
            var reader = new StreamReader(stream!);
            return await reader.ReadToEndAsync();
        }
    }
}