using System;

namespace Castaway.Components;

[AttributeUsage(AttributeTargets.Assembly)]
public class RequiresAttribute : Attribute
{
    public string Id;

    public RequiresAttribute(string id)
    {
        Id = id;
    }
}