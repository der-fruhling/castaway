using System;
using System.Drawing;
using System.IO;
using Castaway.Assets;

namespace Castaway.OpenGL;

[Loads("png", "jpg", "jpeg")]
public class ImageAssetType : IAssetType
{
    public T To<T>(Asset a)
    {
        if (typeof(T) == typeof(Bitmap))
            return (T) (dynamic) new Bitmap(new MemoryStream(a.GetBytes()));
        throw new InvalidOperationException($"Cannot convert ImageAssetType to {typeof(T).FullName}");
    }
}