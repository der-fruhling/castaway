using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Castaway.Assets;

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
            var vertSrc = (string)ldr.LoadFile($"{dir}/shader.vsh");
            var fragSrc = (string)ldr.LoadFile($"{dir}/shader.fsh");
            var confSrc = (string)ldr.LoadFile($"{dir}/shader.csh");

            var vertAttrs = new Dictionary<string, VertexAttribInfo.AttribValue>();
            var fragOutputs = new Dictionary<string, uint>();

            var model = "";
            var view = "";
            var projection = "";

            var properties = new Dictionary<string, string>();

            var lines = confSrc.Split('\n');
            foreach (var line in lines)
            {
                var s = line.Split("//")[0];
                if(s.Length == 0 || s[0] != '#') continue;
                var command = s[1..];
                var cmdParts = command.Split(' ');
                switch (cmdParts[0])
                {
                    case "input" when cmdParts.Length == 4 && cmdParts[2] == "=":
                        vertAttrs[cmdParts[1]] = Enum.Parse<VertexAttribInfo.AttribValue>(cmdParts[3]);
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

            return new LoadedShader(vertAttrs, fragOutputs, vertSrc, fragSrc, model, view, projection, properties);
        }
    }
}