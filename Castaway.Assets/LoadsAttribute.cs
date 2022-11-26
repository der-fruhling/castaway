using System;

namespace Castaway.Assets;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class LoadsAttribute : Attribute
{
	public string[] Extensions;

	public LoadsAttribute(params string[] extensions)
	{
		Extensions = extensions;
	}
}