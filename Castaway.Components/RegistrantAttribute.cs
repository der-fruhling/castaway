using System;

namespace Castaway.Components;

[AttributeUsage(AttributeTargets.Method)]
public class RegistrantAttribute : Attribute
{
	public string Hook;

	public RegistrantAttribute(string hook)
	{
		Hook = hook;
	}
}