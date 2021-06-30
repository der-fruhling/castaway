using System;

namespace Castaway.Math
{
    public static class MathEx
    {
        public static float ToRadians(float degrees)
        {
            return degrees * MathF.PI / 180f;
        }

        public static double ToRadians(double degrees)
        {
            return degrees * System.Math.PI / 180f;
        }

        public static float ToDegrees(float radians)
        {
            return radians * 180f / MathF.PI;
        }

        public static double ToDegrees(double radians)
        {
            return radians * 180f / System.Math.PI;
        }

        public static Vector3 ToRadians(Vector3 degrees)
        {
            return degrees * System.Math.PI / 180f;
        }

        public static Vector3 ToDegrees(Vector3 radians)
        {
            return radians * 180f / System.Math.PI;
        }

        public static float Clamp(float x, float min, float max)
        {
            return MathF.Max(MathF.Min(x, max), min);
        }

        public static double Clamp(double x, double min, double max)
        {
            return System.Math.Max(System.Math.Min(x, max), min);
        }

        public static float Lerp(float a, float b, float p)
        {
            return a + (b - a) * p;
        }
    }
}