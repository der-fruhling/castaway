using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Castaway.Base;
using Castaway.Math;
using Castaway.Rendering.Structures;
using Serilog;

namespace Castaway.Rendering.MeshLoader;

public static class WavefrontOBJ
{
    private static ILogger Logger = CastawayGlobal.GetLogger();

    public static async Task<Mesh> ReadMesh(string[] lines)
    {
        return await Task.Run(delegate
        {
            Logger.Debug("Starting load of Wavefront OBJ mesh ({LineCount} lines)", lines.Length);
            var positions = new List<Vector3>();
            var textureCoords = new List<Vector3>();
            var normals = new List<Vector3>();
            var vertices = new List<Mesh.Vertex>();
            var elements = new List<uint>();

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (!line.Any() || line[0] == '#') continue;

                var parts = line.Split(' ');
                try
                {
                    switch (parts[0])
                    {
                        case "v":
                            positions.Add(new Vector3(
                                float.Parse(parts[1]),
                                float.Parse(parts[2]),
                                float.Parse(parts[3])));
                            break;
                        case "vn":
                            normals.Add(new Vector3(
                                float.Parse(parts[1]),
                                float.Parse(parts[2]),
                                float.Parse(parts[3])));
                            break;
                        case "vt" when parts.Length == 2:
                            textureCoords.Add(new Vector3(
                                float.Parse(parts[1]),
                                0,
                                0));
                            break;
                        case "vt" when parts.Length == 3:
                            textureCoords.Add(new Vector3(
                                float.Parse(parts[1]),
                                float.Parse(parts[2]),
                                0));
                            break;
                        case "vt" when parts.Length == 4:
                            textureCoords.Add(new Vector3(
                                float.Parse(parts[1]),
                                float.Parse(parts[2]),
                                float.Parse(parts[3])));
                            break;
                        case "f" when parts.Length == 4 && Regex.IsMatch(parts[1], @"^\d+$"):
                        {
                            vertices.AddRange(new Mesh.Vertex[]
                            {
                                new()
                                {
                                    Position = positions[int.Parse(parts[1]) - 1], Color = new Vector4(1, 1, 1, 1)
                                },
                                new()
                                {
                                    Position = positions[int.Parse(parts[2]) - 1], Color = new Vector4(1, 1, 1, 1)
                                },
                                new()
                                {
                                    Position = positions[int.Parse(parts[3]) - 1], Color = new Vector4(1, 1, 1, 1)
                                }
                            });
                            break;
                        }
                        case "f" when parts.Length == 4 && Regex.IsMatch(parts[1], @"^\d+/\d+$"):
                        {
                            var a = parts[1..].Select(s => s.Split('/')).ToArray();
                            var p = a.Select(ary => int.Parse(ary[0]) - 1).ToArray();
                            var t = a.Select(ary => int.Parse(ary[1]) - 1).ToArray();

                            vertices.AddRange(new Mesh.Vertex[]
                            {
                                new()
                                {
                                    Position = positions[p[0]], Texture = textureCoords[t[0]],
                                    Color = new Vector4(1, 1, 1, 1)
                                },
                                new()
                                {
                                    Position = positions[p[1]], Texture = textureCoords[t[1]],
                                    Color = new Vector4(1, 1, 1, 1)
                                },
                                new()
                                {
                                    Position = positions[p[2]], Texture = textureCoords[t[2]],
                                    Color = new Vector4(1, 1, 1, 1)
                                }
                            });
                            break;
                        }
                        case "f" when parts.Length == 4 && Regex.IsMatch(parts[1], @"^\d+/\d+/\d+$"):
                        {
                            var a = parts[1..].Select(s => s.Split('/')).ToArray();
                            var p = a.Select(ary => int.Parse(ary[0]) - 1).ToArray();
                            var t = a.Select(ary => int.Parse(ary[1]) - 1).ToArray();
                            var n = a.Select(ary => int.Parse(ary[2]) - 1).ToArray();

                            vertices.AddRange(new Mesh.Vertex[]
                            {
                                new()
                                {
                                    Position = positions[p[0]], Texture = textureCoords[t[0]],
                                    Normal = normals[n[0]], Color = new Vector4(1, 1, 1, 1)
                                },
                                new()
                                {
                                    Position = positions[p[1]], Texture = textureCoords[t[1]],
                                    Normal = normals[n[1]], Color = new Vector4(1, 1, 1, 1)
                                },
                                new()
                                {
                                    Position = positions[p[2]], Texture = textureCoords[t[2]],
                                    Normal = normals[n[2]], Color = new Vector4(1, 1, 1, 1)
                                }
                            });
                            break;
                        }
                        case "f" when parts.Length == 4 && Regex.IsMatch(parts[1], @"^\d+//\d+$"):
                        {
                            var a = parts[1..].Select(s => s.Split('/')).ToArray();
                            var p = a.Select(ary => int.Parse(ary[0]) - 1).ToArray();
                            var n = a.Select(ary => int.Parse(ary[1]) - 1).ToArray();

                            vertices.AddRange(new Mesh.Vertex[]
                            {
                                new()
                                {
                                    Position = positions[p[0]], Normal = normals[n[0]],
                                    Color = new Vector4(1, 1, 1, 1)
                                },
                                new()
                                {
                                    Position = positions[p[1]], Normal = normals[n[1]],
                                    Color = new Vector4(1, 1, 1, 1)
                                },
                                new()
                                {
                                    Position = positions[p[2]], Normal = normals[n[2]],
                                    Color = new Vector4(1, 1, 1, 1)
                                }
                            });
                            break;
                        }
                        case "mtllib": /* TODO Implement Materials */ break;
                        case "usemtl": /* TODO Implement Materials */ break;
                        case "s": /* TODO Implement `s` */ break;
                        case "o":
                            Logger.Verbose("In object {Name}", parts[1]);
                            break;
                        default:
                            throw new InvalidOperationException($"Invalid line {line}");
                    }
                }
                catch (IndexOutOfRangeException e)
                {
                    throw new AggregateException($"Not enough data on line {i + 1}", e);
                }
            }

            Logger.Verbose("Starting to compact {Count} vertices", vertices.Count);
            var realVertices = new List<Mesh.Vertex>();
            for (uint i = 0; i < vertices.Count; i++)
            {
                var vertex = vertices[(int) i];
                if (!realVertices.Contains(vertex))
                {
                    elements.Add((uint) realVertices.Count);
                    realVertices.Add(vertex);
                }
                else
                {
                    elements.Add((uint) realVertices.IndexOf(vertex));
                }
            }

            Logger.Debug("Finished mesh construction; {ElementCount} elements, {VertexCount} vertices",
                elements.Count, realVertices.Count);
            return new Mesh(realVertices.ToArray(), elements.ToArray());
        });
    }
}