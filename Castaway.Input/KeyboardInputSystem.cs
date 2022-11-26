using System;
using System.Collections.Generic;
using Castaway.Base;
using Castaway.Rendering;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Serilog;
using Window = OpenTK.Windowing.GraphicsLibraryFramework.Window;

namespace Castaway.Input;

public class KeyboardInputSystem
{
	private static readonly ILogger Logger = CastawayGlobal.GetLogger();
	private readonly GLFWCallbacks.KeyCallback _callback;
	private readonly Dictionary<Keys, ButtonState> _keys = new();

	internal KeyboardInputSystem()
	{
		unsafe
		{
			_callback = ReactKeyCallback;
		}
	}

	private ButtonState this[Keys key] => _keys.GetValueOrDefault(key, ButtonState.Up | ButtonState.NeverPressed);
	private bool this[Keys key, ButtonState state] => this[key].HasFlag(state);

	public void Init()
	{
		unsafe
		{
			var window = Graphics.Current.Window!.Native;
			GLFW.SetKeyCallback(window, _callback);
		}
	}

	public void Clear()
	{
		foreach (var key in _keys.Keys)
		{
			if (_keys[key].HasFlag(ButtonState.JustPressed)) _keys[key] &= ~ButtonState.JustPressed;
			if (_keys[key].HasFlag(ButtonState.JustReleased)) _keys[key] &= ~ButtonState.JustReleased;
		}
	}

	public bool IsDown(Keys key)
	{
		return this[key, ButtonState.Down];
	}

	public bool IsUp(Keys key)
	{
		return this[key, ButtonState.Up];
	}

	public bool WasJustPressed(Keys key)
	{
		return this[key, ButtonState.JustPressed];
	}

	public bool WasJustReleased(Keys key)
	{
		return this[key, ButtonState.JustReleased];
	}

	public bool WasNeverPressed(Keys key)
	{
		return this[key, ButtonState.NeverPressed];
	}

	public void SetNeverPressed(Keys key)
	{
		_keys[key] |= ButtonState.NeverPressed;
	}

	private unsafe void ReactKeyCallback(Window* ptr, Keys key, int code, InputAction state, KeyModifiers mods)
	{
		switch (state)
		{
			case InputAction.Press:
				if (!_keys.ContainsKey(key))
				{
					_keys[key] = ButtonState.Down | ButtonState.JustPressed;
				}
				else
				{
					_keys[key] |= ButtonState.Down;
					_keys[key] &= ~ButtonState.Up;
					_keys[key] |= ButtonState.JustPressed;
					_keys[key] &= ~ButtonState.NeverPressed;
				}

				break;
			case InputAction.Release:
				if (!_keys.ContainsKey(key))
				{
					_keys[key] = ButtonState.Up | ButtonState.JustReleased;
				}
				else
				{
					_keys[key] |= ButtonState.Up;
					_keys[key] &= ~ButtonState.Down;
					_keys[key] |= ButtonState.JustReleased;
					_keys[key] &= ~ButtonState.NeverPressed;
				}

				break;
			case InputAction.Repeat:
				return;
			default:
				throw new ArgumentOutOfRangeException(nameof(state), state, null);
		}

		Logger.Verbose("Key event: ({State} {Key}) = {NewState}", state, key, _keys[key]);
	}
}