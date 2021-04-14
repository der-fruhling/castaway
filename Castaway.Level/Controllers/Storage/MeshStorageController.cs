using Castaway.Mesh;

namespace Castaway.Levels.Controllers.Storage
{
    [ControllerInfo(Name = "Mesh")]
    public class MeshStorageController : Controller
    {
        public virtual IMesh Mesh { get; set; }
    }
}