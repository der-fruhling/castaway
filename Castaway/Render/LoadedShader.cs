using System.Collections.Generic;
using System.Linq;
using Castaway.Math;
using Castaway.Native;

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
        private readonly string _model;
        private readonly string _view;
        private readonly string _projection;
        private readonly Dictionary<string, string> _properties;

        public LoadedShader(
            Dictionary<string, VertexAttribInfo.AttribValue> vertAttrs, 
            Dictionary<string, uint> fragOutputs, 
            string vertSrc, 
            string fragSrc,
            string model,
            string view,
            string projection,
            Dictionary<string, string> properties)
        {
            _vertAttrs = vertAttrs;
            _fragOutputs = fragOutputs;
            _vertSrc = vertSrc;
            _fragSrc = fragSrc;
            _model = model;
            _view = view;
            _projection = projection;
            _properties = properties;
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

            if (_model.Length > 0)
                program.TModel = GL.GetUniformLocation(program.GLProgram, _model);
            if (_view.Length > 0)
                program.TView = GL.GetUniformLocation(program.GLProgram, _view);
            if (_projection.Length > 0)
                program.TProjection = GL.GetUniformLocation(program.GLProgram, _projection);
            
            program.Use();
            program.SetTModel(Matrix4.Identity);
            program.SetTView(Matrix4.Identity);
            program.SetTProjection(Matrix4.Identity);
            foreach (var (k, v) in _properties)
            {
                program.Properties[k] = v;
            }

            return program;
        }
    }
}