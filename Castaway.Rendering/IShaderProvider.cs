using System.Diagnostics.CodeAnalysis;
using Castaway.Rendering.Objects;

namespace Castaway.Rendering;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public interface IShaderProvider
{
    ShaderObject CreateDefault(Graphics g);
    ShaderObject CreateDefaultTextured(Graphics g);
    ShaderObject CreateDirect(Graphics g);
    ShaderObject CreateDirectTextured(Graphics g);
    ShaderObject CreateUIScaled(Graphics g);
    ShaderObject CreateUIScaledTextured(Graphics g);
    ShaderObject CreateUIUnscaled(Graphics g);
    ShaderObject CreateUIUnscaledTextured(Graphics g);
}