using System;

namespace Castaway.Level;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class LevelSerializedAttribute : Attribute
{
	public string Name;

	public LevelSerializedAttribute(string name)
	{
		Name = name;
	}
}