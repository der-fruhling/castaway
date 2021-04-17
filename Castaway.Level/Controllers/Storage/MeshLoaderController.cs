using System;
using System.Diagnostics.CodeAnalysis;
using Castaway.Assets;
using Castaway.Math;
using Castaway.Mesh;

namespace Castaway.Levels.Controllers.Storage
{
    [ControllerInfo(Name = "Mesh Loader")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class MeshLoaderController : MeshStorageController
    {
        private IMesh _loaded;
        
        public string Asset;
        public bool ZUp = false;

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
            else if (Asset.EndsWith(".obj")) _loaded = AssetManager.Get<OBJMesh>(AssetManager.Index(Asset));
            else throw new ApplicationException("Invalid mesh format.");

            if (!ZUp) return;
            for (var i = 0; i < _loaded!.Vertices.Length; i++)
            {
                var v = _loaded.Vertices[i];
                _loaded.Vertices[i] = new CompleteVertex
                {
                    Norm = new Vector3(v.Norm.X, v.Norm.Z, v.Norm.Y),
                    Pos = new Vector3(v.Pos.X, v.Pos.Z, v.Pos.Y),
                    Tex = v.Tex
                };
            }
        }
    }
}