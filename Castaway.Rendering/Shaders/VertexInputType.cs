using System;
using System.Diagnostics.CodeAnalysis;

namespace Castaway.Rendering.Shaders;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum VertexInputType
{
    PositionXY,
    PositionXYZ,
    ColorG,
    ColorRGB,
    ColorRGBA,

    [Obsolete("Use " + nameof(ColorRGBA) + " instead")]
    ColorBGRA,
    NormalXY,
    NormalXYZ,
    TextureS,
    TextureST,
    TextureSTV
}