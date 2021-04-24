using System;
using System.Diagnostics.CodeAnalysis;
using static Castaway.PirateSL.PSLType;

namespace Castaway.PirateSL
{
    // ReSharper disable once InconsistentNaming
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum PSLType
    {
        p_nul,
        p_int32,
        p_uint32,
        p_bool,
        p_float32,
        p_vector2f32,
        p_vector3f32,
        p_vector4f32,
        p_matrix2f32,
        p_matrix3f32,
        p_matrix4f32
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class PSLTypeEx
    {
        public static string ToName(this PSLType type) =>
            type switch
            {
                p_nul => "nul",
                p_int32 => "int32",
                p_uint32 => "uint32",
                p_bool => "bool",
                p_float32 => "float32",
                p_vector2f32 => "vector2f32",
                p_vector3f32 => "vector3f32",
                p_vector4f32 => "vector4f32",
                p_matrix2f32 => "matrix2f32",
                p_matrix3f32 => "matrix3f32",
                p_matrix4f32 => "matrix4f32",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

        public static string ToGLSLName(this PSLType type) =>
            type switch
            {
                p_nul => "void",
                p_int32 => "int",
                p_uint32 => "uint",
                p_bool => "bool",
                p_float32 => "float",
                p_vector2f32 => "vec2",
                p_vector3f32 => "vec3",
                p_vector4f32 => "vec4",
                p_matrix2f32 => "mat2",
                p_matrix3f32 => "mat3",
                p_matrix4f32 => "mat4",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

        public static PSLType ToType(this string str) =>
            str switch
            {
                "nul" => p_nul,
                "void" => p_nul,
                "int" => p_int32,
                "int32" => p_int32,
                "uint" => p_uint32,
                "uint32" => p_uint32,
                "float" => p_float32,
                "float32" => p_float32,
                "vector2" => p_vector2f32,
                "vector3" => p_vector3f32,
                "vector4" => p_vector4f32,
                "matrix2" => p_matrix2f32,
                "matrix3" => p_matrix3f32,
                "matrix4" => p_matrix4f32,
                "vector2f32" => p_vector2f32,
                "vector3f32" => p_vector3f32,
                "vector4f32" => p_vector4f32,
                "matrix2f32" => p_matrix2f32,
                "matrix3f32" => p_matrix3f32,
                "matrix4f32" => p_matrix4f32,
                _ => throw new ArgumentOutOfRangeException(nameof(str), str, null)
            };
    }
}