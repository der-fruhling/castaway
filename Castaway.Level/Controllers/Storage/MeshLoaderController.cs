using System;
using System.Diagnostics.CodeAnalysis;
using Castaway.Assets;
using Castaway.Mesh;

namespace Castaway.Levels.Controllers.Storage
{
    [ControllerInfo(Name = "Mesh Loader")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class MeshLoaderController : MeshStorageController
    {
        private IMesh _loaded;
        
        public string Asset = null!;

        public override IMesh Mesh
        {
            get => _loaded;
            set => throw new ApplicationException("Cannot set value of loader");
        }

        public override void OnBegin()
        {
            base.OnBegin();
            if (Asset == null) throw new ApplicationException("Asset cannot be null.");
            
            if (Asset.EndsWith(".stl")) _loaded = AssetManager.Get<STLMesh>(AssetManager.Index(Asset));
            else throw new ApplicationException("Invalid mesh format.");
        }
    }
}