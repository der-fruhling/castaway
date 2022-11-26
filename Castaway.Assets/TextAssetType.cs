using System.Text;

namespace Castaway.Assets;

[Loads("txt")]
public class TextAssetType : IAssetType
{
	public object Read(Asset a) => Encoding.UTF8.GetString(a.GetBytes());
}