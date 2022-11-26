using System;
using System.Collections.Generic;
using Castaway.Rendering.Objects;

namespace Castaway.Rendering;

public class DrawBatch
{
	public delegate State Operation(Graphics g, State now);

	private readonly Queue<Operation> _operations = new();

	public void Add(Operation operation)
	{
		_operations.Enqueue(operation);
	}

	public StateBuilder Draw(Drawable drawable)
	{
		return new(new State(Graphics.Current), state => Add((graphics, now) =>
		{
			foreach (var e in Enum.GetValues<BufferTarget>())
				if (now.Buffers[e] != state.Buffers[e])
					if (state.Buffers[e] == null) graphics.UnbindBuffer(e);
					else state.Buffers[e]!.Bind();
			for (var i = 0; i < 32; i++)
				if (now.Textures[i] != state.Textures[i])
					if (state.Textures[i] == null) graphics.UnbindTexture(i);
					else state.Textures[i]!.Bind(i);
			if (now.Shader != state.Shader)
				state.Shader?.Bind();
			if (state.Shader == null)
				throw new InvalidOperationException("Need bound shader to draw.");
			graphics.Draw(state.Shader!, drawable);
			return new State(graphics);
		}));
	}

	public void Run()
	{
		while (_operations.TryDequeue(out var op))
			op(Graphics.Current, new State(Graphics.Current));
	}

	public readonly struct State
	{
		public readonly Graphics GL;
		public readonly ShaderObject? Shader;
		public readonly IReadOnlyDictionary<BufferTarget, BufferObject?> Buffers;
		public readonly TextureObject?[] Textures;

		public State(Graphics g)
		{
			GL = g;
			Shader = g.BoundShader;
			Buffers = g.BoundBuffers;
			Textures = g.BoundTextures;
		}

		public State(Graphics gl, ShaderObject? shader, IReadOnlyDictionary<BufferTarget, BufferObject?> buffers,
			TextureObject?[] textures)
		{
			GL = gl;
			Shader = shader;
			Buffers = buffers;
			Textures = textures;
		}
	}

	public ref struct StateBuilder
	{
		private readonly State _state;
		private readonly Action<State> _doneAction;

		internal StateBuilder(State state, Action<State> doneAction)
		{
			_state = state;
			_doneAction = doneAction;
		}

		public State Get()
		{
			return _state;
		}

		public void Done()
		{
			_doneAction(Get());
		}
	}
}