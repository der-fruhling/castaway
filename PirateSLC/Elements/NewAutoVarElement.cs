using System.Collections.Generic;
using Castaway.PirateSL;

namespace PirateSLC.Elements
{
    public class NewAutoVarElement : Element
    {
        private string _name;
        private PSLValue _value;

        public override bool Matches(string line) =>
            line.Matches($"^var {ValidNames} = .+$");

        public override void SetData(string[] parts)
        {
            _name = parts[1];
            _value = ProcessExpression(string.Join(' ', parts[3..]));
        }

        public override void Apply(ref CodeGenerator g, List<string> errors)
        {
            g.GenVariable(_name, _value);
        }
    }
}