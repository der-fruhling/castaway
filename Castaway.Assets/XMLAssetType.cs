using System;
using System.Text;
using System.Xml;

namespace Castaway.Assets
{
    [Loads("xml")]
    public class XMLAssetType : IAssetType
    {
        public virtual T To<T>(Asset a)
        {
            if (typeof(T) == typeof(string))
                return (T) (dynamic) Encoding.UTF8.GetString(a.GetBytes());
            if (typeof(T) == typeof(XmlDocument))
            {
                var d = new XmlDocument();
                d.LoadXml(To<string>(a));
                return (T) (dynamic) d;
            } 
            throw new InvalidOperationException($"Cannot convert XMLAssetType to {typeof(T).FullName}");
        }
    }
}