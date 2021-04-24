#nullable enable
using System.Collections.Generic;
using Castaway.PirateSL;

namespace PirateSLC.Elements
{
    public class UniformElement : Element
    {
        private PSLType _type;
        private string _name;
        private PSLValue? _value;
        
        public override bool Matches(string line) =>
            line.Matches($@"^uniform {ValidVarTypes} {ValidNames}( = .+)?$");

        public override void SetData(string[] parts)
        {
            _type = parts[1].ToType();
            _name = parts[2];
            if (parts.Length >= 5)
            {
                _value = ProcessExpression(string.Join(' ', parts[4..]));
            }
        }

        public override void Apply(ref CodeGenerator g, List<string> errors)
        {
            g.GenUniform(_type, _name, _value);
        }
    }
}