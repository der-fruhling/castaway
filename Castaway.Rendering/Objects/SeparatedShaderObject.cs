using Castaway.Rendering.Shaders;

namespace Castaway.Rendering.Objects;

public abstract class SeparatedShaderObject : RenderObject, IValidatable
{
	public readonly string SourceCode;
	public readonly string SourceLocation;
	public readonly ShaderStage Stage;

	protected SeparatedShaderObject(ShaderStage stage, string sourceCode, string sourceLocation)
	{
		Stage = stage;
		SourceCode = sourceCode;
		SourceLocation = sourceLocation;
	}

	public abstract bool Valid { get; }
}