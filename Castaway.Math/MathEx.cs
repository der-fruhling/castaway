using System;

namespace Castaway.Math
{
    public static class MathEx
    {
        public static float ToRadians(float degrees) => degrees * MathF.PI / 180f;
        public static float ToDegrees(float radians) => radians * 180f / MathF.PI;
        public static Vector3 ToRadians(Vector3 degrees) => degrees * MathF.PI / 180f;
        public static Vector3 ToDegrees(Vector3 radians) => radians * 180f / MathF.PI;
        public static float Clamp(float x, float min, float max) => MathF.Max(MathF.Min(x, max), min);
        public static float Lerp(float a, float b, float p) => a + (b - a) * p;
    }
}