using System;
using System.Collections.Generic;
using GLFW;

namespace Castaway.OpenGL.Input
{
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
}