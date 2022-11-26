using System;

namespace Castaway.Rendering;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class ProvidesShadersForAttribute : Attribute
{
	public Type When;

	public ProvidesShadersForAttribute(Type when)
	{
		When = when;
	}
}