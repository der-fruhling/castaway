using System;
using System.Collections.Generic;
using System.Diagnostics;
using Castaway.Base;
using Serilog;

namespace Castaway.Assets;

public class AssetCache
{
    private static readonly ILogger Logger = CastawayGlobal.GetLogger();

    private readonly Dictionary<string, object?> _data = new();

    private string GenerateName(string id, Type objType) =>
        '{' + new StackTrace().GetFrames()[2].GetMethod()!.DeclaringType!.AssemblyQualifiedName + '}' +
        id +
        ":{" + objType.AssemblyQualifiedName + '}';

    public T Cache<T>(string id, T obj)
    {
        _data[GenerateName(id, typeof(T))] = obj;
        Logger.Debug("Cached asset {ID}, which is {$ObjectType}", id, obj);
        return obj;
    }

    public bool IsCached<T>(string id)
    {
        return _data.ContainsKey(GenerateName(id, typeof(T)));
    }

    public T Get<T>(string id)
    {
        var d = _data[GenerateName(id, typeof(T))];
        return d is T t ? t : throw new InvalidOperationException($"Somehow, {id} is an invalid type");
    }
}