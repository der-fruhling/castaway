using System;
using System.Reflection;

namespace Castaway.Components;

public class EventHook
{
    public readonly object Container;
    public readonly EventInfo Event;

    public EventHook(object container, EventInfo @event)
    {
        Container = container;
        Event = @event;
    }

    public void Register<T>(T @delegate) where T : Delegate
    {
        Event.AddEventHandler(Container, @delegate);
    }
}