using System;
using System.IO;
using Castaway.Assets;
using SixLabors.ImageSharp;

namespace Castaway.OpenGL;

[Loads("png", "jpg", "jpeg")]
public class ImageAssetType : IAssetType
{
	public T To<T>(Asset a)
	{
		if (typeof(T) != typeof(Image))
			throw new InvalidOperationException($"Cannot convert ImageAssetType to {typeof(T).FullName}");
		using var s = new MemoryStream(a.GetBytes());
		return (T)(dynamic)Image.Load(s);
	}
}