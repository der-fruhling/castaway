using System;
using System.Collections.Generic;
using System.Linq;
using Castaway.Base;
using Castaway.Math;
using Castaway.Rendering;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Serilog;
using Window = OpenTK.Windowing.GraphicsLibraryFramework.Window;

namespace Castaway.Input;

public class MouseInputSystem
{
	private static readonly ILogger Logger = CastawayGlobal.GetLogger();

	private readonly Dictionary<MouseButton, ButtonState> _buttons = new();
	private readonly GLFWCallbacks.MouseButtonCallback _mouseButtonCallback;
	private readonly GLFWCallbacks.CursorEnterCallback _mouseEnterCallback;

	private Vector2 _lastCursorPos;

	private bool _rawInput, _fakeRaw;

	public MouseInputSystem()
	{
		unsafe
		{
			_mouseButtonCallback = MouseButtonCallback;
			_mouseEnterCallback = MouseEnterCallback;
		}
	}

	public bool RawInput
	{
		get => _rawInput;
		set
		{
			unsafe
			{
				_rawInput = value;
				var w = GLFW.GetCurrentContext();

				GLFW.GetWindowSize(w, out var wid, out var hei);
				CursorPosition = new Vector2(wid / 2.0, hei / 2.0);
				_lastCursorPos = CursorPosition;

				if (GLFW.RawMouseMotionSupported())
					GLFW.SetInputMode(w, RawMouseMotionAttribute.RawMouseMotion, value);
				else
					_fakeRaw = value;

				GLFW.SetInputMode(w, CursorStateAttribute.Cursor,
					value ? CursorModeValue.CursorDisabled : CursorModeValue.CursorNormal);
			}
		}
	}

	public Vector2 CursorMovement
	{
		get
		{
			unsafe
			{
				var v = CursorPosition - _lastCursorPos;

				if (!_fakeRaw) return v;
				GLFW.GetWindowSize(GLFW.GetCurrentContext(), out var wid, out var hei);
				CursorPosition = new Vector2(wid / 2.0, hei / 2.0);

				return v;
			}
		}
	}

	public Vector2 CursorPosition
	{
		get
		{
			unsafe
			{
				var window = Graphics.BoundWindows.Single().Native;
				GLFW.GetCursorPos(window, out var x, out var y);
				return new Vector2(x, y);
			}
		}

		set
		{
			unsafe
			{
				var window = Graphics.BoundWindows.Single().Native;
				GLFW.SetCursorPos(window, value.X, value.Y);
			}
		}
	}

	public bool IsOverWindow { get; private set; }

	private ButtonState this[MouseButton button] =>
		_buttons.GetValueOrDefault(button, ButtonState.Up | ButtonState.NeverPressed);

	private bool this[MouseButton button, ButtonState state] => this[button].HasFlag(state);

	public void Init()
	{
		unsafe
		{
			var w = Graphics.Current.Window!.Native;
			GLFW.SetMouseButtonCallback(w, _mouseButtonCallback);
			GLFW.SetCursorEnterCallback(w, _mouseEnterCallback);
			_lastCursorPos = CursorPosition;
		}
	}

	public void Clear()
	{
		foreach (var button in _buttons.Keys)
		{
			if (_buttons[button].HasFlag(ButtonState.JustPressed)) _buttons[button] &= ~ButtonState.JustPressed;
			if (_buttons[button].HasFlag(ButtonState.JustReleased)) _buttons[button] &= ~ButtonState.JustReleased;
		}

		_lastCursorPos = CursorPosition;
	}

	public bool IsDown(MouseButton button)
	{
		return this[button, ButtonState.Down];
	}

	public bool IsUp(MouseButton button)
	{
		return this[button, ButtonState.Up];
	}

	public bool WasJustPressed(MouseButton button)
	{
		return this[button, ButtonState.JustPressed];
	}

	public bool WasJustReleased(MouseButton button)
	{
		return this[button, ButtonState.JustReleased];
	}

	public bool WasNeverPressed(MouseButton button)
	{
		return this[button, ButtonState.NeverPressed];
	}

	public void SetNeverPressed(MouseButton button)
	{
		_buttons[button] |= ButtonState.NeverPressed;
	}

	public bool IsOver(float ax, float ay, float bx, float by)
	{
		var p = CursorPosition;
		return
			p.X >= ax && p.X <= bx &&
			p.Y >= ay && p.Y <= by;
	}

	public bool IsOver(Vector2 a, Vector2 b)
	{
		return IsOver((float)a.X, (float)a.Y, (float)b.X, (float)b.Y);
	}

	private unsafe void MouseButtonCallback(Window* window, MouseButton button, InputAction state,
		KeyModifiers modifiers)
	{
		switch (state)
		{
			case InputAction.Press:
				if (!_buttons.ContainsKey(button))
				{
					_buttons[button] = ButtonState.Down | ButtonState.JustPressed;
				}
				else
				{
					_buttons[button] |= ButtonState.Down;
					_buttons[button] &= ~ButtonState.Up;
					_buttons[button] |= ButtonState.JustPressed;
					_buttons[button] &= ~ButtonState.NeverPressed;
				}

				break;
			case InputAction.Release:
				if (!_buttons.ContainsKey(button))
				{
					_buttons[button] = ButtonState.Up | ButtonState.JustReleased;
				}
				else
				{
					_buttons[button] |= ButtonState.Up;
					_buttons[button] &= ~ButtonState.Down;
					_buttons[button] |= ButtonState.JustReleased;
					_buttons[button] &= ~ButtonState.NeverPressed;
				}

				break;
			case InputAction.Repeat:
				return;
			default:
				throw new ArgumentOutOfRangeException(nameof(state), state, null);
		}

		Logger.Verbose("Mouse click event: ({State} {Key}) = {NewState} at {Position}", state, button,
			_buttons[button], CursorPosition);
	}

	private unsafe void MouseEnterCallback(Window* window, bool entering)
	{
		IsOverWindow = entering;
		Logger.Verbose("Mouse is over window = {Value}", entering);
	}
}