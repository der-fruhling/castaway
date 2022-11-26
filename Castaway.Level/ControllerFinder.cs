using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Castaway.Base;
using Serilog;

namespace Castaway.Level;

public static class ControllerFinder
{
	private static readonly ILogger Logger = CastawayGlobal.GetLogger();
	private static readonly Dictionary<string, Type> Controllers;

	static ControllerFinder()
	{
		var types = AppDomain.CurrentDomain
			.GetAssemblies()
			.Concat(new[] { Assembly.GetEntryAssembly()! })
			.Distinct()
			.SelectMany(a => a.GetTypes())
			.ToImmutableArray();
		Controllers = types
			.Where(t => t.GetCustomAttribute<ControllerNameAttribute>() != null)
			.Select(t => (t.GetCustomAttributes<ControllerNameAttribute>()
				.Select(a => a.Name ?? t.Name)
				.ToArray(), t))
			.SelectMany(t => t.Item1.Select(n => (n, t.t)))
			.ToDictionary(t => t.Item1, t => t.t);
		Logger.Debug("Running with controller set: {Controllers}", Controllers);
	}

	public static void RegisterController(Type controller)
	{
		var attr = controller.GetCustomAttribute<ControllerNameAttribute>();
		if (attr == null)
			throw new InvalidOperationException($"All controllers need a {nameof(ControllerNameAttribute)}");
		var name = attr.Name ?? controller.Name;
		Controllers.Add(name, controller);
		Logger.Debug("Registered new controller {Controller} as {Name}", controller, name);
		Logger.Debug("Running with controller set: {Controllers}", Controllers);
	}

	public static Type Get(string name)
	{
		return Controllers.ContainsKey(name)
			? Controllers[name]
			: throw new InvalidOperationException($"{name} does not exist as a controller");
	}
}