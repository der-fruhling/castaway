using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Castaway.Render
{
    /// <summary>
    /// Type of shaders loaded by <see cref="GLSLShaderAssetLoader"/>. Call
    /// <see cref="ToHandle"/> to create a usable shader.
    /// </summary>
    public class LoadedShader
    {
        private readonly Dictionary<string, VertexAttribInfo.AttribValue> _vertAttrs;
        private readonly Dictionary<string, uint> _fragOutputs;
        private readonly string _vertSrc;
        private readonly string _fragSrc;

        public LoadedShader(Dictionary<string, VertexAttribInfo.AttribValue> vertAttrs, Dictionary<string, uint> fragOutputs, string vertSrc, string fragSrc)
        {
            _vertAttrs = vertAttrs;
            _fragOutputs = fragOutputs;
            _vertSrc = vertSrc;
            _fragSrc = fragSrc;
        }

        /// <summary>
        /// Creates a new <see cref="ShaderHandle"/> from the data contained
        /// in this instance.
        ///
        /// Should only be called once, saving the result to a variable for
        /// later use. <b>Each call to this function creates a new shader.</b>
        /// </summary>
        /// <returns>New <see cref="ShaderHandle"/>, with <see cref="_vertSrc"/>,
        /// and <see cref="_fragSrc"/> as the sources.</returns>
        public ShaderHandle ToHandle()
        {
            var attrList = _vertAttrs.Select(a => new VertexAttribInfo(a.Value, a.Key)).ToArray();
            var program = ShaderManager.CreateShader(_vertSrc, _fragSrc, attrList);
            foreach (var (key, value) in _fragOutputs) program.BindFragmentLocation(value, key);
            program.Finish();

            return program;
        }
    }
}