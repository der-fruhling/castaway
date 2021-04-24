using System.Collections.Generic;
using System.Text.RegularExpressions;
using Castaway.PirateSL;

namespace PirateSLC.Elements
{
    public class FragmentOutputElement : Element
    {
        protected override bool ToFragment => true;
        protected override bool ToConfig => true;
        
        private PSLType _type;
        private string _name;
        private int _target;
        
        public override bool Matches(string line) =>
            Regex.IsMatch(line, $@"^fragment output {ValidVarTypes} {ValidNames} is \d+$");

        public override void SetData(string[] parts)
        {
            _type = parts[2].ToType();
            _name = parts[3];
            _target = int.Parse(parts[5]);
        }

        public override void Apply(ref CodeGenerator g, List<string> errors)
        {
            g.GenFragOutput(_type, _name, _target);
        }
    }
}