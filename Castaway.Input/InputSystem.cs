using System.Collections.Generic;

namespace Castaway.Input;

public static class InputSystem
{
    public static readonly KeyboardInputSystem Keyboard = new();
    public static readonly MouseInputSystem Mouse = new();
    public static readonly GamepadInputSystem Gamepad = new();

    public static void Init()
    {
        Keyboard.Init();
        Mouse.Init();
        Gamepad.Init();
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

        if (state.HasFlag(ButtonState.Down)) value.Add(nameof(ButtonState.Down));
        if (state.HasFlag(ButtonState.Up)) value.Add(nameof(ButtonState.Up));
        if (state.HasFlag(ButtonState.JustPressed)) value.Add(nameof(ButtonState.JustPressed));
        if (state.HasFlag(ButtonState.JustReleased)) value.Add(nameof(ButtonState.JustReleased));
        if (state.HasFlag(ButtonState.NeverPressed)) value.Add(nameof(ButtonState.NeverPressed));

        return string.Join(delimiter, value);
    }
}