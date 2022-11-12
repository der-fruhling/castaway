using System;
using System.Text;
using Castaway.Assets;
using Castaway.Rendering.MeshLoader;
using Castaway.Rendering.Structures;

namespace Castaway.Rendering;

[Loads("obj", "mtl")]
public class WavefrontOBJAssetType : IAssetType
{
    public T To<T>(Asset a)
    {
        if (typeof(T) == typeof(string))
            return (T) (dynamic) Encoding.UTF8.GetString(a.GetBytes());
        if (typeof(T) == typeof(Mesh))
            return (T) (dynamic) WavefrontOBJ.ReadMesh(To<string>(a).Split('\n')).Result;
        throw new InvalidOperationException($"Cannot convert {nameof(WavefrontOBJAssetType)} to {typeof(T).Name}");
    }
}