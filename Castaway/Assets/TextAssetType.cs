using System;
using System.Text;

namespace Castaway.Assets
{
    public class TextAssetType : IAssetType
    {
        public T To<T>(Asset a)
        {
            if (typeof(T) == typeof(string))
                return (T) (dynamic) Encoding.UTF8.GetString(a.GetBytes());
            throw new InvalidOperationException($"Cannot convert TextAssetType to {typeof(T).FullName}");
        }
    }
}