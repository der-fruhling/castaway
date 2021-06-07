using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castaway.Math;
using Castaway.Rendering;

namespace Castaway.OpenGL
{
    [SuppressMessage("ReSharper", "CA2211")]
    public static class BuiltinShaders
    {
        public static ShaderProgram Default;
        public static ShaderProgram DefaultTextured;
        public static ShaderProgram NoTransform;
        public static ShaderProgram NoTransformTextured;

        public static void Init()
        {
            #region Mess
            var taskList = Assembly.GetExecutingAssembly().GetManifestResourceNames()
                .Where(s => s.StartsWith("Castaway.OpenGL._shaders."))
                .Select(s => s.Replace("Castaway.OpenGL._shaders.", ""))
                .Select(s => (s, s.Split('.', 2)))
                .Select(a => ($"{a.Item2[0]}/{a.Item2[1]}", ReadShader(a.Item1)))
                .Aggregate(
                    new Dictionary<string, Task<string>>(), 
                    (dict, t) =>
                    {
                        var (name, task) = t;
                        dict[name] = task;
                        return dict;
                    });
            #endregion

            var g = OpenGL.Get();

            Shader vs, fs;
            ShaderProgram p;
            
            // `default`
            vs = g.CreateShader(ShaderStage.Vertex, taskList["default/vertex.glsl"].Result);
            fs = g.CreateShader(ShaderStage.Fragment, taskList["default/fragment.glsl"].Result);
            p = g.CreateProgram(vs, fs);
            g.CreateInput(p, VertexInputType.PositionXYZ, "vPosition");
            g.CreateInput(p, VertexInputType.ColorRGB, "vColor");
            g.CreateOutput(p, 0, "oColor");
            g.BindUniform(p, "tPersp", UniformType.TransformPerspective);
            g.BindUniform(p, "tView", UniformType.TransformView);
            g.BindUniform(p, "tModel", UniformType.TransformModel);
            g.FinishProgram(ref p);
            g.SetUniform(p, UniformType.TransformPerspective, Matrix4.Ident);
            g.SetUniform(p, UniformType.TransformView, Matrix4.Ident);
            g.SetUniform(p, UniformType.TransformModel, Matrix4.Ident);
            Default = p;
            
            // `default-tex`
            vs = g.CreateShader(ShaderStage.Vertex, taskList["default_tex/vertex.glsl"].Result);
            fs = g.CreateShader(ShaderStage.Fragment, taskList["default_tex/fragment.glsl"].Result);
            p = g.CreateProgram(vs, fs);
            g.CreateInput(p, VertexInputType.PositionXYZ, "vPosition");
            g.CreateInput(p, VertexInputType.ColorRGB, "vColor");
            g.CreateInput(p, VertexInputType.TextureST, "vTextureCoords");
            g.CreateOutput(p, 0, "oColor");
            g.BindUniform(p, "tPersp", UniformType.TransformPerspective);
            g.BindUniform(p, "tView", UniformType.TransformView);
            g.BindUniform(p, "tModel", UniformType.TransformModel);
            g.BindUniform(p, "uTexture");
            g.FinishProgram(ref p);
            g.SetUniform(p, UniformType.TransformPerspective, Matrix4.Ident);
            g.SetUniform(p, UniformType.TransformView, Matrix4.Ident);
            g.SetUniform(p, UniformType.TransformModel, Matrix4.Ident);
            DefaultTextured = p;
            
            // `notransform`
            vs = g.CreateShader(ShaderStage.Vertex, taskList["notransform/vertex.glsl"].Result);
            fs = g.CreateShader(ShaderStage.Fragment, taskList["default/fragment.glsl"].Result);
            p = g.CreateProgram(vs, fs);
            g.CreateInput(p, VertexInputType.PositionXYZ, "vPosition");
            g.CreateInput(p, VertexInputType.ColorRGB, "vColor");
            g.CreateOutput(p, 0, "oColor");
            g.FinishProgram(ref p);
            NoTransform = p;
            
            // `notransform-tex`
            vs = g.CreateShader(ShaderStage.Vertex, taskList["notransform_tex/vertex.glsl"].Result);
            fs = g.CreateShader(ShaderStage.Fragment, taskList["default_tex/fragment.glsl"].Result);
            p = g.CreateProgram(vs, fs);
            g.CreateInput(p, VertexInputType.PositionXYZ, "vPosition");
            g.CreateInput(p, VertexInputType.ColorRGB, "vColor");
            g.CreateInput(p, VertexInputType.TextureST, "vTextureCoords");
            g.CreateOutput(p, 0, "oColor");
            g.FinishProgram(ref p);
            NoTransformTextured = p;
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