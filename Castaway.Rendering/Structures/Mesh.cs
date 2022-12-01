using System;
using System.Collections.Generic;
using System.Linq;
using Castaway.Math;
using Castaway.Rendering.Objects;
using Castaway.Rendering.Shaders;

namespace Castaway.Rendering.Structures;

public struct Mesh
{
	public struct Vertex
	{
		public Vector3 Position;
		public Vector4 Color;
		public Vector3 Normal;
		public Vector3 Texture;

		public Vertex(Vector3 position, Vector4? color = null, Vector3? texture = null, Vector3? normal = null)
		{
			Position = position;
			Color = color ?? new Vector4(1, 1, 1, 1);
			Normal = normal ?? new Vector3(0, 0, 0);
			Texture = texture ?? new Vector3(0, 0, 0);
		}

		public bool Equals(Vertex other)
		{
			return Position.Equals(other.Position) && Color.Equals(other.Color) && Normal.Equals(other.Normal) &&
			       Texture.Equals(other.Texture);
		}

		public override bool Equals(object? obj)
		{
			return obj is Vertex other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Position, Color, Normal, Texture);
		}

		public static bool operator ==(Vertex left, Vertex right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Vertex left, Vertex right)
		{
			return !left.Equals(right);
		}
	}

	public Vertex[] Vertices;
	public uint[] Elements;

	public Mesh(Vertex[] vertices, uint[] elements)
	{
		Vertices = vertices;
		Elements = elements;
	}

	private float[] ConstructVertexArray(ShaderObject shader)
	{
		var size = VertexSize(shader.GetInputs().Select(shader.GetInput).ToList());
		var value = new double[size * Vertices.Length];

		var j = 0;
		foreach (var v in Vertices)
		foreach (var input in shader.GetInputs().Select(shader.GetInput))
			switch (input)
			{
				case VertexInputType.PositionXY:
					value[j++] = v.Position.X;
					value[j++] = v.Position.Y;
					break;
				case VertexInputType.PositionXYZ:
					value[j++] = v.Position.X;
					value[j++] = v.Position.Y;
					value[j++] = v.Position.Z;
					break;
				case VertexInputType.ColorG:
					value[j++] = v.Color.Y;
					break;
				case VertexInputType.ColorRGB:
					value[j++] = v.Color.X;
					value[j++] = v.Color.Y;
					value[j++] = v.Color.Z;
					break;
				case VertexInputType.ColorRGBA:
					value[j++] = v.Color.X;
					value[j++] = v.Color.Y;
					value[j++] = v.Color.Z;
					value[j++] = v.Color.W;
					break;
#pragma warning disable 618
				case VertexInputType.ColorBGRA:
					value[j++] = v.Color.Z;
					value[j++] = v.Color.Y;
					value[j++] = v.Color.X;
					value[j++] = v.Color.W;
					break;
#pragma warning restore 618
				case VertexInputType.NormalXY:
					value[j++] = v.Normal.X;
					value[j++] = v.Normal.Y;
					break;
				case VertexInputType.NormalXYZ:
					value[j++] = v.Normal.X;
					value[j++] = v.Normal.Y;
					value[j++] = v.Normal.Z;
					break;
				case VertexInputType.TextureS:
					value[j++] = v.Texture.X;
					break;
				case VertexInputType.TextureST:
					value[j++] = v.Texture.X;
					value[j++] = v.Texture.Y;
					break;
				case VertexInputType.TextureSTV:
					value[j++] = v.Texture.X;
					value[j++] = v.Texture.Y;
					value[j++] = v.Texture.Z;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(shader), input, "Invalid input type.");
			}

		return value.Select(n => (float)n).ToArray();
	}

	private static int VertexSize(ICollection<VertexInputType> values)
	{
		return values.Sum(v => v switch
		{
			VertexInputType.PositionXY => 2,
			VertexInputType.PositionXYZ => 3,
			VertexInputType.ColorG => 1,
			VertexInputType.ColorRGB => 3,
			VertexInputType.ColorRGBA => 4,
#pragma warning disable 618
			VertexInputType.ColorBGRA => 4,
#pragma warning restore 618
			VertexInputType.NormalXY => 2,
			VertexInputType.NormalXYZ => 3,
			VertexInputType.TextureS => 1,
			VertexInputType.TextureST => 2,
			VertexInputType.TextureSTV => 3,
			_ => throw new ArgumentOutOfRangeException(nameof(values), v, null)
		});
	}

	public Drawable ConstructFor(ShaderObject shader)
	{
		var g = Graphics.Current;
		var vertexBuffer = g.NewBuffer(BufferTarget.VertexArray, ConstructVertexArray(shader));
		var elementBuffer = g.NewBuffer(BufferTarget.ElementArray, Elements);

		return g.NewDrawable(Elements.Length, vertexBuffer, elementBuffer);
	}

	[Obsolete("Use " + nameof(ConstructFor) + " instead")]
	public Drawable ConstructUnoptimisedFor(ShaderObject shader)
	{
		var g = Graphics.Current;
		var vertexBuffer = g.NewBuffer(BufferTarget.VertexArray, ConstructVertexArray(shader));
		var elementBuffer = g.NewBuffer(BufferTarget.ElementArray, Elements);

		return new Drawable(Elements.Length, vertexBuffer, elementBuffer);
	}
}