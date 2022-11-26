using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Castaway.Assets;

public static class AssetLoaderFinder
{
	private static readonly Dictionary<string, Type> Loaders;

	static AssetLoaderFinder()
	{
		Loaders = AppDomain.CurrentDomain
			.GetAssemblies()
			.Concat(Assembly.GetEntryAssembly()!.GetReferencedAssemblies()
				.Select(n => AppDomain.CurrentDomain.Load(n)))
			.Concat(new[] { Assembly.GetEntryAssembly() })
			.Distinct()
			.SelectMany(a => a!.GetTypes())
			.Where(t => t.GetInterfaces().Contains(typeof(IAssetType)) &&
			            t.GetCustomAttribute<LoadsAttribute>() != null)
			.Select(t => (t.GetCustomAttribute<LoadsAttribute>()!.Extensions, t))
			.SelectMany(t => t.Extensions.Select(e => (e, t.t)))
			.Distinct()
			.ToDictionary(t => t.e, t => t.t);
	}

	public static IAssetType Get(string extension)
	{
		return (Loaders.ContainsKey(extension)
			? Activator.CreateInstance(Loaders[extension]) as IAssetType
			: new TextAssetType()) ?? throw new InvalidOperationException($"Bad asset loader for {extension}");
	}
}