using System.Text;
using Castaway.Assets;
using Castaway.Rendering.MeshLoader;

namespace Castaway.Rendering;

[Loads("obj", "mtl")]
public class WavefrontObjAssetType : IAssetType
{
	public object Read(Asset a)
	{
		return WavefrontOBJ.ReadMesh(
				Encoding.UTF8.GetString(a.GetBytes()).Split('\n'))
			.Result;
	}
}