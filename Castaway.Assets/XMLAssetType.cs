using System.Text;
using System.Xml;

namespace Castaway.Assets;

[Loads("xml")]
public class XmlAssetType : IAssetType
{
	public virtual object Read(Asset a)
	{
		var d = new XmlDocument();
		d.LoadXml(Encoding.UTF8.GetString(a.GetBytes()));
		return d;
	}
}