using System.Collections.Generic;
using Castaway.PirateSL;

namespace Castaway.PSLC.Elements
{
    public class FunctionReturnElement : Element
    {
        private PSLValue _value = new PSLValue(PSLType.p_nul, null);
        
        public override bool Matches(string line) =>
            line.Matches(@"return(\s+.+)?");

        public override void SetData(string[] parts)
        {
            _value = ProcessExpression(string.Join(' ', parts[1..]));
        }

        public override void Apply(ref CodeGenerator g, List<string> errors)
        {
            g.GenReturn(_value);
        }
    }
}