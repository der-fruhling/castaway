using System.Collections.Generic;
using System.Linq;

namespace Castaway.Render
{
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