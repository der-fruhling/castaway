namespace Castaway.OpenGL
{
    public static partial class GL
    {
        public enum BufferTarget
        {
            [ConstValue(0x8892)] ArrayBuffer,
            [ConstValue(0x92C0)] AtomicCounterBuffer,
            [ConstValue(0x8F36)] CopyReadBuffer,
            [ConstValue(0x8F37)] CopyWriteBuffer,
            [ConstValue(0x90EE)] DispatchIndirectBuffer,
            [ConstValue(0x8F3F)] DrawIndirectBuffer,
            [ConstValue(0x8893)] ElementArrayBuffer,
            [ConstValue(0x88EB)] PixelPackBuffer,
            [ConstValue(0x88EC)] PixelUnpackBuffer,
            [ConstValue(0x9192)] QueryBuffer,
            [ConstValue(0x90D2)] ShaderStorageBuffer,
            [ConstValue(0x8C2A)] TextureBuffer,
            [ConstValue(0x8C8E)] TransformFeedbackBuffer,
            [ConstValue(0x8A11)] UniformBuffer
        }

        public enum Error
        {
            [ConstValue(0x0)] None,
            [ConstValue(0x500)] InvalidEnum,
            [ConstValue(0x501)] InvalidValue,
            [ConstValue(0x502)] InvalidOperation,
            [ConstValue(0x503)] StackOverflow,
            [ConstValue(0x504)] StackUnderflow,
            [ConstValue(0x505)] OutOfMemory
        }

        public enum ShaderStage
        {
            [ConstValue(0x91B9)] ComputeShader,
            [ConstValue(0x8b31)] VertexShader,
            [ConstValue(0x8e88)] TessControlShader,
            [ConstValue(0x8e87)] TessEvaluationShader,
            [ConstValue(0x8dd9)] GeometryShader,
            [ConstValue(0x8b30)] FragmentShader
        }

        public enum ShaderQuery
        {
            [ConstValue(0x8b4f)] ShaderType,
            [ConstValue(0x8b80)] DeleteStatus,
            [ConstValue(0x8b81)] CompileStatus,
            [ConstValue(0x8b84)] InfoLogLength,
            [ConstValue(0x8b88)] SourceLength
        }

        public enum ProgramQuery
        {
            [ConstValue(0x8b80)] DeleteStatus,
            [ConstValue(0x8b82)] LinkStatus,
            [ConstValue(0x8b83)] ValidateStatus,
            [ConstValue(0x8b84)] InfoLogLength,
            [ConstValue(0x8b85)] AttachedShaders,
            [ConstValue(0x92d9)] ActiveAtomicCounterBuffers,
            [ConstValue(0x8b89)] ActiveAttributes,
            [ConstValue(0x8b8a)] ActiveAttributeMaxLength,
            [ConstValue(0x8b86)] ActiveUniforms,
            [ConstValue(0x8a36)] ActiveUniformBlocks,
            [ConstValue(0x8a35)] ActiveUniformBlockMaxNameLength,
            [ConstValue(0x8b87)] ActiveUniformMaxLength,
            [ConstValue(0x8267)] ComputeWorkGroupSize,
            [ConstValue(0x8741)] BinaryLength,
            [ConstValue(0x8c7f)] TransformFeedbackBufferMode,
            [ConstValue(0x8c83)] TransformFeedbackVaryings,
            [ConstValue(0x8c76)] TransformFeedbackVaryingMaxLength,
            [ConstValue(0x8916)] GeometryVerticesOut,
            [ConstValue(0x8917)] GeometryInputType,
            [ConstValue(0x8918)] GeometryOutputType
        }
    }
}