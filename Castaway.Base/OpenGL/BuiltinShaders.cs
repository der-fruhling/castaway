using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castaway.Assets;

namespace Castaway.OpenGL
{
    [SuppressMessage("ReSharper", "CA2211")]
    public static class BuiltinShaders
    {
        public static ShaderProgram Default;
        public static ShaderProgram DefaultTextured;
        public static ShaderProgram Direct;
        public static ShaderProgram DirectTextured;

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
            
            Default = ShaderAssetType.LoadOpenGL(taskList["default/normal"].Result);
            DefaultTextured = ShaderAssetType.LoadOpenGL(taskList["default/textured"].Result);
            Direct = ShaderAssetType.LoadOpenGL(taskList["direct/normal"].Result);
            DirectTextured = ShaderAssetType.LoadOpenGL(taskList["direct/textured"].Result);
        }

        public static void Destroy()
        {
            var g = OpenGL.Get();
            g.Destroy(Default);
        }

        private static async Task<string> ReadShader(string path)
        {
            var asm = Assembly.GetExecutingAssembly();
            await using var stream = asm.GetManifestResourceStream($"Castaway.OpenGL._shaders.{path}");
            var reader = new StreamReader(stream!);
            return await reader.ReadToEndAsync();
        }
    }
}