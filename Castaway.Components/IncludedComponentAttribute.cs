using System;
using Castaway.Components;
using static System.AttributeTargets;

[assembly: IncludedComponent("castaway.components", "Castaway Components System")]

namespace Castaway.Components;

[AttributeUsage(Assembly)]
public class IncludedComponentAttribute : Attribute
{
	public string Id, ReadableName;

	public IncludedComponentAttribute(string id, string readableName)
	{
		Id = id;
		ReadableName = readableName;
	}
}