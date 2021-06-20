#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Serilog;

namespace Castaway.OpenGL.FunctionListGenerator
{
    internal static class Program
    {
        private const string Url = "https://raw.githubusercontent.com/KhronosGroup/OpenGL-Registry/master/xml/gl.xml";

        private static string ExtractNameFromCommand(ILogger log, XmlElement e)
        {
            var proto = e.GetElementsByTagName("proto").Item(0) as XmlElement;
            var name = proto!.GetElementsByTagName("name").Item(0) as XmlElement;
            return name!.InnerText;
        }

        private static void Search(ILogger log, ICollection<string> commands, IDictionary<string, string> constants, XmlElement e)
        {
            switch (e.Name)
            {
                case "command" when !e.HasChildNodes:
                    commands.Add(e.GetAttribute("name"));
                    return;
                case "command" when e.HasChildNodes:
                    commands.Add(ExtractNameFromCommand(log, e));
                    return;
                case "enum" when e.HasAttribute("value"):
                    var v = e.GetAttribute("value");
                    var l = Convert.ToInt64(v, v.StartsWith("0x") ? 16 : 10);
                    v = unchecked((int) l).ToString();

                    constants[e.GetAttribute("name")] = v;
                    return;
                case "extension":
                    return;
            }

            if (e.IsEmpty || e.ChildNodes.Count == 0) return;

            foreach (var n in e.GetElementsByTagName("*"))
            {
                var element = n as XmlElement;
                Search(log, commands, constants, element!);
            }
        }

        private static void Main()
        {
            using var log = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: "({Timestamp:HH:mm:ss} {Level:u3}) {Message:lj}{NewLine}{Exception}")
                .MinimumLevel.Information()
                .CreateLogger();
            
            var doc = new XmlDocument();
            log.Information("Downloading document from {Url}", Url);
            using (var reader = new XmlTextReader(Url))
            {
                log.Information("Loading content");
                doc.Load(reader);
            }

            log.Information("Searching XML data for points of interest");
            var root = doc.GetElementsByTagName("registry").Item(0) as XmlElement;
            var commands = new List<string>();
            var constants = new Dictionary<string, string>();
            Search(log, commands, constants, root!);
            log.Debug("Found {Count} commands before filtering", commands.Count);
            log.Debug("Found {Count} constants before filtering", constants.Count);
            commands = commands.Distinct().ToList();
            log.Debug("Found {Count} commands after filtering", commands.Count);
            log.Debug("Found {Count} constants after filtering", constants.Count);

            log.Information("Generating enum GLF");
            var commandLines = new List<string>();
            commandLines.Add("namespace Castaway.OpenGL.Native");
            commandLines.Add("{");
            commandLines.Add("    public enum GLF : ushort");
            commandLines.Add("    {");
            log.Debug("Iterating {Count} commands", commands.Count);
            commandLines.AddRange(commands.Select(c => $"        {c},"));
            commandLines.Add("    }");
            commandLines.Add("}");

            File.WriteAllLines("GLF.Generated.cs", commandLines);
            log.Debug("Finished generating enum GLF; wrote to GLF.Generated.cs");

            log.Information("Generating enum GLC");
            var constantLines = new List<string>();
            constantLines.Add("namespace Castaway.OpenGL.Native");
            constantLines.Add("{");
            constantLines.Add("    public enum GLC : int");
            constantLines.Add("    {");
            log.Debug("Iterating {Count} constants", constants.Count);
            constantLines.AddRange(constants.Select(c => $"        {c.Key}={c.Value},"));
            constantLines.Add("    }");
            constantLines.Add("}");

            File.WriteAllLines("GLC.Generated.cs", constantLines);
            log.Debug("Finished generating enum GLC; wrote to GLC.Generated.cs");
        }
    }
}