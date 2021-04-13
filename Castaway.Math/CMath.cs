using System;

namespace Castaway.Math
{
    public static class CMath
    {
        public static float Radians(float degrees) => degrees * (MathF.PI / 180f);
        public static float Degrees(float radians) => radians * (180f / MathF.PI);
    }
}