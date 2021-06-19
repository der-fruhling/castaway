using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Castaway.Assets;
using Castaway.Rendering;

namespace Castaway.OpenGL
{
    [Loads("shdr")]
    public class ShaderAssetType : XMLAssetType
    {
        private enum Ordering
        {
            Version,
            Start,
            End,
            Structs,
            VarConsts,
            VarInputs,
            VarOutputs,
            VarUniforms,
            Variables,
            Functions
        }

        private enum Transform
        {
            Perspective,
            View,
            Model
        }

        private struct Struct
        {
            public Dictionary<string, UniformType> Uniforms;

            public Struct(Dictionary<string, UniformType> uniforms)
            {
                Uniforms = uniforms;
            }
        }
        
        private static readonly string[] GlobalNodeTypes =
        {
            "input", 
            "output", 
            "uniform", 
            "uniform-from", 
            "transform", 
            "var", 
            "const",
            "array"
        };

        private static readonly Dictionary<VertexInputType, string> VertexInputTypeTypes = new()
        {
            [VertexInputType.PositionXY] = "vec2",
            [VertexInputType.PositionXYZ] = "vec3",
            [VertexInputType.ColorG] = "float",
            [VertexInputType.ColorRGB] = "vec3",
            [VertexInputType.ColorRGBA] = "vec4",
            #pragma warning disable 618
            [VertexInputType.ColorBGRA] = "vec4",
            #pragma warning restore 618
            [VertexInputType.NormalXY] = "vec2",
            [VertexInputType.NormalXYZ] = "vec3",
            [VertexInputType.TextureS] = "float",
            [VertexInputType.TextureST] = "vec2",
            [VertexInputType.TextureSTV] = "vec3"
        };
        
        public override T To<T>(Asset a)
        {
            if (typeof(T) == typeof(ShaderObject))
                return (T) (dynamic) LoadOpenGL(base.To<XmlDocument>(a), a.Index);
            return base.To<T>(a);
        }

        public static ShaderObject LoadOpenGL(string str, string path)
        {
            var doc = new XmlDocument();
            doc.LoadXml(str);
            return LoadOpenGL(doc, path);
        }

        public static ShaderObject LoadOpenGL(XmlDocument doc, string path)
        {
            var g = Graphics.Current;
            var root = doc.DocumentElement;
            if (root == null) throw new InvalidOperationException("Document needs root element.");
            if (root.GetElementsByTagName("vertex").Count < 1)
                throw new InvalidOperationException("Need at least one vertex shader.");
            if (root.GetElementsByTagName("fragment").Count < 1)
                throw new InvalidOperationException("Need at least one fragment shader.");

            var shaders = new List<ShaderPart>();
            var inputs = new Dictionary<string, VertexInputType>();
            var uniforms = new Dictionary<string, UniformType>();
            var transforms = new Dictionary<string, Transform>();
            var outputs = new Dictionary<string, uint>();

            var enumerator = root.ChildNodes;
            for (int i = 0; i < enumerator.Count; i++)
            {
                var xml = enumerator.Item(i);
                if (xml is not XmlElement element)
                    throw new InvalidOperationException("All shaders need to be element nodes.");
                var code = new Dictionary<Ordering, List<string>>();
                foreach (var e in Enum.GetValues<Ordering>())
                    code[e] = new List<string>();
                var shaderEnumerator = element.ChildNodes;
                for (int j = 0; j < shaderEnumerator.Count; j++)
                {
                    var e = shaderEnumerator.Item(j) as XmlElement;
                    if (GlobalNodeTypes.Any(e!.Name.Equals))
                    {
                        var qual = string.Join(' ', e.GetAttribute("qual").Split(','));
                        if (qual.Any()) qual += ' ';
                        switch (e.Name)
                        {
                            case "input" when xml.Name == "vertex":
                            {
                                var vtx = Enum.Parse<VertexInputType>(e.GetAttribute("from"));
                                inputs[e.GetAttribute("name")] = vtx;
                                code[Ordering.VarInputs].Add(qual +
                                                             "in " +
                                                             $"{VertexInputTypeTypes[vtx]} " +
                                                             $"{e.GetAttribute("name")};\n");
                                break;
                            }
                            case "input" when xml.Name == "fragment":
                                code[Ordering.VarInputs].Add(qual +
                                                             "in " +
                                                             $"{e.GetAttribute("type")} " +
                                                             $"{e.GetAttribute("name")};\n");
                                break;
                            case "output" when xml.Name == "vertex":
                                code[Ordering.VarOutputs].Add(qual +
                                                              "out " +
                                                              $"{e.GetAttribute("type")} " +
                                                              $"{e.GetAttribute("name")};\n");
                                break;
                            case "output" when xml.Name == "fragment":
                                outputs[e.GetAttribute("name")] = uint.Parse(e.GetAttribute("to"));
                                code[Ordering.VarOutputs].Add(qual +
                                                              "out " +
                                                              "vec4 " +
                                                              $"{e.GetAttribute("name")};\n");
                                break;
                            case "uniform" when e.InnerText.Any():
                                code[Ordering.VarUniforms].Add(qual +
                                                               "uniform " +
                                                               $"{e.GetAttribute("type")} " +
                                                               $"{e.GetAttribute("name")} = " +
                                                               $"{e.InnerText};\n");
                                goto case "uniform-from";
                            case "uniform" when !e.InnerText.Any():
                                code[Ordering.VarUniforms].Add(qual +
                                                               "uniform " +
                                                               $"{e.GetAttribute("type")} " +
                                                               $"{e.GetAttribute("name")};\n");
                                goto case "uniform-from";
                            case "uniform-from":
                                if (Enum.TryParse<UniformType>(e.GetAttribute("from"), out var u))
                                    uniforms[e.GetAttribute("name")] = u;
                                else if (!e.GetAttribute("from").Any())
                                    uniforms[e.GetAttribute("name")] = UniformType.Custom;
                                else
                                    throw new InvalidOperationException(
                                        $"Invalid uniform `from` attribute: {e.GetAttribute("from")}");

                                break;
                            case "transform":
                                transforms[e.GetAttribute("name")] = Enum.Parse<Transform>(e.GetAttribute("matrix"));
                                code[Ordering.VarUniforms].Add(qual +
                                                               $"uniform mat4 {e.GetAttribute("name")} = mat4(\n" +
                                                               "    1, 0, 0, 0,\n" +
                                                               "    0, 1, 0, 0,\n" +
                                                               "    0, 0, 1, 0,\n" +
                                                               "    0, 0, 0, 1);\n");
                                break;
                            case "const":
                                code[Ordering.VarConsts].Add(qual +
                                                              "const " +
                                                             $"{e.GetAttribute("type")} " +
                                                             $"{e.GetAttribute("name")}" +
                                                             $" = {e.InnerText};");
                                break;
                            case "var" when e.InnerText.Any():
                                code[Ordering.Variables].Add(qual +
                                                             $"{e.GetAttribute("type")} " +
                                                             $"{e.GetAttribute("name")} = " +
                                                             $"{e.InnerText};");
                                break;
                            case "var" when !e.InnerText.Any():
                                code[Ordering.Variables].Add(qual +
                                                             $"{e.GetAttribute("type")} " +
                                                             $"{e.GetAttribute("name")};");
                                break;
                            case "array" when e.InnerText.Any():
                                code[Ordering.Variables].Add(qual +
                                                             $"{e.GetAttribute("type")} " +
                                                             $"{e.GetAttribute("name")}" +
                                                             $"[{e.GetAttribute("size")}] = " +
                                                             $"{e.GetAttribute("type")}[](" +
                                                             $"{e.InnerText}" +
                                                             $");");
                                break;
                            case "array" when !e.InnerText.Any():
                                code[Ordering.Variables].Add(qual +
                                                             $"{e.GetAttribute("type")} " +
                                                             $"{e.GetAttribute("name")}" +
                                                             $"[{e.GetAttribute("size")}];");
                                break;
                        }
                    }
                    else if (e.Name == "glsl")
                    {
                        var parts = e.InnerText.Split('\n').Select(s => s.Trim());
                        code[Enum.Parse<Ordering>(e.GetAttribute("order"))].Add(string.Join('\n', parts) + '\n');
                    }
                    else if (e.Name == "function")
                    {
                        var parts = e.InnerText.Split('\n').Select(s => s.Trim());
                        code[Ordering.Functions].Add(string.Join('\n', parts) + "\n\n");
                    }
                    else if (e.Name == "struct")
                    {
                        var children = e.GetElementsByTagName("member");
                        var result = $"struct {e.GetAttribute("name")}\n{{\n";

                        if (children.Count == 0) result += "    /* empty */ float _;\n";
                        else
                        {
                            for (int k = 0; k < children.Count; k++)
                            {
                                var c = children.Item(k) as XmlElement;
                                result += $"    {c!.GetAttribute("type")} {c.GetAttribute("name")};\n";
                            }
                        }

                        result += "};";
                        code[Ordering.Structs].Add(result);
                    }
                }

                var str = "";
                str += code[Ordering.Version].Aggregate("", (a, s) => a + s) + '\n';
                str += "// Castaway Generated\n";
                str += code[Ordering.Start].Aggregate("", (a, s) => a + s) + '\n';
                str += "// Structures\n";
                str += code[Ordering.Structs].Aggregate("", (a, s) => a + s) + '\n';
                str += "// Constants\n";
                str += code[Ordering.VarConsts].Aggregate("", (a, s) => a + s) + '\n';
                str += "// Inputs\n";
                str += code[Ordering.VarInputs].Aggregate("", (a, s) => a + s) + '\n';
                str += "// Outputs\n";
                str += code[Ordering.VarOutputs].Aggregate("", (a, s) => a + s) + '\n';
                str += "// Uniforms\n";
                str += code[Ordering.VarUniforms].Aggregate("", (a, s) => a + s) + '\n';
                str += "// Variables\n";
                str += code[Ordering.Variables].Aggregate("", (a, s) => a + s) + '\n';
                str += "// Functions\n";
                str += code[Ordering.Functions].Aggregate("", (a, s) => a + s);
                str += code[Ordering.End].Aggregate("", (a, s) => a + s);

                var sha = SHA1
                    .HashData(Encoding.UTF8.GetBytes(str))
                    .Select(b => b.ToString("x2"));
                var data = $"// SHA: {sha}\n{str}";
                foreach (var (n, v) in inputs)
                    data = data.Insert(0, $"// Input {n} from {v}\n");
                foreach (var (n, u) in uniforms)
                    data = data.Insert(0, $"// Uniform {n} bound to {u}\n");
                foreach (var (n, u) in outputs)
                    data = data.Insert(0, $"// Output {n} writes to {u}\n");
                
                try
                {
                    shaders.Add(new ShaderPart(xml.Name switch
                    {
                        "vertex" => ShaderStage.Vertex,
                        "fragment" => ShaderStage.Fragment,
                        _ => throw new ArgumentOutOfRangeException(null, $"Invalid shader type {xml.Name}")
                    }, data, path));
                }
                catch (GraphicsException)
                {
                    Console.WriteLine("Castaway Generated Shader Source:");
                    var split = data.Split('\n');
                    for (var index = 0; index < split.Length; index++)
                        Console.WriteLine($"{index + 1}\t{split[index]}");

                    throw;
                }
            }

            foreach (var (n, t) in transforms)
            {
                uniforms[n] = t switch
                {
                    Transform.Perspective => UniformType.TransformPerspective,
                    Transform.View => UniformType.TransformView,
                    Transform.Model => UniformType.TransformModel,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            
            var program = new Shader(shaders.Cast<SeparatedShaderObject>().ToArray());
            foreach(var (n, v) in inputs) program.RegisterInput(n, v);
            foreach(var (n, c) in outputs) program.RegisterOutput(n, c);
            foreach(var (n, u) in uniforms) program.RegisterUniform(n, u);
            program.Link();

            return program;
        }
    }
}