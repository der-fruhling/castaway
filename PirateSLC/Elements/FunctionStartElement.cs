using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Castaway.PirateSL;

namespace PirateSLC.Elements
{
    public class FunctionStartElement : Element
    {
        private PSLType _returnType;
        private string _name, _location;
        private FuncParam[] _params = new FuncParam[0];
        
        public override bool Matches(string line) =>
            line.Matches($@"^{ValidLocations}\s+{ValidTypes}\s+{ValidNames}\s*(.+)$");

        public override void SetData(string[] parts)
        {
            _location = parts[0];
            _returnType = parts[1].ToType();
            _name = parts[2][..parts[2].IndexOf('(')];
            var @params = parts[2][(parts[2].IndexOf('(') + 1)..parts[2].IndexOf(')')];
            @params = parts[3..].Aggregate(@params, (current, p) => current + p);
            if (@params.Length > 0)
            {
                _params = Regex.Split(@params, @"\s*,\s*")
                    .Select(s => Regex.Split(s, @"\s*:\s*"))
                    .Select(a => new FuncParam {Name = a[0], Type = a[1].ToType()})
                    .ToArray();
            }
        }

        public override void Apply(ref CodeGenerator g, List<string> errors)
        {
            g.GenFunctionStart(_location switch
            {
                "vertex" => GLSLGenerator.Vertex,
                "fragment" => GLSLGenerator.Fragment,
                _ => throw new InvalidOperationException($"Invalid location \"{_location}\"")
            }, _name, _returnType, _params);
        }
    }
}