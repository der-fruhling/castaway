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
	private static readonly Dictionary<string, Type> _controllers;
	private static List<Type> _controllerBases;

	static ControllerFinder()
	{
		var types = AppDomain.CurrentDomain
			.GetAssemblies()
			.Concat(new[] { Assembly.GetEntryAssembly()! })
			.Distinct()
			.SelectMany(a => a.GetTypes())
			.ToImmutableArray();
		_controllers = types
			.Where(t => t.GetCustomAttribute<ControllerNameAttribute>() != null)
			.Select(t => (t.GetCustomAttributes<ControllerNameAttribute>()
				.Select(a => a.Name ?? t.Name)
				.ToArray(), t))
			.SelectMany(t => t.Item1.Select(n => (n, t.t)))
			.ToDictionary(t => t.Item1, t => t.t);
		_controllerBases = types
			.Where(t => t.GetCustomAttribute<ControllerBaseAttribute>() != null)
			.ToList();
		Logger.Debug("Running with controller set: {Controllers}", _controllers);
	}

	public static void RegisterController(Type controller)
	{
		var attr = controller.GetCustomAttribute<ControllerNameAttribute>();
		if (attr == null)
			throw new InvalidOperationException($"All controllers need a {nameof(ControllerNameAttribute)}");
		var name = attr.Name ?? controller.Name;
		_controllers.Add(name, controller);
		Logger.Debug("Registered new controller {Controller} as {Name}", controller, name);
		Logger.Debug("Running with controller set: {Controllers}", _controllers);
	}

	public static Type Get(string name)
	{
		return _controllers.ContainsKey(name)
			? _controllers[name]
			: throw new InvalidOperationException($"{name} does not exist as a controller");
	}
}