using System;
using System.Collections.Generic;
using GLFW;

namespace Castaway.OpenGL.Input
{
    [Flags]
    public enum KeyState
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
        private readonly Dictionary<Keys, KeyState> _keys = new();

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
                if (_keys[key].HasFlag(KeyState.JustPressed)) _keys[key] &= ~KeyState.JustPressed;
                if (_keys[key].HasFlag(KeyState.JustReleased)) _keys[key] &= ~KeyState.JustReleased;
            }
        }

        private KeyState this[Keys key] => _keys.GetValueOrDefault(key, KeyState.Up | KeyState.NeverPressed);
        private bool this[Keys key, KeyState state] => this[key].HasFlag(state);
        public bool IsDown(Keys key) => this[key, KeyState.Down];
        public bool IsUp(Keys key) => this[key, KeyState.Up];
        public bool WasJustPressed(Keys key) => this[key, KeyState.JustPressed];
        public bool WasJustReleased(Keys key) => this[key, KeyState.JustReleased];
        public bool WasNeverPressed(Keys key) => this[key, KeyState.NeverPressed];
        public void SetNeverPressed(Keys key) => _keys[key] |= KeyState.NeverPressed;

        private void ReactKeyCallback(IntPtr ptr, Keys key, int code, InputState state, ModifierKeys mods)
        {
            if(state == InputState.Repeat) return;
            switch (state)
            {
                case InputState.Press:
                    Console.WriteLine($"+ {key}");
                    if (!_keys.ContainsKey(key))
                        _keys[key] = KeyState.Down | KeyState.JustPressed;
                    else
                    {
                        _keys[key] |= KeyState.Down;
                        _keys[key] &= ~KeyState.Up;
                        _keys[key] |= KeyState.JustPressed;
                        _keys[key] &= ~KeyState.NeverPressed;
                    }
                    break;
                case InputState.Release:
                    Console.WriteLine($"- {key}");
                    if (!_keys.ContainsKey(key))
                        _keys[key] = KeyState.Up | KeyState.JustReleased;
                    else
                    {
                        _keys[key] |= KeyState.Down;
                        _keys[key] &= ~KeyState.Up;
                        _keys[key] |= KeyState.JustPressed;
                        _keys[key] &= ~KeyState.NeverPressed;
                    }
                    break;
                case InputState.Repeat:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }

    public static class InputSystem
    {
        public static readonly KeyboardInputSystem Keyboard = new();

        public static void Init()
        {
            Keyboard.Init();
        }
    }
}