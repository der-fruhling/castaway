using System.Collections.Generic;
using Castaway.PirateSL;

namespace Castaway.PSLC.Elements
{
    public class UniformPropertyElement : Element
    {
        private PSLType _type;
        private string _name, _target;
        
        public override bool Matches(string line) =>
            line.Matches($@"^uniform {ValidVarTypes} {ValidNames} is [A-Za-z\[\]0-9.]+$");

        public override void SetData(string[] parts)
        {
            _type = parts[1].ToType();
            _name = parts[2];
            _target = parts[4];
        }

        public override void Apply(ref CodeGenerator g, List<string> errors)
        {
            g.GenUniform(_type, _name, _target);
        }
    }
}