#nullable enable
using System;

namespace Castaway.Rendering;

public class GraphicsException : ApplicationException
{
    public GraphicsException()
    {
    }

    public GraphicsException(string? message) : base(message)
    {
    }

    public GraphicsException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}