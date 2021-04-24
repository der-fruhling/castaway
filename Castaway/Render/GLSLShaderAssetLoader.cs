using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Castaway.Assets;
using static Castaway.Render.VertexAttribInfo;

namespace Castaway.Render
{
    /// <summary>
    /// Loads GLSL shaders from a folder.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GLSLShaderAssetLoader : IAssetLoader
    {
        public IEnumerable<string> FileExtensions { get; } = new[] {"shdr"};
        
        public object LoadFile(string path)
        {
            var ldr = new TextAssetLoader();
            var dir = AssetManager.AssetFolderPath + (string)ldr.LoadFile(path);
            var vertSrc = (string)ldr.LoadFile($"{dir}/vert.glsl");
            var fragSrc = (string)ldr.LoadFile($"{dir}/frag.glsl");
            var confSrc = (string)ldr.LoadFile($"{dir}/conf.csnf");

            var vertAttrs = new Dictionary<string, AttribValue>();
            var fragOutputs = new Dictionary<string, uint>();
            var model = "";
            var view = "";
            var projection = "";
            var properties = new Dictionary<string, string>();
            
            Configure(confSrc, vertAttrs, fragOutputs, ref model, ref view, ref projection, properties);
            return new LoadedShader(vertAttrs, fragOutputs, vertSrc, fragSrc, model, view, projection, properties);
        }

        public static void Configure(string confSrc, Dictionary<string, AttribValue> vertAttrs, 
            Dictionary<string, uint> fragOutputs, ref string model, ref string view, ref string projection,
            Dictionary<string, string> properties)
        {
            var lines = confSrc.Split('\n');
            foreach (var line in lines)
            {
                var command = line.Split("//")[0].Trim();
                if(command.Length == 0) continue;
                var cmdParts = command.Split(' ');
                switch (cmdParts[0])
                {
                    case "input" when cmdParts.Length == 4 && cmdParts[2] == "=":
                        vertAttrs[cmdParts[1]] = Enum.Parse<AttribValue>(cmdParts[3]);
                        break;
                    case "output" when cmdParts.Length == 4 && cmdParts[2] == "=":
                        fragOutputs[cmdParts[1]] = uint.Parse(cmdParts[3]);
                        break;
                    case "transform" when cmdParts.Length == 4 && cmdParts[2] == "=":
                        switch (cmdParts[1])
                        {
                            case "model":
                                model = cmdParts[3];
                                break;
                            case "view":
                                view = cmdParts[3];
                                break;
                            case "projection":
                                projection = cmdParts[3];
                                break;
                            default:
                                throw new InvalidOperationException(
                                    $"Cannot set transform matrix {cmdParts[1]} to {cmdParts[3]}");
                        }
                        break;
                    case "use" when cmdParts.Length == 4 && cmdParts[2] == "as":
                        properties[cmdParts[3]] = cmdParts[1];
                        break;
                    case "set" when cmdParts.Length == 4 && cmdParts[2] == "=":
                        properties[cmdParts[1]] = cmdParts[3];
                        break;
                    default:
                        throw new InvalidOperationException($"Couldn't process config: Invalid line `{line}`");
                }
            }
        }
    }
}