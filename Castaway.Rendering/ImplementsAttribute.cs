using System;

namespace Castaway.Rendering;

[AttributeUsage(AttributeTargets.Class)]
public class ImplementsAttribute : Attribute
{
	public string Name;

	public ImplementsAttribute(string name)
	{
		Name = name;
	}
}