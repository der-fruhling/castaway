using System;
using System.Reflection;

namespace Castaway.Rendering.Objects;

public class RenderObjectInvalidException : ApplicationException
{
    public RenderObjectInvalidException(MemberInfo type, string name) : base($"{type.Name}:{name}")
    {
    }
}