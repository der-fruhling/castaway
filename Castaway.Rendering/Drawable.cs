using System;
using Castaway.Rendering.Objects;

namespace Castaway.Rendering;

public class Drawable : IDisposable
{
	public BufferObject? ElementArray;
	public BufferObject? VertexArray;
	public int VertexCount;

	public Drawable(int vertexCount, BufferObject vertexArray)
	{
		VertexCount = vertexCount;
		VertexArray = vertexArray;
		ElementArray = null;
	}

	public Drawable(int vertexCount, BufferObject vertexArray, BufferObject elementArray)
	{
		VertexCount = vertexCount;
		VertexArray = vertexArray;
		ElementArray = elementArray;
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
		VertexArray?.Dispose();
		ElementArray?.Dispose();
	}
}