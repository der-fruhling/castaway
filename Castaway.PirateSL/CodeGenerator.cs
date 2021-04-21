#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Castaway.PirateSL
{
    public struct FuncParam
    {
        public PSLType Type;
        public string Name;
    }

    public abstract class CodeGenerator
    {
        protected uint Context;
        
        public abstract string[] Data { get; }
        
        public abstract CodeGenerator GenFunctionStart(uint where, string name, PSLType returnType, params FuncParam[] funcParams);
        public abstract CodeGenerator GenEntrypointStart(uint where);
        public abstract CodeGenerator GenIfBlock(uint where, PSLValue condition);
        public abstract CodeGenerator GenElseBlock(uint where);
        public abstract CodeGenerator GenElseIfBlock(uint where, PSLValue condition);
        public abstract CodeGenerator GenForBlock(uint where, string varName, PSLType varType, PSLValue initialValue, PSLValue condition, PSLValue increment);
        public abstract CodeGenerator GenWhileBlock(uint where, PSLValue condition);
        public abstract CodeGenerator GenBlockEnd(uint where);
        public abstract CodeGenerator GenVariableSet(uint where, string name, PSLValue value);
        
        public virtual CodeGenerator GenIfBlock(PSLValue condition) => GenIfBlock(Context, condition);
        public virtual CodeGenerator GenElseBlock() => GenElseBlock(Context);
        public virtual CodeGenerator GenElseIfBlock(PSLValue condition) => GenElseIfBlock(Context, condition);
        public virtual CodeGenerator GenForBlock(string varName, PSLType varType, PSLValue initialValue, PSLValue condition, PSLValue increment) =>
            GenForBlock(Context, varName, varType, initialValue, condition, increment);
        public virtual CodeGenerator GenWhileBlock(PSLValue condition) => GenWhileBlock(Context, condition);
        public virtual CodeGenerator GenBlockEnd() => GenBlockEnd(Context);
        public virtual CodeGenerator GenVariableSet(string name, PSLValue value) =>
            GenVariableSet(Context, name, value);

        public abstract CodeGenerator GenInput(uint where, PSLType type, string name);
        public abstract CodeGenerator GenVertInput(PSLType type, string name, string target);
        public abstract CodeGenerator GenOutput(uint where, PSLType type, string name);
        public abstract CodeGenerator GenFragOutput(PSLType type, string name, int target);
        public abstract CodeGenerator GenUniform(uint where, PSLType type, string name, PSLValue? initialValue = null);
        public abstract CodeGenerator GenUniform(PSLType type, string name, string target);
        public abstract CodeGenerator GenVariable(uint where, PSLType type, string name, PSLValue? initialValue = null);
        public abstract CodeGenerator GenVariable(uint where, string name, PSLValue value);
        public abstract CodeGenerator GenReturn(uint where, PSLValue value);
        public abstract CodeGenerator GenSkip(uint where);
        
        public virtual CodeGenerator GenVariable(PSLType type, string name, PSLValue? initialValue = null) =>
            GenVariable(Context, type, name, initialValue);
        public virtual CodeGenerator GenVariable(string name, PSLValue value) => GenVariable(Context, name, value);
        public virtual CodeGenerator GenReturn(PSLValue value) => GenReturn(Context, value);
        public virtual CodeGenerator GenSkip() => GenSkip(Context);
        
        public abstract CodeGenerator GenComment(uint where, string text);
        public abstract CodeGenerator GenConfiguration(string confStr);

        public abstract void WriteOut();
        public abstract void WriteOut(Dictionary<string, string> files);
        public abstract CodeGenerator GenUniform(PSLType type, string name, PSLValue? initialValue = null);
    }
}