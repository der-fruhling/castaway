using System;
using Castaway.Rendering;
using Castaway.Rendering.Objects;
using OpenTK.Graphics.OpenGL;

namespace Castaway.OpenGL;

public class VertexArrayDrawable : Drawable
{
	internal readonly uint VertexArrayObject;
	internal bool SetUp;

	public VertexArrayDrawable(int vertexCount, BufferObject vertexArray) : base(vertexCount, vertexArray)
	{
		if (Graphics.Current is not OpenGLImpl) throw new InvalidOperationException("Need OpenGL >= 3.2");
		GL.CreateVertexArrays(1, out VertexArrayObject);
		GL.BindVertexArray(VertexArrayObject);
		VertexArray?.Bind();
		GL.BindVertexArray(0);
	}

	public VertexArrayDrawable(int vertexCount, BufferObject vertexArray, BufferObject elementArray) : base(
		vertexCount, vertexArray, elementArray)
	{
		if (Graphics.Current is not OpenGLImpl) throw new InvalidOperationException("Need OpenGL >= 3.2");
		GL.CreateVertexArrays(1, out VertexArrayObject);
		GL.BindVertexArray(VertexArrayObject);
		VertexArray?.Bind();
		ElementArray?.Bind();
		GL.BindVertexArray(0);
	}
}