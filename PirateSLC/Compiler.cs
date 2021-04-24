#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Castaway.Assets;
using Castaway.PirateSL;
using PirateSLC.Elements;

namespace PirateSLC
{
    public class Compiler
    {
        private static List<string> _errors = new List<string>();

        private static readonly Element[] Elements = {
            new VertexInputElement(),
            new UsesElement(),
            new VariableElement(),
            new CommonVariableElement(),
            new EntrypointStartElement(),
            new BlockEndElement(),
            new FragmentOutputElement(),
            new NewAutoVarElement(),
            new NewVarElement(),
            new SetVarElement(),
            new UniformElement(),
            new UniformPropertyElement(),
            new FunctionStartElement(),
            new FunctionReturnElement(),
            new IfElement(),
            new ElseElement(),
            new SkipElement()
        };

        private readonly struct Line
        {
            public readonly string String;
            public readonly string Original;
            public readonly int LineNumber;

            public Line(string s, string original, int lineNumber)
            {
                String = s;
                Original = original;
                LineNumber = lineNumber;
            }
        }

        private static CodeGenerator? Compile(string data)
        {
            CodeGenerator? generator = null;
            Compile(ref generator, data);
            return _errors.Count > 0 ? null : generator;
        }

        private static void Compile(ref CodeGenerator? generator, string data)
        {
            var lines = Regex.Replace(data, @"/\*.*\*/", " ")
                .Split("\n", StringSplitOptions.RemoveEmptyEntries)
                .Select((s, i) => new Line(s.Trim(), s, i+1))
                .Where(s => s.String.Length > 0 && !s.String.StartsWith("//"));
            var la = lines as Line[] ?? lines.ToArray();
            for (var i = 0; i < la.Length; i++)
            {
                if(la[i].String[0] == '#') ProcessConfiguration(ref generator, la[i]);
                else ProcessStatement(ref generator, la[i]);
            }
            _errors.ForEach(s => Console.Error.WriteLine($"Error: {s}"));
        }

        private static void ProcessConfiguration(ref CodeGenerator? generator, Line line)
        {
            var parts = line.String.Split(' ', 3);
            switch (parts[0])
            {
                case "#output":
                    generator = parts[1] switch
                    {
                        "glsl" => new GLSLGenerator(),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    generator.GenConfiguration(parts[2][1..^1]);
                    break;
                case "#include":
                    Compile(ref generator, File.ReadAllText($"{AssetManager.AssetFolderPath}{parts[1]}"));
                    break;
            }
        }

        private static void ProcessStatement(ref CodeGenerator? generator, Line line)
        {
            if (generator == null) throw new ApplicationException("No output set.");
            
            foreach (var element in Elements)
            {
                if(!element.Matches(line.String)) continue;
                element.SetData(line.String.Split(' ', StringSplitOptions.RemoveEmptyEntries));
                element.Apply(ref generator, _errors);
                return;
            }

            _errors.Add($"[{line.LineNumber}] Invalid statement: \"{line.String}\"");
        }

        public static void Compile(string data, out string vert, out string frag, out string conf)
        {
            var gen = Compile(data) ?? throw new ApplicationException("Shader file failed to compile.");
            var files = new Dictionary<string, string>();
            gen.WriteOut(files);
            vert = files["shader.vsh"];
            frag = files["shader.fsh"];
            conf = files["shader.csh"];
        }

        public static void CompileOut(string data)
        {
            var gen = Compile(data) ?? throw new ApplicationException("Shader file failed to compile.");
            gen.WriteOut();
        }
    }
}