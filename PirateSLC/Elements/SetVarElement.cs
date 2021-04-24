using System.Collections.Generic;
using System.Text.RegularExpressions;
using Castaway.PirateSL;

namespace PirateSLC.Elements
{
    public class SetVarElement : Element
    {
        protected override bool ToFragment => true;
        protected override bool ToVertex => true;

        private string _to;
        private PSLValue _from;

        public override bool Matches(string line)
            => Regex.IsMatch(line, $@"^{ValidNames} = .+$");

        public override void SetData(string[] parts)
        {
            _to = parts[0];
            _from = ProcessExpression(string.Join(' ', parts[2..]));
        }

        public override void Apply(ref CodeGenerator g, List<string> errors)
        {
            g.GenVariableSet(_to, _from);
        }
    }
}