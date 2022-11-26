using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Castaway.Base;
using Castaway.Math;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Serilog;

namespace Castaway.Input;

public class GamepadInputSystem
{
	private static readonly ILogger Logger = CastawayGlobal.GetLogger();
	private readonly GLFWCallbacks.JoystickCallback _joystickCallback;

	private int _active;
	private GamepadTypeImpl? _impl;
	public bool Locked = false;

	public GamepadInputSystem()
	{
		_joystickCallback = JoystickCallback;
	}

	public List<int> Available { get; } = new();
	public bool Valid => _impl != null;

	public int Active
	{
		get => _active;
		set
		{
			if (Locked) throw new InputSystemLockedException();
			_active = value;
			unsafe
			{
				Refresh(Properties);
			}
		}
	}

	private unsafe GamepadProperties* Properties
	{
		get
		{
			var ptr = GLFW.GetJoystickUserPointer(Active);
			return ptr == null ? null : (GamepadProperties*)ptr;
		}

		set
		{
			var newPtr = Marshal.AllocHGlobal(sizeof(GamepadProperties));
			var ptr = (IntPtr)value;
			for (var i = 0; i < sizeof(GamepadProperties); i++)
				Marshal.WriteByte(newPtr, i, Marshal.ReadByte(ptr, i));
		}
	}

	public Vector2 LeftStick => _impl?.LeftStick ?? throw new InvalidOperationException("No joystick active.");
	public Vector2 RightStick => _impl?.RightStick ?? throw new InvalidOperationException("No joystick active.");
	public float LeftTrigger => _impl?.LeftTrigger ?? throw new InvalidOperationException("No joystick active.");
	public float RightTrigger => _impl?.RightTrigger ?? throw new InvalidOperationException("No joystick active.");
	public bool LeftBumper => _impl?.LeftBumper ?? throw new InvalidOperationException("No joystick active.");
	public bool RightBumper => _impl?.RightBumper ?? throw new InvalidOperationException("No joystick active.");

	public bool LeftStickPress =>
		_impl?.LeftStickPress ?? throw new InvalidOperationException("No joystick active.");

	public bool RightStickPress =>
		_impl?.RightStickPress ?? throw new InvalidOperationException("No joystick active.");

	public bool A => _impl?.A ?? throw new InvalidOperationException("No joystick active.");
	public bool B => _impl?.B ?? throw new InvalidOperationException("No joystick active.");
	public bool X => _impl?.X ?? throw new InvalidOperationException("No joystick active.");
	public bool Y => _impl?.Y ?? throw new InvalidOperationException("No joystick active.");
	public bool Up => _impl?.Up ?? throw new InvalidOperationException("No joystick active.");
	public bool Down => _impl?.Down ?? throw new InvalidOperationException("No joystick active.");
	public bool Left => _impl?.Left ?? throw new InvalidOperationException("No joystick active.");
	public bool Right => _impl?.Right ?? throw new InvalidOperationException("No joystick active.");
	public bool Select => _impl?.Select ?? throw new InvalidOperationException("No joystick active.");
	public bool Start => _impl?.Start ?? throw new InvalidOperationException("No joystick active.");

	public void Init()
	{
		GLFW.SetJoystickCallback(_joystickCallback);
		if (!GLFW.JoystickIsGamepad(Active)) return;
		unsafe
		{
			var data = Marshal.AllocHGlobal(sizeof(GamepadProperties));
			var p = (GamepadProperties*)data;
			p->Locked = false;
			p->Type = GamepadType.Generic;
			GLFW.SetJoystickUserPointer(Active, p);
			Refresh(Properties);
		}
	}

	public unsafe void SetType(GamepadType type)
	{
		var p = Properties;
		if (p == null) throw new InvalidOperationException("Gamepad must be valid.");
		if (p->Locked) throw new GamepadLockedException();
		p->Type = type;
		Properties = p;
		Refresh(p);
	}

	private unsafe void Refresh(GamepadProperties* properties)
	{
		_impl = properties->Type switch
		{
			GamepadType.Generic => new GenericGamepadType(Active),
			_ => throw new ArgumentOutOfRangeException()
		};
	}

	private void JoystickCallback(int joystick, ConnectedState state)
	{
		switch (state)
		{
			case ConnectedState.Connected:
				unsafe
				{
					var data = Marshal.AllocHGlobal(sizeof(GamepadProperties));
					var p = (GamepadProperties*)data;
					p->Locked = false;
					p->Type = GamepadType.Generic;
					GLFW.SetJoystickUserPointer(joystick, p);
				}

				Available.Add(joystick);
				Active = joystick;
				Logger.Information("Connected gamepad {ID}", joystick);
				break;
			case ConnectedState.Disconnected:
				unsafe
				{
					Available.Remove(joystick);
					var ptr = GLFW.GetJoystickUserPointer(joystick);
					Marshal.FreeHGlobal((IntPtr)ptr);
					Logger.Information("Disconnected gamepad {ID}", joystick);
					break;
				}

			default:
				throw new ArgumentOutOfRangeException(nameof(state), state, null);
		}
	}

	public void Read()
	{
		_impl?.Read();
	}

	public override string ToString()
	{
		return $"{nameof(LeftStick)}: ({LeftStick}), " +
		       $"{nameof(RightStick)}: ({RightStick}), " +
		       $"{nameof(LeftTrigger)}: {LeftTrigger}, " +
		       $"{nameof(RightTrigger)}: {RightTrigger}, " +
		       $"{nameof(LeftBumper)}: {LeftBumper}, " +
		       $"{nameof(RightBumper)}: {RightBumper}, " +
		       $"{nameof(LeftStickPress)}: {LeftStickPress}, " +
		       $"{nameof(RightStickPress)}: {RightStickPress}, " +
		       $"{nameof(A)}: {A}, " +
		       $"{nameof(B)}: {B}, " +
		       $"{nameof(X)}: {X}, " +
		       $"{nameof(Y)}: {Y}, " +
		       $"{nameof(Up)}: {Up}, " +
		       $"{nameof(Down)}: {Down}, " +
		       $"{nameof(Left)}: {Left}, " +
		       $"{nameof(Right)}: {Right}, " +
		       $"{nameof(Select)}: {Select}, " +
		       $"{nameof(Start)}: {Start}";
	}
}