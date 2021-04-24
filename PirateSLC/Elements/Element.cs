using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Castaway.PirateSL;
using static Castaway.PirateSL.PSLType;

namespace PirateSLC.Elements
{
    public abstract class Element
    {
        protected const string ValidTypes = @"(int(?:32)?|uint(?:32)?|float(?:32)?|bool|nul|void|(?:vector|matrix)[2-4](?:f32)?)";
        protected const string ValidVarTypes = @"(int(?:32)?|uint(?:32)?|float(?:32)?|bool|(?:vector|matrix)[2-4](?:f32)?)";
        protected const string ValidNames = @"([A-Za-z_][A-Za-z0-9$_]*)";
        protected const string ValidLocations = @"(fragment|vertex)";

        protected virtual bool ToFragment => false;
        protected virtual bool ToVertex => false;
        protected virtual bool ToConfig => false;

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public string GLSLOutputs => $"{(ToVertex ? 'V' : ' ')}{(ToFragment ? 'F' : ' ')}{(ToConfig ? 'C' : ' ')}";
        
        public abstract bool Matches(string line);
        public abstract void SetData(string[] parts);
        public abstract void Apply(ref CodeGenerator g, List<string> errors);

        protected static PSLValue ProcessExpression(string str, bool allowVariables = true)
        {
            const string newVector2Regex = @"^new vector2(?:f32)?\((?<f1>.+),\s*(?<f2>.+)\)$";
            if (str.Matches(newVector2Regex))
            {
                var m = Regex.Match(str, newVector2Regex);
                var f = new object[] {
                    m.Groups["f1"].Value,
                    m.Groups["f2"].Value
                };
                return new PSLValue(p_vector2f32, f);
            }
            
            const string newVector3Regex = @"^new vector3(?:f32)?\((?<f1>.+),\s*(?<f2>.+),\s*(?<f3>.+)\)$";
            if (str.Matches(newVector3Regex))
            {
                var m = Regex.Match(str, newVector3Regex);
                var f = new object[] {
                    m.Groups["f1"].Value,
                    m.Groups["f2"].Value,
                    m.Groups["f3"].Value
                };
                return new PSLValue(p_vector3f32, f);
            }
            
            const string newVector4Regex = @"^new vector4(?:f32)?\((?<f1>.+),\s*(?<f2>.+),\s*(?<f3>.+),\s*(?<f4>.+)\)$";
            if (str.Matches(newVector4Regex))
            {
                var m = Regex.Match(str, newVector4Regex);
                var f = new object[] {
                    m.Groups["f1"].Value,
                    m.Groups["f2"].Value,
                    m.Groups["f3"].Value,
                    m.Groups["f4"].Value
                };
                return new PSLValue(p_vector4f32, f);
            }

            const string newFloat32 = @"^new float(?:32)?\((?<v>\d+(?:\.\d+)?)\)$";
            if (str.Matches(newFloat32))
            {
                var m = str.Match(newFloat32);
                var v = float.Parse(m.Groups["v"].Value);
                return new PSLValue(p_float32, v);
            }
            
            const string newUInt32 = @"^new uint(?:32)?\((?<v>\d+)\)$";
            if (str.Matches(newUInt32))
            {
                var m = str.Match(newUInt32);
                var v = uint.Parse(m.Groups["v"].Value);
                return new PSLValue(p_uint32, v);
            }

            const string newInt32 = @"^new int(?:32)?\((?<v>\d+(?:\.\d+)?)\)$";
            if (str.Matches(newInt32))
            {
                var m = str.Match(newInt32);
                var v = int.Parse(m.Groups["v"].Value);
                return new PSLValue(p_int32, v);
            }

            if (int.TryParse(str, out var intVal))
                return new PSLValue(p_int32, intVal);
            if (uint.TryParse(str, out var uintVal))
                return new PSLValue(p_uint32, uintVal);
            if (float.TryParse(str, out var floatVal))
                return new PSLValue(p_float32, floatVal);

            if (str.Matches($@"{ValidNames}\(.+\)")) return new PSLDirectValue(str);
            if (str.Matches(@"^\(.+\)$")) return new PSLParenthesesValue(ProcessExpression(str[1..^1]));
            
            if (str.Matches(@"^.+ [+\-*\/%] .+"))
            {
                var parts = str.Split(' ');
                var op = parts[1][0];
                var exps = new List<string>();
                
                for (var i = 0; i < parts.Length; i += 2) exps.Add(parts[i]);
                for (var i = 3; i < parts.Length; i += 2)
                    if (parts[i][0] != op)
                        throw new ApplicationException("Cannot combine operators. Use parentheses.");

                return new PSLOperatorValue(op, exps.Select(s => (object) ProcessExpression(s)).ToArray());
            }

            if (allowVariables) return new PSLDirectValue(str);
            throw new ApplicationException($"Invalid expression: \"{str}\"");
        }
    }
}