#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using static Castaway.PirateSL.PSLCastConfig.PSLCastConfigType;
using static Castaway.PirateSL.PSLType;

namespace Castaway.PirateSL
{
    // ReSharper disable once InconsistentNaming
    public class GLSLGenerator : CodeGenerator
    {
        private const string DefaultData = "// PirateSL Generated File.\n" +
                                           "// Intended for use with Castaway.\n\n";

        public const uint Vertex = 0, Fragment = 1, Config = 2;
        
        public override string[] Data { get; } = {DefaultData, DefaultData, DefaultData};

        private List<Action> _oneTime = new List<Action>();
        private int _indent;

        private string Indent
        {
            get
            {
                if (_indent <= 0) return "";
                var s = "";
                for (var i = 0; i < _indent; i++) s += "    ";
                return s;
            }
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public Dictionary<string, string> GLSLObjects = new Dictionary<string, string>();

        private void RunOneTime()
        {
            _oneTime.ForEach(a => a());
            _oneTime.Clear();
        }
        
        public override CodeGenerator GenFunctionStart(uint where, string name, PSLType returnType, params FuncParam[] p)
        {
            Data[where] += $"\n{Indent}{returnType.ToGLSLName()} {name}(";
            var pl = p.ToList();
            while (pl.Count > 0)
            {
                var param = pl[0];
                Data[where] += $"{param.Type.ToGLSLName()} {param.Name}";
                pl.RemoveAt(0);
                if (pl.Count > 0) Data[where] += ", ";
            }
            Data[where] += $")\n{Indent}{{\n";
            _indent++;
            Context = where;
            return this;
        }

        public override CodeGenerator GenEntrypointStart(uint where)
        {
            Data[where] += $"\n{Indent}/* entrypoint */ void main()\n{Indent}{{\n";
            _indent++;
            Context = where;
            return this;
        }

        public override CodeGenerator GenIfBlock(uint where, PSLValue condition)
        {
            Data[where] += $"{Indent}if({condition.GLSL})\n{Indent}{{\n";
            _indent++;
            return this;
        }

        public override CodeGenerator GenElseBlock(uint where)
        {
            _indent--;
            Data[where] += $"{Indent}}}\n{Indent}else\n{Indent}{{\n";
            _indent++;
            return this;
        }

        public override CodeGenerator GenElseIfBlock(uint where, PSLValue condition)
        {
            _indent--;
            Data[where] += $"{Indent}}}\n{Indent}else if({condition.GLSL})\n{Indent}{{\n";
            _indent++;
            return this;
        }

        public override CodeGenerator GenForBlock(uint where, string varName, PSLType varType, PSLValue initialValue, PSLValue condition,
            PSLValue increment)
        {
            Data[where] +=
                $"{Indent}for({varType.ToGLSLName()} {varName} = {initialValue.GLSL}; {condition.GLSL}; {increment.GLSL})\n{{";
            _indent++;
            return this;
        }

        public override CodeGenerator GenWhileBlock(uint where, PSLValue condition)
        {
            Data[where] += $"{Indent}while({condition.GLSL})\n{Indent}{{\n";
            _indent++;
            return this;
        }

        public override CodeGenerator GenBlockEnd(uint where)
        {
            _indent--;
            Data[where] += $"{Indent}}}\n";
            if (_indent <= 0) Context = uint.MaxValue;
            return this;
        }

        public override CodeGenerator GenInput(uint where, PSLType type, string name)
        {
            Data[where] += $"{Indent}/* input {type.ToName()} */ in {type.ToGLSLName()} {name};\n";
            return this;
        }

        public override CodeGenerator GenVertInput(PSLType type, string name, string target)
        {
            Data[Vertex] += $"{Indent}/* vertex input {type.ToName()} */ in {type.ToGLSLName()} {name}; // from {target}\n";
            Data[Config] += $"// vertex input {type.ToName()}\ninput {name} = {target}\n\n";
            return this;
        }

        public override CodeGenerator GenOutput(uint where, PSLType type, string name)
        {
            Data[where] += $"{Indent}/* output {type.ToName()} */ out {type.ToGLSLName()} {name};\n";
            return this;
        }

        public override CodeGenerator GenFragOutput(PSLType type, string name, int target)
        {
            Data[Fragment] += $"{Indent}/* fragment output {type.ToName()} */ out {type.ToGLSLName()} {name}; // to {target}\n";
            Data[Config] += $"// fragment output {type.ToName()}\noutput {name} = {target}\n\n";
            return this;
        }

        public override CodeGenerator GenUniform(uint where, PSLType type, string name, PSLValue? initialValue = null)
        {
            if (initialValue == null) Data[where] += $"{Indent}/* uniform {type.ToName()} */ uniform {type.ToGLSLName()} {name};\n";
            else Data[where] += $"{Indent}uniform {type.ToGLSLName()} {name} = {initialValue.GLSL};\n";
            return this;
        }

        public override void WriteOut(Dictionary<string, string> files)
        {
            files["shader.vsh"] = Data[Vertex];
            files["shader.fsh"] = Data[Fragment];
            files["shader.csh"] = Data[Config];
        }

        public override CodeGenerator GenUniform(PSLType type, string name, PSLValue? initialValue = null)
        {
            GenUniform(Vertex, type, name, initialValue);
            GenUniform(Fragment, type, name, initialValue);
            return this;
        }

        public override CodeGenerator GenUniform(PSLType type, string name, string target)
        {
            Data[Vertex] += $"{Indent}/* uniform {type.ToName()} */ uniform {type.ToGLSLName()} {name}; // as {target}\n";
            Data[Fragment] += $"{Indent}/* uniform {type.ToName()} */ uniform {type.ToGLSLName()} {name}; // as {target}\n";
            Data[Config] += $"// uniform {type.ToName()}\nuse {name} as {target}\n\n";
            return this;
        }

        public override CodeGenerator GenVariable(uint where, PSLType type, string name, PSLValue? initialValue = null)
        {
            if (initialValue == null) Data[where] += $"{Indent}{type.ToGLSLName()} {name};\n";
            else Data[where] += $"{Indent}{type.ToGLSLName()} {name} = {initialValue.GLSL};\n";
            return this;
        }

        public override CodeGenerator GenVariable(uint where, string name, PSLValue value)
        {
            Data[where] += $"{Indent}/* var */ {value.Type.ToGLSLName()} {name} = {value.GLSL};\n";
            return this;
        }

        public override CodeGenerator GenReturn(uint where, PSLValue value)
        {
            if (value.Type == p_nul) Data[where] += $"{Indent}return;\n";
            else Data[where] += $"{Indent}return {value.GLSL};\n";
            return this;
        }

        public override CodeGenerator GenSkip(uint @where)
        {
            Data[where] += $"{Indent}discard;\n";
            return this;
        }

        public override CodeGenerator GenVariableSet(uint where, string name, PSLValue value)
        {
            if(GLSLObjects.ContainsKey(name)) Data[where] += $"{Indent}{GLSLObjects[name]} /* {name} */ = {value.GLSL};\n";
            else Data[where] += $"{Indent}{name} = {value.GLSL};\n";
            return this;
        }

        public override CodeGenerator GenComment(uint where, string text)
        {
            Data[where] += $"{Indent}// {text}\n";
            return this;
        }

        public override CodeGenerator GenConfiguration(string confStr)
        {
            Data[Vertex] += $"#version {confStr}\n\n";
            Data[Fragment] += $"#version {confStr}\n\n";
            return this;
        }

        public override void WriteOut()
        {
            File.WriteAllText("shader.vsh", Data[Vertex]);
            File.WriteAllText("shader.fsh", Data[Fragment]);
            File.WriteAllText("shader.csh", Data[Config]);
        }
    }
}
