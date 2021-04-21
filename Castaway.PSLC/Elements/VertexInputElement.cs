using System.Collections.Generic;
using System.Text.RegularExpressions;
using Castaway.PirateSL;

namespace Castaway.PSLC.Elements
{
    public class VertexInputElement : Element
    {
        protected override bool ToVertex => true;
        protected override bool ToConfig => true;

        private const string ValidTargets = @"(Position|Colou?r|Texture|Normal)";

        private PSLType _type;
        private string _name, _target;
        
        public override bool Matches(string line) => 
            Regex.IsMatch(line, $@"^vertex input {ValidVarTypes} {ValidNames} is {ValidTargets}$");
        
        public override void Apply(ref CodeGenerator g, List<string> errors)
        {
            g = g.GenVertInput(_type, _name, _target);
        }

        public override void SetData(string[] parts)
        {
            _type = parts[2].ToType();
            _name = parts[3];
            _target = parts[5];
        }
    }
}