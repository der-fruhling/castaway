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
    }
}