using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using static Castaway.PirateSL.PSLType;

namespace Castaway.PirateSL
{
    // ReSharper disable once InconsistentNaming
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class PSLValue
    {
        public object Value;
        public PSLType Type;

        public PSLValue(PSLType type, object value)
        {
            Value = value;
            Type = type;
        }

        protected PSLValue() : this(p_nul, null) {}

        public virtual string GLSL => Type switch
        {
            p_nul => null,
            p_int32 => Value.ToString(),
            p_uint32 => Value.ToString(),
            p_bool => Value.ToString(),
            p_float32 => Value.ToString(),
            p_vector2f32 => NewGLSLVector(Type, 2),
            p_vector3f32 => NewGLSLVector(Type, 3),
            p_vector4f32 => NewGLSLVector(Type, 4),
            p_matrix2f32 => NewGLSLMatrix(Type, 2, 2),
            p_matrix3f32 => NewGLSLMatrix(Type, 3, 3),
            p_matrix4f32 => NewGLSLMatrix(Type, 4, 4),
            _ => throw new ArgumentOutOfRangeException()
        };

        private string NewGLSLVector(PSLType type, int size)
        {
            var val = $"{type.ToGLSLName()}(";
            var ary = (object[]) Value;
            
            for (var i = 0; i < size; i++)
            {
                val += ary[i].ToString();
                if (i != size - 1) val += ",";
            }

            return val + ')';
        }
        
        private string NewGLSLMatrix(PSLType type, int width, int height)
        {
            var val = $"{type.ToGLSLName()}(";
            var ary = (object[][]) Value;

            for (var j = 0; j < height; j++)
            {
                for (var i = 0; i < width; i++)
                {
                    val += ary[j][i].ToString();
                    if (j != height - 1 && i != width - 1) val += ",";
                }
            }

            return val + ')';
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class PSLDirectValue : PSLValue
    {
        public PSLDirectValue(string value) : base(p_nul, value) { }

        public override string GLSL => Value as string;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class PSLOperatorValue : PSLValue
    {
        private readonly char _operator;
        private readonly object[] _values;
        
        public PSLOperatorValue(char op, params object[] values)
        {
            _operator = op;
            _values = values;
        }

        public override string GLSL
        {
            get
            {
                var l = _values.ToList();
                var s = "";
                
                while (l.Count > 0)
                {
                    var e = l[0];
                    if (e is PSLValue v) s += v.GLSL;
                    else s += e.ToString();
                    l.RemoveAt(0);
                    if (l.Count > 0) s += $" {_operator} ";
                }

                return s;
            }
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class PSLParenthesesValue : PSLValue
    {
        private readonly PSLValue _value;

        public PSLParenthesesValue(PSLValue value) => _value = value;

        public override string GLSL => $"{{{_value.GLSL}}}";
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class PSLValueEx
    {
        public static PSLValue PSL(this float o) => new PSLValue(p_float32, o);
        public static PSLValue PSL(this int o) => new PSLValue(p_int32, o);
        public static PSLValue PSL(this uint o) => new PSLValue(p_uint32, o);
        public static PSLValue PSL(this Vector2 o) => new PSLValue(p_vector2f32, new[] {o.X, o.Y});
        public static PSLValue PSL(this Vector3 o) => new PSLValue(p_vector2f32, new[] {o.X, o.Y, o.Z});
        public static PSLValue PSL(this Vector4 o) => new PSLValue(p_vector2f32, new[] {o.X, o.Y, o.Z, o.W});
    }
}