using System;
using System.Runtime.InteropServices;
using Castaway.Math;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Castaway.Input;

internal class GenericGamepadType : GamepadTypeImpl
{
	private const int
		AxisLeftX = 0,
		AxisLeftY = 1,
		AxisRightX = 2,
		AxisRightY = 3,
		AxisLeftTrigger = 4,
		AxisRightTrigger = 5;

	private const int
		ButtonA = 0,
		ButtonB = 1,
		ButtonX = 2,
		ButtonY = 3,
		ButtonLeftBumper = 4,
		ButtonRightBumper = 5,
		ButtonSelect = 6,
		ButtonStart = 7,
		ButtonLeftStick = 9,
		ButtonRightStick = 10,
		ButtonDUp = 11,
		ButtonDRight = 12,
		ButtonDDown = 13,
		ButtonDLeft = 14;

	private float[] _axes = new float[6];
	private bool[] _buttons = new bool[15];

	public GenericGamepadType(int joystick) : base(joystick)
	{
	}

	protected override float DeadZone => 0.05f;

	public override Vector2 LeftStick =>
		new(ApplyDeadZone(_axes[AxisLeftX]), ApplyDeadZone(_axes[AxisLeftY]));

	public override Vector2 RightStick =>
		new(ApplyDeadZone(_axes[AxisRightX]), ApplyDeadZone(_axes[AxisRightY]));

	public override float LeftTrigger => (_axes[AxisLeftTrigger] + 1f) / 2f;
	public override float RightTrigger => (_axes[AxisRightTrigger] + 1f) / 2f;
	public override bool LeftBumper => _buttons[ButtonLeftBumper];
	public override bool RightBumper => _buttons[ButtonRightBumper];
	public override bool LeftStickPress => _buttons[ButtonLeftStick];
	public override bool RightStickPress => _buttons[ButtonRightStick];
	public override bool A => _buttons[ButtonA];
	public override bool B => _buttons[ButtonB];
	public override bool X => _buttons[ButtonX];
	public override bool Y => _buttons[ButtonY];
	public override bool Up => _buttons[ButtonDUp];
	public override bool Down => _buttons[ButtonDDown];
	public override bool Left => _buttons[ButtonDLeft];
	public override bool Right => _buttons[ButtonDRight];
	public override bool Select => _buttons[ButtonSelect];
	public override bool Start => _buttons[ButtonStart];

	internal override void Read()
	{
		if (!GLFW.JoystickIsGamepad(Joystick))
			return;
		if (!GLFW.GetGamepadState(Joystick, out var state))
			throw new InvalidOperationException("Failed to get gamepad state.");

		unsafe
		{
			Marshal.Copy((IntPtr)state.Axes, _axes, 0, 6);
			for (var i = 0; i < 15; i++) _buttons[i] = state.Buttons[i] > 0;
		}
	}
}