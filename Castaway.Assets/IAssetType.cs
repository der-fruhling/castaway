using System;

namespace Castaway.Assets;

public interface IAssetType
{
	T Read<T>(Asset a) => Read(a) is T t
		? t
		: throw new InvalidCastException($"Read<T>() couldn't cast into {typeof(T).FullName}");

	object Read(Asset a);
}