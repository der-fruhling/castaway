using System;
using System.Collections.Generic;
using System.Linq;
using Castaway.Rendering;
using Castaway.Rendering.Objects;
using OpenTK.Graphics.OpenGL;
using BufferTarget = OpenTK.Graphics.OpenGL.BufferTarget;
using CBufferTarget = Castaway.Rendering.BufferTarget;

namespace Castaway.OpenGL;

internal sealed class Buffer : BufferObject
{
	public int SetupProgram;
	public CBufferTarget Target;

	public Buffer(CBufferTarget target, uint number)
	{
		Target = target;
		Number = number;
	}

	public Buffer(CBufferTarget target, uint number, IEnumerable<float> data) : this(target, number)
	{
		Upload(data);
	}

	public Buffer(CBufferTarget target, uint number, IEnumerable<uint> data) : this(target, number)
	{
		Upload(data);
	}

	public Buffer(CBufferTarget target, uint number, IEnumerable<int> data) : this(target, number)
	{
		Upload(data);
	}

	public Buffer(CBufferTarget target, uint number, IEnumerable<double> data) : this(target, number)
	{
		Upload(data);
	}

	public Buffer(CBufferTarget target)
	{
		Target = target;
		GL.CreateBuffers(1, out uint n);
		Number = n;
	}

	public Buffer(CBufferTarget target, IEnumerable<float> data) : this(target)
	{
		Upload(data);
	}

	public Buffer(CBufferTarget target, IEnumerable<uint> data) : this(target)
	{
		Upload(data);
	}

	public Buffer(CBufferTarget target, IEnumerable<int> data) : this(target)
	{
		Upload(data);
	}

	public Buffer(CBufferTarget target, IEnumerable<double> data) : this(target)
	{
		Upload(data);
	}

	public bool Destroyed { get; set; }
	public uint Number { get; set; }
	public override string Name => $"{Target}->{Number}({Valid})";
	public override bool Valid => GL.IsBuffer(Number) && !Destroyed;

	public override void Bind()
	{
		if (Graphics.Current is not OpenGLImpl gl) throw new InvalidOperationException("Need OpenGL >= 3.2");
		GL.BindBuffer(Target switch
		{
			CBufferTarget.VertexArray => BufferTarget.ArrayBuffer,
			CBufferTarget.ElementArray => BufferTarget.ElementArrayBuffer,
			_ => throw new ArgumentOutOfRangeException()
		}, Number);
		gl.BoundBuffers[Target] = this;
	}

	public override void Unbind()
	{
		if (Graphics.Current is not OpenGLImpl gl) throw new InvalidOperationException("Need OpenGL >= 3.2");
		GL.BindBuffer(Target switch
		{
			CBufferTarget.VertexArray => BufferTarget.ArrayBuffer,
			CBufferTarget.ElementArray => BufferTarget.ElementArrayBuffer,
			_ => throw new ArgumentOutOfRangeException()
		}, 0);
		gl.BoundBuffers[Target] = null;
	}

	public bool Equals(Buffer other)
	{
		return SetupProgram == other.SetupProgram && Target == other.Target && Destroyed == other.Destroyed &&
		       Number == other.Number;
	}

	public override bool Equals(object? obj)
	{
		return obj is Buffer other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(SetupProgram, (int)Target, Destroyed, Number);
	}

	public static bool operator ==(Buffer left, Buffer right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(Buffer left, Buffer right)
	{
		return !left.Equals(right);
	}

	public override void Dispose()
	{
		GL.DeleteBuffers(1, new[] { Number });
	}

	public override void Upload(IEnumerable<byte> bytes)
	{
		var target = Target switch
		{
			CBufferTarget.VertexArray => BufferTarget.ArrayBuffer,
			CBufferTarget.ElementArray => BufferTarget.ElementArrayBuffer,
			_ => throw new ArgumentOutOfRangeException()
		};
		GL.GetInteger(Target switch
		{
			CBufferTarget.VertexArray => GetPName.ArrayBufferBinding,
			CBufferTarget.ElementArray => GetPName.ElementArrayBufferBinding,
			_ => throw new ArgumentOutOfRangeException()
		});
		GL.BindBuffer(target, Number);
		var b = bytes.ToArray();
		GL.BufferData(target, b.Length, b, BufferUsageHint.StaticDraw);
	}
}