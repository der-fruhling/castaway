using System;
using System.Diagnostics.CodeAnalysis;
using Castaway.Components;
using static System.AttributeTargets;

[assembly: IncludedComponent("castaway.components", "Castaway Components System")]

namespace Castaway.Components
{
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

    [AttributeUsage(Assembly)]
    public class BaseAttribute : Attribute
    {
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum VersionStream
    {
        None,
        RL,
        PR,
        PG
    }

    [AttributeUsage(Assembly)]
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

    [AttributeUsage(Assembly)]
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

    [AttributeUsage(Assembly)]
    public class VersionAttribute : Attribute
    {
        public VersionStream Stream;
        public Version Version;

        public VersionAttribute(VersionStream stream, int major, int minor, int build)
        {
            Version = new Version(major, minor, build);
            Stream = stream;
        }

        public override string ToString()
        {
            return Version.ToString();
        }
    }

    [AttributeUsage(Method)]
    public class RegistrantAttribute : Attribute
    {
        public string Hook;

        public RegistrantAttribute(string hook)
        {
            Hook = hook;
        }
    }

    [AttributeUsage(Method)]
    public class EventHandlerAttribute : Attribute
    {
        public string Hook;

        public EventHandlerAttribute(string hook)
        {
            Hook = hook;
        }
    }

    [AttributeUsage(Assembly)]
    public class RequiresAttribute : Attribute
    {
        public string Id;

        public RequiresAttribute(string id)
        {
            Id = id;
        }
    }

    [AttributeUsage(Event | Field | Property)]
    public class PublishAttribute : Attribute
    {
        public string Id;

        public PublishAttribute(string id)
        {
            Id = id;
        }
    }

    [AttributeUsage(Field | Property)]
    public class SetToAttribute : Attribute
    {
        public string Id;

        public SetToAttribute(string id)
        {
            Id = id;
        }
    }
}