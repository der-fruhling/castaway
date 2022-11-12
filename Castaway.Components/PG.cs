using System;

namespace Castaway.Components;

[AttributeUsage(AttributeTargets.Assembly)]
public class PG : Attribute
{
    public int Year, Month, Day, Hotfix;

    public PG(int year, int month, int day, int hotfix)
    {
        Year = year;
        Month = month;
        Day = day;
        Hotfix = hotfix;
    }

    public int Number => int.Parse($"{Year}{Month:02}{Day:02}{Hotfix}");

    public override string ToString()
    {
        return $"pg {Year}.{Month}.{Day}.{Hotfix}";
    }
}