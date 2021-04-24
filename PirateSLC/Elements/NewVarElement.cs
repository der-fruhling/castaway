#nullable enable
using System.Collections.Generic;
using Castaway.PirateSL;

namespace PirateSLC.Elements
{
    public class NewVarElement : Element
    {
        private PSLType _type;
        private string _name;
        private PSLValue? _value;
        
        public override bool Matches(string line) =>
            line.Matches($@"^{ValidTypes} {ValidNames}( = .+)?$");

        public override void SetData(string[] parts)
        {
            _type = parts[0].ToType();
            _name = parts[1];
            if (parts.Length >= 4)
            {
                _value = ProcessExpression(string.Join(' ', parts[3..]), false);
            }
        }

        public override void Apply(ref CodeGenerator g, List<string> errors)
        {
            g.GenVariable(_type, _name, _value);
        }
    }
}