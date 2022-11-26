using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castaway.Base;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Serilog;

namespace Castaway.Rendering;

public class ImplFinder
{
	private static Dictionary<string, Type>? _implementations;
	private static readonly ILogger Logger = CastawayGlobal.GetLogger();

	public static async Task<Graphics?> Find(string name)
	{
		_implementations ??= await FindImplementations();
		if (!_implementations.ContainsKey(name)) return null;
		var obj = Activator.CreateInstance(_implementations[name]);
		_implementations[name]
			.GetConstructor(Array.Empty<Type>())?
			.Invoke(obj, null);
		return obj as Graphics;
	}

	private static async Task<Dictionary<string, Type>> FindImplementations()
	{
		return await Task.Run(() =>
		{
			Logger.Debug("Searching for implementations");
			var types = AppDomain.CurrentDomain
				.GetAssemblies()
				.Concat(Assembly.GetEntryAssembly()!.GetReferencedAssemblies()
					.Select(n => AppDomain.CurrentDomain.Load(n)))
				.Concat(new[] { Assembly.GetEntryAssembly() })
				.Distinct()
				.SelectMany(a => a!.GetTypes())
				.Distinct()
				.Where(t => t.GetCustomAttribute<ImplementsAttribute>() != null);
			var implList = types as Type[] ?? types.ToArray();
			Logger.Debug("Found {Count} implementations", implList.Length);
			foreach (var i in implList)
				Logger.Verbose("Implementation {Name} {Type}", i.GetCustomAttribute<ImplementsAttribute>()!.Name,
					i);
			var impls = new Dictionary<string, Type>();
			foreach (var type in implList)
			{
				var impl = type.GetCustomAttribute<ImplementsAttribute>();
				impls.Add(impl!.Name, type);
			}

			return impls;
		});
	}

	public static async Task<Graphics?> FindOptimalImplementation(Window window)
	{
		int major, minor;

		unsafe
		{
			major = GLFW.GetWindowAttrib(window.Native, WindowAttributeGetInt.ContextVersionMajor);
			minor = GLFW.GetWindowAttrib(window.Native, WindowAttributeGetInt.ContextVersionMinor);
		}

		if (Supports(major, minor, 4, 2)) return await Find("OpenGL-4.2");
		if (Supports(major, minor, 4, 1)) return await Find("OpenGL-4.1");
		if (Supports(major, minor, 4, 0)) return await Find("OpenGL-4.0");
		if (Supports(major, minor, 3, 3)) return await Find("OpenGL-3.3");
		if (Supports(major, minor, 3, 2)) return await Find("OpenGL-3.2");

		throw new GraphicsException($"Minimum OpenGL requirement is 3.2, found {major}.{minor}");
	}

	private static bool Supports(int maj1, int min1, int maj2, int min2)
	{
		return (min1 >= min2 && maj1 == maj2) || maj1 > maj2;
	}
}