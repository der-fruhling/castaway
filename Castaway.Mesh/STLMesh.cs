using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using Castaway.Assets;
using Castaway.Math;

namespace Castaway.Mesh
{
    /// <summary>
    /// Loads meshes from .stl files.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class STLMesh : IMesh
    {
        private readonly List<CompleteVertex> _vertices = new List<CompleteVertex>();
        
        public void Load(byte[] input)
        {
            var r = new BinaryReader(new MemoryStream(input));
            var header = r.ReadBytes(80);
            var count = r.ReadUInt32();
            for (var i = 0; i < count; i++)
            {
                var normal = new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
                var v1 = new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
                var v2 = new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
                var v3 = new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
                
                _vertices.Add(new CompleteVertex { Pos = v1, Norm = normal });
                _vertices.Add(new CompleteVertex { Pos = v2, Norm = normal });
                _vertices.Add(new CompleteVertex { Pos = v3, Norm = normal });

                var attrSize = r.ReadUInt16();
                r.ReadBytes(attrSize);
            }
        }

        public CompleteVertex[] Vertices => _vertices.ToArray();
        public MeshConverter Converter => new MeshConverter(Vertices);

        /// <summary>
        /// Allows loading .stl asset files.
        /// </summary>
        /// <seealso cref="MeshModule"/>
        public class Loader : IAssetLoader
        {
            public IEnumerable<string> FileExtensions { get; } = new[] {"stl"};
            
            public object LoadFile(string path)
            {
                var m = new STLMesh();
                m.Load(File.ReadAllBytes(path));
                return m;
            }
        }
    }
}