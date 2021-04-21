using System.Collections.Generic;
using Castaway.PirateSL;

namespace Castaway.PSLC.Elements
{
    public class SkipElement : Element
    {
        public override bool Matches(string line) =>
            line.Matches(@"^skip$");

        public override void SetData(string[] parts) { }

        public override void Apply(ref CodeGenerator g, List<string> errors)
        {
            g.GenSkip();
        }
    }
}