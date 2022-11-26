using System;

namespace Castaway.Base;

/// <summary>
///     Workaround type. Use with a type from each of the assemblies
///     that you will use.
/// </summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class ImportsAttribute : Attribute
{
	public Type[] Types;

	public ImportsAttribute(params Type[] types)
	{
		Types = types;
	}
}