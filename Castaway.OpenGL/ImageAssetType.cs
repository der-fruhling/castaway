using System.IO;
using Castaway.Assets;
using SixLabors.ImageSharp;

namespace Castaway.OpenGL;

[Loads("png", "jpg", "jpeg")]
public class ImageAssetType : IAssetType
{
	public object Read(Asset a)
	{
		using var s = new MemoryStream(a.GetBytes());
		return Image.Load(s);
	}
}