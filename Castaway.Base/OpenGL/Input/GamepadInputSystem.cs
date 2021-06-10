using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Castaway.Math;
using GLFW;
using Exception = System.Exception;

namespace Castaway.OpenGL.Input
{
    public enum GamepadType
    {
        Generic
    }

    public abstract class GamepadTypeImpl
    {
        protected readonly int[] Joysticks;
        protected abstract float DeadZone { get; }
        
        protected GamepadTypeImpl(params int[] joysticks)
        {
            Joysticks = joysticks;
        }

        internal abstract void Read();

        protected virtual float ApplyDeadZone(float x) => x >= DeadZone || x <= -DeadZone ? x :0;

        public abstract Vector2 LeftStick { get; }
        public abstract Vector2 RightStick { get; }
        public abstract float LeftTrigger { get; }
        public abstract float RightTrigger { get; }
        public abstract bool LeftBumper { get; }
        public abstract bool RightBumper { get; }
        public abstract bool LeftStickPress { get; }
        public abstract bool RightStickPress { get; }
        public abstract bool A { get; }
        public abstract bool B { get; }
        public abstract bool X { get; }
        public abstract bool Y { get; }
        public abstract bool Up { get; }
        public abstract bool Down { get; }
        public abstract bool Left { get; }
        public abstract bool Right { get; }
        public abstract bool Select { get; }
        public abstract bool Start { get; }
    }

    internal class GenericGamepadType : GamepadTypeImpl
    {
        private GamePadState _state;

        public GenericGamepadType(params int[] joysticks) : base(joysticks)
        {
        }

        protected override float DeadZone => 0.05f;

        internal override void Read()
        {
            if (!Glfw.JoystickIsGamepad(Joysticks[0]))
                throw new InvalidOperationException($"Joystick {Joysticks[0]} is not a gamepad.");
            if (!Glfw.GetGamepadState(Joysticks[0], out _state))
                throw new InvalidOperationException("Failed to get gamepad state.");
        }

        public override Vector2 LeftStick => new(ApplyDeadZone(_state.GetAxis(GamePadAxis.LeftX)), ApplyDeadZone(_state.GetAxis(GamePadAxis.LeftY)));
        public override Vector2 RightStick => new(ApplyDeadZone(_state.GetAxis(GamePadAxis.RightX)), ApplyDeadZone(_state.GetAxis(GamePadAxis.RightY)));
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
    }

    public struct GamepadProperties
    {
        public bool Locked;
        public GamepadType Type;
    }
    
    public class GamepadInputSystem
    {
        private readonly JoystickCallback _joystickCallback;
        private GamepadTypeImpl? _impl;

        public List<Joystick> Available { get; } = new();
        public bool Locked = false;
        public bool Valid => _impl != null;
        
        private int _active;
        public int Active
        {
            get => _active;
            set
            {
                if (Locked) throw new InputSystemLockedException();
                _active = value;
                unsafe { Refresh(Properties); }
            }
        }

        private unsafe GamepadProperties* Properties
        {
            get
            {
                var ptr = Glfw.GetJoystickUserPointer(Active);
                return ptr == IntPtr.Zero ? null : (GamepadProperties*) ptr;
            }

            set
            {
                var newPtr = Marshal.AllocHGlobal(sizeof(GamepadProperties));
                var ptr = (IntPtr) value;
                for (var i = 0; i < sizeof(GamepadProperties); i++)
                    Marshal.WriteByte(newPtr, i, Marshal.ReadByte(ptr, i));
            }
        }

        public GamepadInputSystem()
        {
            _joystickCallback = JoystickCallback;
        }

        public void Init()
        {
            Glfw.SetJoystickCallback(_joystickCallback);
            if (!Glfw.JoystickIsGamepad(Active)) return;
            unsafe
            {
                var data = Marshal.AllocHGlobal(sizeof(GamepadProperties));
                var p = (GamepadProperties*) data;
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
                        var p = (GamepadProperties*) data;
                        p->Locked = false;
                        p->Type = GamepadType.Generic;
                        Glfw.SetJoystickUserPointer((int) joystick, data);
                    }
                    Available.Add(joystick);
                    Active = (int) joystick;
                    break;
                case ConnectionStatus.Disconnected:
                    Available.Remove(joystick);
                    var ptr = Glfw.GetJoystickUserPointer((int) joystick);
                    Marshal.FreeHGlobal(ptr);
                    break;
                case ConnectionStatus.Unknown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        public void Read() => _impl?.Read();
        public Vector2 LeftStick => _impl?.LeftStick ?? throw new InvalidOperationException("No joystick active.");
        public Vector2 RightStick => _impl?.RightStick ?? throw new InvalidOperationException("No joystick active.");
        public float LeftTrigger => _impl?.LeftTrigger ?? throw new InvalidOperationException("No joystick active.");
        public float RightTrigger => _impl?.RightTrigger ?? throw new InvalidOperationException("No joystick active.");
        public bool LeftBumper => _impl?.LeftBumper ?? throw new InvalidOperationException("No joystick active.");
        public bool RightBumper => _impl?.RightBumper ?? throw new InvalidOperationException("No joystick active.");
        public bool LeftStickPress => _impl?.LeftStickPress ?? throw new InvalidOperationException("No joystick active.");
        public bool RightStickPress => _impl?.RightStickPress ?? throw new InvalidOperationException("No joystick active.");
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

    public class GamepadLockedException : Exception { }
    public class InvalidGamepadException : Exception { }
}