using System;

namespace Castaway.Components;

[AttributeUsage(AttributeTargets.Method)]
public class EventHandlerAttribute : Attribute
{
    public string Hook;

    public EventHandlerAttribute(string hook)
    {
        Hook = hook;
    }
}