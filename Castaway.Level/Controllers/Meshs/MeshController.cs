using Castaway.Assets;
using Castaway.Base;
using Castaway.Rendering.Structures;
using Serilog;

namespace Castaway.Level.Controllers;

[ControllerName("LoadedMesh")]
public class MeshController : Controller
{
	private static readonly ILogger Logger = CastawayGlobal.GetLogger();

	public Mesh? Mesh;

	[LevelSerialized("AssetPath")] public Asset? Asset { get; set; }
	[LevelSerialized("DisableCache")] public bool CacheDisabled { get; set; } = false;

	public override void OnInit(LevelObject parent)
	{
		base.OnInit(parent);
		if (CacheDisabled) Mesh = ResolveNormally();
		else
			Mesh = AssetLoader.Loader!.Cache.IsCached<Mesh>(Asset!.Index)
				? ResolveWithCache()
				: ResolveNormallyAndCache();
	}

	private Mesh ResolveNormally()
	{
		return Asset!.Type.Read<Mesh>(Asset);
	}

	private Mesh ResolveNormallyAndCache()
	{
		Logger.Debug("Caching {Asset} as {Type}", Asset!.Index, typeof(Mesh));
		return AssetLoader.Loader!.Cache.Cache(Asset!.Index, ResolveNormally());
	}

	private Mesh ResolveWithCache()
	{
		Logger.Debug("Resolving {Asset} from cache as {Type}", Asset!.Index, typeof(Mesh));
		return AssetLoader.Loader!.Cache.Get<Mesh>(Asset!.Index);
	}
}