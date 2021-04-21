using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Castaway.PirateSL;

namespace Castaway.PSLC.Elements
{
    public class UsesElement : Element
    {
        protected override bool ToVertex => true;
        protected override bool ToFragment => true;
        
        private const string ValidUses = @"(outPosition)";

        private string _name, _value;

        public override bool Matches(string line) =>
            Regex.IsMatch(line, $@"^uses {ValidNames}$");

        public override void SetData(string[] parts)
        {
            _name = parts[1];
            _value = _name switch
            {
                "outPosition" => "gl_Position",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public override void Apply(ref CodeGenerator g, List<string> errors)
        {
            switch (g)
            {
                case GLSLGenerator glsl:
                    glsl.GLSLObjects[_name] = _value;
                    g = g.GenComment(GLSLGenerator.Vertex, $"Using {_value} as {_name}");
                    g = g.GenComment(GLSLGenerator.Fragment, $"Using {_value} as {_name}");
                    break;
                default:
                    errors.Add("The current output language does not support `uses` statements.");
                    break;
            }
        }
    }
}