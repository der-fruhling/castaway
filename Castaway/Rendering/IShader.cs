using System.Diagnostics.CodeAnalysis;

namespace Castaway.Rendering
{
    public enum ShaderStage
    {
        Vertex,
        Fragment
    }

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
    
    public interface IShader
    {
        bool IsValid { get; }

        bool IsVertexShader();
        bool IsFragmentShader();
    }
}