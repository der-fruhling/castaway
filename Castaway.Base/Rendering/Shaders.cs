using System.Diagnostics.CodeAnalysis;

namespace Castaway.Rendering
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum VertexInputType
    {
        PositionXY,
        PositionXYZ,
        ColorG,
        ColorRGB,
        ColorRGBA,
        ColorBGRA,
        NormalXY,
        NormalXYZ,
        TextureS,
        TextureST,
        TextureSTV
    }

    public enum UniformType
    {
        Custom,
        TransformPerspective,
        TransformView,
        TransformModel,
        PointLightCount,
        PointLightPositionIndexed,
        PointLightColorIndexed,
        AmbientLight,
        AmbientLightColor,
        ViewPosition
    }

    public enum ShaderStage
    {
        Vertex,
        Fragment
    }
}