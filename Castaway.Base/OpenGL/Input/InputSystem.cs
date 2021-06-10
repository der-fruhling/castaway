using System;
using System.Collections.Generic;
using Castaway.Math;
using GLFW;

namespace Castaway.OpenGL.Input
{
    [Flags]
    public enum ButtonState
    {
        Down = 1 << 0,
        Up = 1 << 1,
        JustPressed = 1 << 2,
        JustReleased = 1 << 3,
        NeverPressed = 1 << 4
    }
    
    public class KeyboardInputSystem
    {
        private readonly KeyCallback _callback;
        private readonly Dictionary<Keys, ButtonState> _keys = new();

        internal KeyboardInputSystem()
        {
            _callback = ReactKeyCallback;
        }

        public void Init()
        {
            var window = OpenGL.Get().BoundWindow!.Value.GlfwWindow;
            Glfw.SetKeyCallback(window, _callback);
        }

        public void Clear()
        {
            foreach (var key in _keys.Keys)
            {
                if (_keys[key].HasFlag(ButtonState.JustPressed)) _keys[key] &= ~ButtonState.JustPressed;
                if (_keys[key].HasFlag(ButtonState.JustReleased)) _keys[key] &= ~ButtonState.JustReleased;
            }
        }

        private ButtonState this[Keys key] => _keys.GetValueOrDefault(key, ButtonState.Up | ButtonState.NeverPressed);
        private bool this[Keys key, ButtonState state] => this[key].HasFlag(state);
        public bool IsDown(Keys key) => this[key, ButtonState.Down];
        public bool IsUp(Keys key) => this[key, ButtonState.Up];
        public bool WasJustPressed(Keys key) => this[key, ButtonState.JustPressed];
        public bool WasJustReleased(Keys key) => this[key, ButtonState.JustReleased];
        public bool WasNeverPressed(Keys key) => this[key, ButtonState.NeverPressed];
        public void SetNeverPressed(Keys key) => _keys[key] |= ButtonState.NeverPressed;

        private void ReactKeyCallback(IntPtr ptr, Keys key, int code, InputState state, ModifierKeys mods)
        {
            if(state == InputState.Repeat) return;
            switch (state)
            {
                case InputState.Press:
                    if (!_keys.ContainsKey(key))
                        _keys[key] = ButtonState.Down | ButtonState.JustPressed;
                    else
                    {
                        _keys[key] |= ButtonState.Down;
                        _keys[key] &= ~ButtonState.Up;
                        _keys[key] |= ButtonState.JustPressed;
                        _keys[key] &= ~ButtonState.NeverPressed;
                    }
                    Console.WriteLine($"+ {key} = {_keys[key].GetString()}");
                    break;
                case InputState.Release:
                    if (!_keys.ContainsKey(key))
                        _keys[key] = ButtonState.Up | ButtonState.JustReleased;
                    else
                    {
                        _keys[key] |= ButtonState.Up;
                        _keys[key] &= ~ButtonState.Down;
                        _keys[key] |= ButtonState.JustReleased;
                        _keys[key] &= ~ButtonState.NeverPressed;
                    }
                    Console.WriteLine($"- {key} = {_keys[key].GetString()}");
                    break;
                case InputState.Repeat:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }

    public class MouseInputSystem
    {
        private readonly MouseButtonCallback _mouseButtonCallback;
        private readonly MouseEnterCallback _mouseEnterCallback;

        private readonly Dictionary<MouseButton, ButtonState> _buttons = new();

        public float PositionScale = 1.0f;

        public MouseInputSystem()
        {
            _mouseButtonCallback = MouseButtonCallback;
            _mouseEnterCallback = MouseEnterCallback;
        }

        public void Init()
        {
            var w = OpenGL.Get().BoundWindow!.Value.GlfwWindow;
            Glfw.SetMouseButtonCallback(w, _mouseButtonCallback);
            Glfw.SetCursorEnterCallback(w, _mouseEnterCallback);
        }

        public void Clear()
        {
            foreach (var button in _buttons.Keys)
            {
                if (_buttons[button].HasFlag(ButtonState.JustPressed)) _buttons[button] &= ~ButtonState.JustPressed;
                if (_buttons[button].HasFlag(ButtonState.JustReleased)) _buttons[button] &= ~ButtonState.JustReleased;
            }
        }

        public Vector2 CursorPosition
        {
            get
            {
                var window = OpenGL.Get().BoundWindow!.Value.GlfwWindow;
                Glfw.GetCursorPosition(window, out var x, out var y);
                return new Vector2((float) x, (float) y) / PositionScale;
            }
        }

        public bool IsOverWindow { get; private set; }

        private ButtonState this[MouseButton button] => _buttons.GetValueOrDefault(button, ButtonState.Up | ButtonState.NeverPressed);
        private bool this[MouseButton button, ButtonState state] => this[button].HasFlag(state);
        public bool IsDown(MouseButton button) => this[button, ButtonState.Down];
        public bool IsUp(MouseButton button) => this[button, ButtonState.Up];
        public bool WasJustPressed(MouseButton button) => this[button, ButtonState.JustPressed];
        public bool WasJustReleased(MouseButton button) => this[button, ButtonState.JustReleased];
        public bool WasNeverPressed(MouseButton button) => this[button, ButtonState.NeverPressed];
        public void SetNeverPressed(MouseButton button) => _buttons[button] |= ButtonState.NeverPressed;

        public bool IsOver(float ax, float ay, float bx, float by)
        {
            var p = CursorPosition;
            return 
                p.X >= ax && p.X <= bx &&
                p.Y >= ay && p.Y <= by;
        }

        private void MouseButtonCallback(IntPtr window, MouseButton button, InputState state, ModifierKeys modifiers)
        {
            if(state == InputState.Repeat) return;
            switch (state)
            {
                case InputState.Press:
                    if (!_buttons.ContainsKey(button))
                        _buttons[button] = ButtonState.Down | ButtonState.JustPressed;
                    else
                    {
                        _buttons[button] |= ButtonState.Down;
                        _buttons[button] &= ~ButtonState.Up;
                        _buttons[button] |= ButtonState.JustPressed;
                        _buttons[button] &= ~ButtonState.NeverPressed;
                    }
                    Console.WriteLine($"+ {button} = {_buttons[button].GetString()}");
                    break;
                case InputState.Release:
                    if (!_buttons.ContainsKey(button))
                        _buttons[button] = ButtonState.Up | ButtonState.JustReleased;
                    else
                    {
                        _buttons[button] |= ButtonState.Up;
                        _buttons[button] &= ~ButtonState.Down;
                        _buttons[button] |= ButtonState.JustReleased;
                        _buttons[button] &= ~ButtonState.NeverPressed;
                    }
                    Console.WriteLine($"- {button} = {_buttons[button].GetString()}");
                    break;
                case InputState.Repeat:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void MouseEnterCallback(IntPtr window, bool entering)
        {
            IsOverWindow = entering;
        }
    }

    public static class InputSystem
    {
        public static readonly KeyboardInputSystem Keyboard = new();
        public static readonly MouseInputSystem Mouse = new();

        public static void Init()
        {
            Keyboard.Init();
            Mouse.Init();
        }

        public static void Clear()
        {
            Keyboard.Clear();
            Mouse.Clear();
        }

        public static string GetString(this ButtonState state)
        {
            var value = new List<string>();
            const char delimiter = ',';

            if(state.HasFlag(ButtonState.Down)) value.Add(nameof(ButtonState.Down));
            if(state.HasFlag(ButtonState.Up)) value.Add(nameof(ButtonState.Up));
            if(state.HasFlag(ButtonState.JustPressed)) value.Add(nameof(ButtonState.JustPressed));
            if(state.HasFlag(ButtonState.JustReleased)) value.Add(nameof(ButtonState.JustReleased));
            if(state.HasFlag(ButtonState.NeverPressed)) value.Add(nameof(ButtonState.NeverPressed));
            
            return string.Join(delimiter, value);
        }
    }
}