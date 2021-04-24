using System.Collections.Generic;
using Castaway.PirateSL;

namespace PirateSLC.Elements
{
    public class IfElement : Element
    {
        private PSLValue _value;
        private bool _elseIf;
        
        public override bool Matches(string line) =>
            line.Matches(@"^(el)?if .+$");

        public override void SetData(string[] parts)
        {
            _value = ProcessExpression(string.Join(' ', parts[1..]));
            _elseIf = parts[0] == "elif";
        }

        public override void Apply(ref CodeGenerator g, List<string> errors)
        {
            if (_elseIf) g.GenElseIfBlock(_value);
            else g.GenIfBlock(_value);
        }
    }

    public class ElseElement : Element
    {
        public override bool Matches(string line) => line.Matches("^else$");
        public override void SetData(string[] parts) { }
        public override void Apply(ref CodeGenerator g, List<string> errors) => g.GenElseBlock();
    }
}