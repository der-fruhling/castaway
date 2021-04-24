using System.Collections.Generic;
using System.Text.RegularExpressions;
using Castaway.PirateSL;

namespace Castaway.PSLC.Elements
{
    public class BlockEndElement : Element
    {
        protected override bool ToFragment => true;
        protected override bool ToVertex => true;
        protected override bool ToConfig => true;
        
        public override bool Matches(string line) => Regex.IsMatch(line, "^end$");
        public override void SetData(string[] parts) {}
        public override void Apply(ref CodeGenerator g, List<string> errors) => g = g.GenBlockEnd();
    }
}