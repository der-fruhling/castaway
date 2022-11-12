using System;
using Castaway.Math;
using GLFW;

namespace Castaway.Input;

internal class GenericGamepadType : GamepadTypeImpl
{
    private GamePadState _state;

    public GenericGamepadType(params int[] joysticks) : base(joysticks)
    {
    }

    protected override float DeadZone => 0.05f;

    public override Vector2 LeftStick =>
        new(ApplyDeadZone(_state.GetAxis(GamePadAxis.LeftX)), ApplyDeadZone(_state.GetAxis(GamePadAxis.LeftY)));

    public override Vector2 RightStick =>
        new(ApplyDeadZone(_state.GetAxis(GamePadAxis.RightX)), ApplyDeadZone(_state.GetAxis(GamePadAxis.RightY)));

    public override float LeftTrigger => (_state.GetAxis(GamePadAxis.LeftTrigger) + 1f) / 2f;
    public override float RightTrigger => (_state.GetAxis(GamePadAxis.RightTrigger) + 1f) / 2f;
    public override bool LeftBumper => _state.GetButtonState(GamePadButton.LeftBumper) == InputState.Press;
    public override bool RightBumper => _state.GetButtonState(GamePadButton.RightBumper) == InputState.Press;
    public override bool LeftStickPress => _state.GetButtonState(GamePadButton.LeftThumb) == InputState.Press;
    public override bool RightStickPress => _state.GetButtonState(GamePadButton.RightThumb) == InputState.Press;
    public override bool A => _state.GetButtonState(GamePadButton.A) == InputState.Press;
    public override bool B => _state.GetButtonState(GamePadButton.B) == InputState.Press;
    public override bool X => _state.GetButtonState(GamePadButton.X) == InputState.Press;
    public override bool Y => _state.GetButtonState(GamePadButton.Y) == InputState.Press;
    public override bool Up => _state.GetButtonState(GamePadButton.DpadUp) == InputState.Press;
    public override bool Down => _state.GetButtonState(GamePadButton.DpadDown) == InputState.Press;
    public override bool Left => _state.GetButtonState(GamePadButton.DpadLeft) == InputState.Press;
    public override bool Right => _state.GetButtonState(GamePadButton.DpadRight) == InputState.Press;
    public override bool Select => _state.GetButtonState(GamePadButton.Back) == InputState.Press;
    public override bool Start => _state.GetButtonState(GamePadButton.Start) == InputState.Press;

    internal override void Read()
    {
        if (!Glfw.JoystickIsGamepad(Joysticks[0]))
            throw new InvalidOperationException($"Joystick {Joysticks[0]} is not a gamepad.");
        if (!Glfw.GetGamepadState(Joysticks[0], out _state))
            throw new InvalidOperationException("Failed to get gamepad state.");
    }
}