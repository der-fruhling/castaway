using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Castaway.Assets;
using Castaway.Math;

namespace Castaway.Mesh
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class OBJMesh : IMesh
    {
        private List<CompleteVertex> _vertices = new List<CompleteVertex>();
        public Material Material = new Material();
        
        private readonly List<Vector3> vertices = new List<Vector3>();
        private readonly List<Vector2> textures = new List<Vector2>();
        private readonly List<Vector3> normals = new List<Vector3>();
        private readonly Stopwatch stopwatch = new Stopwatch();
        
        public void Load(byte[] input, string path)
        {
            stopwatch.Restart();
            var lines = Encoding.UTF8.GetString(input)
                .Split('\n')
                .Select(s => s.Split('#')[0].Trim())
                .Where(s => s.Length > 0)
                .Select(s => s.Split(' '));
            
            Dictionary<string, Material> materials = null!;

            foreach (var parts in lines)
            {
                switch (parts[0])
                {
                    case "v":
                    {
                        if (!float.TryParse(parts[1], out var x))
                            throw new ApplicationException($"Invalid OBJ file: {parts}");
                        if (!float.TryParse(parts[2], out var y))
                            throw new ApplicationException($"Invalid OBJ file: {parts}");
                        if (!float.TryParse(parts[3], out var z))
                            throw new ApplicationException($"Invalid OBJ file: {parts}");
                        
                        vertices.Add(new Vector3(x, y, z));
                        break;
                    }
                    case "vt":
                    {
                        if (!float.TryParse(parts[1], out var x))
                            throw new ApplicationException($"Invalid OBJ file: {parts}");
                        if (!float.TryParse(parts[2], out var y))
                            throw new ApplicationException($"Invalid OBJ file: {parts}");
                        
                        textures.Add(new Vector2(x, y));
                        break;
                    }
                    case "vn":
                    {
                        if (!float.TryParse(parts[1], out var x))
                            throw new ApplicationException($"Invalid OBJ file: {parts}");
                        if (!float.TryParse(parts[2], out var y))
                            throw new ApplicationException($"Invalid OBJ file: {parts}");
                        if (!float.TryParse(parts[3], out var z))
                            throw new ApplicationException($"Invalid OBJ file: {parts}");
                        
                        normals.Add(new Vector3(x, y, z));
                        break;
                    }
                    case "f" when parts.Length == 4:
                    {
                        foreach (var s in parts[1..]) ResolveReference(s);
                        break;
                    }
                    case "f" when parts.Length == 5:
                    {
                        ResolveReference(parts[1]);
                        ResolveReference(parts[2]);
                        ResolveReference(parts[3]);
                        ResolveReference(parts[1]);
                        ResolveReference(parts[4]);
                        ResolveReference(parts[3]);
                        break;
                    }
                    case "mtllib":
                    {
                        ReadMaterials(out materials, $"{Directory.GetParent(path)}/{parts[1]}");
                        break;
                    }
                    case "usemtl":
                        Material = materials?[parts[1]];
                        break;
                    case "o":
                    case "g":
                    case "s": break;
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"Load took {stopwatch.ElapsedMilliseconds}ms");
        }

        private static void ReadMaterials(out Dictionary<string, Material> materials, string path)
        {
            var lines = File.ReadAllLines(path)
                .Select(s => s.Split('#')[0].Trim())
                .Where(s => s.Length > 0)
                .Select(s => s.Split(' '));
            materials = new Dictionary<string, Material>();
            string current = null;
            foreach (var parts in lines)
            {
                switch (parts[0])
                {
                    case "newmtl":
                        current = parts[1];
                        materials[current] = new Material {Name = current};
                        break;
                    case "Ka" when current != null:
                        materials[current].Ambient = new Vector3(
                            float.Parse(parts[1]),
                            float.Parse(parts[2]),
                            float.Parse(parts[3]));
                        break;
                    case "Kd" when current != null:
                        materials[current].Diffuse = new Vector3(
                            float.Parse(parts[1]),
                            float.Parse(parts[2]),
                            float.Parse(parts[3]));
                        break;
                    case "Ks" when current != null:
                        materials[current].Specular = new Vector3(
                            float.Parse(parts[1]),
                            float.Parse(parts[2]),
                            float.Parse(parts[3]));
                        break;
                    case "Ns" when current != null:
                        materials[current].SpecularExponent = float.Parse(parts[1]);
                        break;
                    case "d" when current != null:
                        materials[current].Dissolve = float.Parse(parts[1]);
                        break;
                    case "Tr" when current != null:
                        materials[current].Dissolve = 1 - float.Parse(parts[1]);
                        break;
                    case "Ni" when current != null:
                        materials[current].IndexOfRefraction = float.Parse(parts[1]);
                        break;
                    case "illum" when current != null:
                        materials[current].Mode = (Material.IllumMode) int.Parse(parts[1]);
                        break;
                    default:
                        throw new ApplicationException($"Invalid material line: {parts}");
                }
            }
        }

        private void ResolveReference(string v)
        {
            if (Regex.IsMatch(v, @"^\d+\/\d+\/\d+$"))
            {
                var p = v.Split('/');
                _vertices.Add(new CompleteVertex
                {
                    Pos = vertices[int.Parse(p[0]) - 1],
                    Tex = textures[int.Parse(p[1]) - 1],
                    Norm = normals[int.Parse(p[2]) - 1],
                });
            }
            else if (Regex.IsMatch(v, @"^\d+$"))
            {
                var p = v.Split('/');
                _vertices.Add(new CompleteVertex
                {
                    Pos = vertices[int.Parse(p[0]) - 1],
                    Tex = Vector2.Zero,
                    Norm = Vector3.Zero,
                });
            }
            else if (Regex.IsMatch(v, @"^\d+\/\d+$"))
            {
                var p = v.Split('/');
                _vertices.Add(new CompleteVertex
                {
                    Pos = vertices[int.Parse(p[0]) - 1],
                    Tex = textures[int.Parse(p[1]) - 1],
                    Norm = Vector3.Zero,
                });
            }
            else if (Regex.IsMatch(v, @"^\d+\/\/\d+$"))
            {
                var p = v.Split('/', StringSplitOptions.RemoveEmptyEntries);
                _vertices.Add(new CompleteVertex
                {
                    Pos = vertices[int.Parse(p[0]) - 1],
                    Tex = Vector2.Zero,
                    Norm = normals[int.Parse(p[1]) - 1],
                });
            }
            else throw new ApplicationException($"Invalid vertex reference: {v}");
        }

        public CompleteVertex[] Vertices
        {
            get
            {
                stopwatch.Restart();
                var v = _vertices.ToArray();
                stopwatch.Stop();
                Console.WriteLine($"Array conversion took {stopwatch.ElapsedMilliseconds}ms");
                return v;
            }

            set
            {
                stopwatch.Restart();
                _vertices = value.ToList();
                stopwatch.Stop();
                Console.WriteLine($"List conversion took {stopwatch.ElapsedMilliseconds}ms");
            }
        }

        public MeshConverter Converter => new MeshConverter(Vertices);

        public class Loader : IAssetLoader
        {
            public IEnumerable<string> FileExtensions { get; } = new[] {"obj"};
            
            public object LoadFile(string path)
            {
                var m = new OBJMesh();
                m.Load(File.ReadAllBytes(path), path);
                return m;
            }
        }
    }
}