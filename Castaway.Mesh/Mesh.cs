#nullable enable
using Castaway.Math;

namespace Castaway.Mesh
{
    /// <summary>
    /// Structure to store position, normal, and texture coordinates for
    /// vertices.
    /// </summary>
    public class CompleteVertex
    {
        public Vector3 Pos = Vector3.Zero;
        public Vector3 Norm = Vector3.Zero;
        public Vector3 Tex = Vector3.Zero;

        public override string ToString()
        {
            return $"{nameof(Pos)}: {Pos}, {nameof(Norm)}: {Norm}, {nameof(Tex)}: {Tex}";
        }
    }
    
    /// <summary>
    /// Allows easier converting an <see cref="IMesh"/> to other formats.
    /// </summary>
    public class MeshConverter
    {
        private CompleteVertex[] _vertices;
        private int _i;

        public MeshConverter(CompleteVertex[] vertices)
        {
            _vertices = vertices;
        }

        public void Next(out Vector3? pos, out Vector3? tex, out Vector3? norm)
        {
            var v = _vertices[_i];
            pos = v.Pos;
            tex = v.Tex;
            norm = v.Norm;
            _i++;
        }

        public bool More => _i < _vertices.Length;
    }
    
    /// <summary>
    /// Stores all <see cref="CompleteVertex"/> objects contained in a mesh,
    /// and supports loading that data from a byte array.
    /// </summary>
    public interface IMesh
    {
        /// <summary>
        /// Loads this mesh from a byte array.
        /// </summary>
        /// <param name="input">Array to load from.</param>
        /// <param name="path"></param>
        public void Load(byte[] input, string path);
        
        /// <summary>
        /// Gets every <see cref="CompleteVertex"/> in this mesh.
        /// </summary>
        public CompleteVertex[] Vertices { get; }
        
        /// <summary>
        /// Creates a new <see cref="MeshConverter"/> to convert
        /// <see cref="CompleteVertex"/> objects to another format.
        /// </summary>
        public MeshConverter Converter { get; }
    }
}