using System;
using Castaway.Math;

namespace Castaway.Rendering;

public static class CameraMath
{
    private static Matrix4 Ortho(float t, float b, float r, float l, float f, float n)
    {
        return new()
        {
            X = new Vector4(2 / (r - l), 0, 0, -((r + l) / (r - l))),
            Y = new Vector4(0, 2 / (t - b), 0, -((t + b) / (t - b))),
            Z = new Vector4(0, 0, -2 / (f - n), -((f + n) / (f - n))),
            W = new Vector4(0, 0, 0, 1)
        };
    }

    public static Matrix4 Ortho(Window window, float farCutoff, float nearCutoff, float scale = 1)
    {
        window.GetSize(out var w, out var h);
        var a = (float) w / h * scale;
        return Ortho(scale, -scale, a, -a, farCutoff, nearCutoff);
    }

    private static Matrix4 Persp(float t, float b, float r, float l, float f, float n)
    {
        return new()
        {
            X = new Vector4(2 * n / (r - l), 0, (r + l) / (r - l), 0),
            Y = new Vector4(0, 2 * n / (t - b), (t + b) / (t - b), 0),
            Z = new Vector4(0, 0, -((f + n) / (f - n)), -(2 * f * n / (f - n))),
            W = new Vector4(0, 0, -1, 0)
        };
    }

    private static Matrix4 Persp(float fov, float aspect, float f, float n, float scale)
    {
        var t = MathF.Tan(fov / 2) * n * scale;
        var b = -t;
        var r = t * aspect;
        var l = -t * aspect;
        return Persp(t, b, r, l, f, n);
    }

    public static Matrix4 Persp(Window window, float farCutoff, float nearCutoff,
        float verticalFov, float scale = 1)
    {
        window.GetSize(out var w, out var h);
        var a = (float) w / h;
        return Persp(verticalFov, a, farCutoff, nearCutoff, scale);
    }
}