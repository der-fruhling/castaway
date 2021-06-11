using System;
using Castaway.Math;
using Castaway.OpenGL;

namespace Castaway.Level.OpenGL
{
    public class LightController : EmptyController
    {
        [LevelSerialized("Type")] public LightType Type;
        [LevelSerialized("Color")] public Vector3 Color = new(1, 1, 1);
        
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
}