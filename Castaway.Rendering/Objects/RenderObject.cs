using System;

namespace Castaway.Rendering.Objects;

public abstract class RenderObject : IDisposable
{
	public abstract string Name { get; }
	public abstract void Dispose();
}