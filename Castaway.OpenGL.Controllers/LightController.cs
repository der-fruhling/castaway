using System;
using Castaway.Base;
using Castaway.Level;
using Castaway.Math;
using Castaway.Rendering;
using Castaway.Rendering.Lighting;

namespace Castaway.OpenGL.Controllers;

[ControllerName("Light")]
[Imports(typeof(OpenGLImpl))]
public class LightController : Controller
{
    [LevelSerialized("Color")] public Vector3 Color = new(1, 1, 1);
    [LevelSerialized("Type")] public LightType Type;

    public override void PreRenderFrame(LevelObject camera, LevelObject? parent)
    {
        base.PreRenderFrame(camera, parent);
        switch (Type)
        {
            case LightType.Point:
                LightResolver.Add(new PointLight(parent!.Position, Color));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(Type), Type, "Invalid light type.");
        }
    }
}