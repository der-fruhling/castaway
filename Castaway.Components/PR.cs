using System;

namespace Castaway.Components;

[AttributeUsage(AttributeTargets.Assembly)]
public class PR : Attribute
{
	public int Week, Hotfix;

	public PR(int week, int hotfix)
	{
		Week = week;
		Hotfix = hotfix;
	}

	public int Number => int.Parse($"{Week}00{Hotfix}");

	public override string ToString()
	{
		return $"pg {Week}.{Hotfix}";
	}
}