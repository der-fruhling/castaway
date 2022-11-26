using System.Diagnostics.CodeAnalysis;

namespace Castaway.Rendering.Shaders;

[SuppressMessage("ReSharper", "InconsistentNaming")]
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
	ViewPosition,
	FramebufferSize,
	UIScale
}