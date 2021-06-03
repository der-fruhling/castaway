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
        TextureU,
        TextureUV,
        TextureUVT
    }

    public enum UniformType
    {
        Custom,
        TransformPerspective,
        TransformView,
        TransformModel
    }

    public enum ShaderStage
    {
        Vertex,
        Fragment
    }
}