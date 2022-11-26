using System;

namespace Castaway.Components;

[AttributeUsage(AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Property)]
public class PublishAttribute : Attribute
{
	public string Id;

	public PublishAttribute(string id)
	{
		Id = id;
	}
}