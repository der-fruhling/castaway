using System;
using Serilog;

namespace Castaway.Base;

public class EnumParseException : MessageException
{
    public string Context, Value;
    public object Default;
    public Action RepairState;
    public Type Type;

    public EnumParseException(string context, string got, Type type, Action repairState) : base(
        $"In {context}: Got {got}; needed any of {string.Join(", ", Enum.GetNames(type))}",
        false)
    {
        Context = context;
        Value = got;
        Type = type;
        RepairState = repairState;
        Default = 0;
    }

    public EnumParseException(string context, string got, Type type, Action repairState, object @default) : base(
        $"In {context}: Got {got}; needed any of {string.Join(", ", Enum.GetNames(type))}",
        false)
    {
        Context = context;
        Value = got;
        Type = type;
        Default = @default;
        RepairState = repairState;
    }

    public override void Log(ILogger logger)
    {
        logger.Error("Got invalid value {Got} in {Context}, needed any of {Values}; using {Default}",
            Value, Context, Enum.GetNames(Type), Enum.GetName(Type, Default));
    }

    public override void Repair(ILogger logger)
    {
        RepairState();
    }
}