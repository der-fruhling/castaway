using System;

namespace Castaway.Components;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class SetToAttribute : Attribute
{
	public string Id;

	public SetToAttribute(string id)
	{
		Id = id;
	}
}