using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Castaway.PirateSL;

namespace Castaway.PSLC.Elements
{
    public class EntrypointStartElement : Element
    {
        protected override bool ToFragment => true;
        protected override bool ToVertex => true;

        private string _where;
        
        public override bool Matches(string line) => Regex.IsMatch(line, $@"^{ValidLocations} entrypoint$");

        public override void SetData(string[] parts)
        {
            _where = parts[0];
        }

        public override void Apply(ref CodeGenerator g, List<string> errors)
        {
            switch (g)
            {
                case GLSLGenerator _:
                    switch (_where)
                    {
                        case "vertex":
                            g.GenEntrypointStart(GLSLGenerator.Vertex);
                            break;
                        case "fragment":
                            g.GenEntrypointStart(GLSLGenerator.Fragment);
                            break;
                        default:
                            errors.Add($"Invalid location: \"{_where}\"");
                            break;
                    }
                    break;
            }
        }
    }
}