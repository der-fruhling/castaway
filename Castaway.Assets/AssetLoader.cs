#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Castaway.Assets;

public class AssetLoader
{
	public static AssetLoader? Loader;

	private readonly Dictionary<string, (string, IAssetType)> _assets;

	public AssetCache Cache = new();

	public AssetLoader()
	{
		_assets = new Dictionary<string, (string, IAssetType)>();
	}

	public Asset GetAssetByName(string name)
	{
		return new Asset(name, this, _assets[name].Item2);
	}

	internal byte[] GetBytes(Asset asset)
	{
		return File.ReadAllBytes(_assets[asset.Index].Item1);
	}

	private void Discover(string assetPath, string basePath)
	{
		var fullAssetPath = Path.GetFullPath(assetPath);
		var fullBasePath = Path.GetFullPath(basePath);

		foreach (var file in Directory.EnumerateFiles(fullAssetPath))
		{
			var name = file
				.Remove(file.IndexOf(fullBasePath, StringComparison.Ordinal), fullBasePath.Length)
				.Replace('\\', '/');
			_assets[name] = (file, GetAssetType(file));
		}

		foreach (var p in Directory.EnumerateDirectories(fullAssetPath)) Discover(p, basePath);
	}

	private static IAssetType GetAssetType(string file)
	{
		return AssetLoaderFinder.Get(file.Split('.')[^1]);
	}

	public void Discover(string assetPath)
	{
		Discover(assetPath, assetPath);
	}

	public static void Init()
	{
		Loader = new AssetLoader();
		using var json = JsonDocument.Parse(File.ReadAllText("config.json"));
		var root = json.RootElement;
		var assets = root.GetProperty("assets");
		var discover = assets.GetProperty("discover");

		for (var i = 0; i < discover.GetArrayLength(); i++)
			Loader.Discover(discover[i].GetString()
			                ?? throw new InvalidOperationException("Discovery paths must be strings."));
	}
}