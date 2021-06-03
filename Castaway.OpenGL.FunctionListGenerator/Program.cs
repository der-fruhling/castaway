#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Castaway.OpenGL.FunctionListGenerator
{
    
    internal static class Program
    {
        private const string Url = "https://raw.githubusercontent.com/KhronosGroup/OpenGL-Registry/master/xml/gl.xml";

        private static string ExtractNameFromCommand(XmlElement e)
        {
            var proto = e.GetElementsByTagName("proto").Item(0) as XmlElement;
            var name = proto!.GetElementsByTagName("name").Item(0) as XmlElement;
            return name!.InnerText;
        }
        
        private static void Search(ICollection<string> commands, IDictionary<string, string> constants, XmlElement e)
        {
            switch (e.Name)
            {
                case "command" when !e.HasChildNodes:
                    commands.Add(e.GetAttribute("name"));
                    return;
                case "command" when e.HasChildNodes:
                    commands.Add(ExtractNameFromCommand(e));
                    return;
                case "enum" when e.HasAttribute("value"):
                    var v = e.GetAttribute("value");
                    var l = Convert.ToInt64(v, v.StartsWith("0x") ? 16 : 10);
                    v = unchecked((int)l).ToString();

                    constants[e.GetAttribute("name")] = v;
                    return;
                case "extension":
                    return;
            }
            
            if (e.IsEmpty || e.ChildNodes.Count == 0) return;

            foreach (var n in e.GetElementsByTagName("*"))
            {
                var element = n as XmlElement;
                Search(commands, constants, element!);
            }
        }

        private static void Main()
        {
            var doc = new XmlDocument();
            Console.WriteLine($"Downloading document from {Url}");
            var reader = new XmlTextReader(Url);
            Console.WriteLine("Loading content");
            doc.Load(reader);
            reader.Dispose();
            
            Console.WriteLine("Searching");
            var root = doc.GetElementsByTagName("registry").Item(0) as XmlElement;
            var commands = new List<string>();
            var constants = new Dictionary<string, string>();
            Search(commands, constants, root!);
            commands = commands.OrderBy(a => a).Distinct().ToList();
            
            Console.WriteLine("Parsing data");
            var commandLines = new List<string>();
            commandLines.Add("namespace Castaway.OpenGL");
            commandLines.Add("{");
            commandLines.Add("    public enum GLF : ushort");
            commandLines.Add("    {");
            commandLines.AddRange(commands.Select(c => $"        {c},"));
            commandLines.Add("    }");
            commandLines.Add("}");
            
            File.WriteAllLines("GLF.Generated.cs", commandLines);
            Console.WriteLine("  Wrote GLF.Generated.cs");

            var constantLines = new List<string>();
            constantLines.Add("namespace Castaway.OpenGL");
            constantLines.Add("{");
            constantLines.Add("    public enum GLC : int");
            constantLines.Add("    {");
            constantLines.AddRange(constants.Select(c => $"        {c.Key}={c.Value},"));
            constantLines.Add("    }");
            constantLines.Add("}");
            
            File.WriteAllLines("GLC.Generated.cs", constantLines);
            Console.WriteLine("  Wrote GLC.Generated.cs");
        }
    }
}