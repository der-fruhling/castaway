using System;
using Castaway.OpenGL;

namespace Castaway.Math
{
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

        public static Matrix4 Ortho(OpenGL.OpenGL g, Window window, float farCutoff, float nearCutoff)
        {
            var (w, h) = g.GetWindowSize(window);
            var a = (float) w / h;
            return Ortho(1, -1, a, -a, farCutoff, nearCutoff);
        }

        private static Matrix4 Persp(float t, float b, float r, float l, float f, float n)
        {
            return new()
            {
                X = new Vector4((2 * n) / (r - l), 0, (r + l) / (r - l), 0),
                Y = new Vector4(0, (2 * n) / (t - b), (t + b) / (t - b), 0),
                Z = new Vector4(0, 0, -((f + n) / (f - n)), -((2 * f * n) / (f - n))),
                W = new Vector4(0, 0, -1, 0)
            };
        }

        private static Matrix4 Persp(float fov, float aspect, float f, float n)
        {
            var t = MathF.Tan(fov / 2) * n;
            var b = -t;
            var r = t * aspect;
            var l = -t * aspect;
            return Persp(t, b, r, l, f, n);
        }

        public static Matrix4 Persp(OpenGL.OpenGL g, Window window, float farCutoff, float nearCutoff,
            float verticalFov)
        {
            var (w, h) = g.GetWindowSize(window);
            var a = (float) w / h;
            return Persp(verticalFov, a, farCutoff, nearCutoff);
        }
    }
}