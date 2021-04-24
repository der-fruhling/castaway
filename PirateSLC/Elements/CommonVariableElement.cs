using System.Collections.Generic;
using System.Text.RegularExpressions;
using Castaway.PirateSL;

namespace PirateSLC.Elements
{
    public class CommonVariableElement : Element
    {
        protected override bool ToVertex => true;
        protected override bool ToFragment => true;
        
        private PSLType _type;
        private string _name;
        
        public override bool Matches(string line) =>
            Regex.IsMatch(line, $@"^common {ValidVarTypes} {ValidNames}$");

        public override void SetData(string[] parts)
        {
            _type = parts[1].ToType();
            _name = parts[2];
        }

        public override void Apply(ref CodeGenerator g, List<string> errors)
        {
            switch (g)
            {
                case GLSLGenerator _:
                    g.GenOutput(GLSLGenerator.Vertex, _type, _name);
                    g.GenInput(GLSLGenerator.Fragment, _type, _name);
                    break;
            }
        }
    }
}