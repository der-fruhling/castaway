using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Castaway.PirateSL;

namespace PirateSLC.Elements
{
    public class VariableElement : Element
    {
        protected override bool ToVertex => true;
        protected override bool ToFragment => true;
        
        // ReSharper disable InconsistentNaming
        private const string ValidGLSLLocations = @"(fragment|vertex)";
        // ReSharper restore InconsistentNaming
        
        private string _where, _name;
        private PSLType _type;
        
        public override bool Matches(string line) =>
            Regex.IsMatch(line, $@"^{ValidLocations} {ValidVarTypes} {ValidNames}$");

        public override void SetData(string[] parts)
        {
            _where = parts[0];
            _type = parts[1].ToType();
            _name = parts[2];
        }

        public override void Apply(ref CodeGenerator g, List<string> errors)
        {
            switch (g)
            {
                case GLSLGenerator _:
                    if(!Regex.IsMatch(_where, ValidGLSLLocations))
                        errors.Add($"Invalid location {_where} for GLSL. (in [{_type.ToName()} {_name}])");
                    else
                    {
                        g.GenVariable(_where switch
                        {
                            "fragment" => GLSLGenerator.Fragment,
                            "vertex" => GLSLGenerator.Vertex,
                            _ => throw new ArgumentOutOfRangeException()
                        }, _type, _name);
                    }
                    break;
            }
        }
    }
}