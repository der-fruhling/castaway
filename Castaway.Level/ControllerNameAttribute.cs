using System;

namespace Castaway.Level;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public class ControllerNameAttribute : Attribute
{
    public string? Name;

    public ControllerNameAttribute(string name)
    {
        Name = name;
    }

    public ControllerNameAttribute()
    {
        Name = null;
    }
}