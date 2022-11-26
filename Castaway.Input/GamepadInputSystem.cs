using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Castaway.Base;
using Castaway.Math;
using GLFW;
using Serilog;

namespace Castaway.Input;

public class GamepadInputSystem
{
	private static readonly ILogger Logger = CastawayGlobal.GetLogger();
	private readonly JoystickCallback _joystickCallback;

	private int _active;
	private GamepadTypeImpl? _impl;
	public bool Locked = false;

	public GamepadInputSystem()
	{
		_joystickCallback = JoystickCallback;
	}

	public List<Joystick> Available { get; } = new();
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
			var ptr = Glfw.GetJoystickUserPointer(Active);
			return ptr == IntPtr.Zero ? null : (GamepadProperties*)ptr;
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
		Glfw.SetJoystickCallback(_joystickCallback);
		if (!Glfw.JoystickIsGamepad(Active)) return;
		unsafe
		{
			var data = Marshal.AllocHGlobal(sizeof(GamepadProperties));
			var p = (GamepadProperties*)data;
			p->Locked = false;
			p->Type = GamepadType.Generic;
			Glfw.SetJoystickUserPointer(Active, data);
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

	private void JoystickCallback(Joystick joystick, ConnectionStatus status)
	{
		switch (status)
		{
			case ConnectionStatus.Connected:
				unsafe
				{
					var data = Marshal.AllocHGlobal(sizeof(GamepadProperties));
					var p = (GamepadProperties*)data;
					p->Locked = false;
					p->Type = GamepadType.Generic;
					Glfw.SetJoystickUserPointer((int)joystick, data);
				}

				Available.Add(joystick);
				Active = (int)joystick;
				Logger.Information("Connected gamepad {ID}", (int)joystick);
				break;
			case ConnectionStatus.Disconnected:
				Available.Remove(joystick);
				var ptr = Glfw.GetJoystickUserPointer((int)joystick);
				Marshal.FreeHGlobal(ptr);
				Logger.Information("Disconnected gamepad {ID}", (int)joystick);
				break;
			case ConnectionStatus.Unknown:
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(status), status, null);
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