using System;
using System.Collections.Generic;
using Castaway.Base;
using Castaway.Math;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Serilog;

namespace Castaway.Input;

public class GamepadInputSystem
{
	private static readonly ILogger Logger = CastawayGlobal.GetLogger();
	private int _activeJoystickNumber;
	private GamepadTypeImpl? _impl;

	public List<int> Available { get; } = new();
	public bool Valid => _impl != null;

	public int ActiveJoystickNumber
	{
		get => _activeJoystickNumber;
		set
		{
			_activeJoystickNumber = value;
			_impl = new GenericGamepadType(_activeJoystickNumber);
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
		GLFW.SetJoystickCallback(JoystickCallback);
	}

	private void JoystickCallback(int joystick, ConnectedState state)
	{
		switch (state)
		{
			case ConnectedState.Connected:
				Available.Add(joystick);
				ActiveJoystickNumber = joystick;
				Logger.Information("Connected gamepad {ID}", joystick);
				break;
			case ConnectedState.Disconnected:
				Available.Remove(joystick);
				Logger.Information("Disconnected gamepad {ID}", joystick);
				break;

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