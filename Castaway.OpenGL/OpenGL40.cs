using System;
using Castaway.Math;
using Castaway.OpenGL.Native;
using Castaway.Rendering;

namespace Castaway.OpenGL
{
    [Implements("OpenGL-4.0")]
    public class OpenGL40 : OpenGL33
    {
        public override void SetDoubleUniform(ShaderObject p, string name, double i)
        {
            if (p is not Shader s)
                throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
            GL.SetUniform(GL.GetUniformLocation(s.Number, name), 1, new []{i});
        }

        public override void SetDoubleUniform(ShaderObject p, string name, double x, double y)
        {
            if (p is not Shader s)
                throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
            GL.SetUniformVector2(GL.GetUniformLocation(s.Number, name), 1, new []{x, y});
        }

        public override void SetDoubleUniform(ShaderObject p, string name, double x, double y, double z)
        {
            if (p is not Shader s)
                throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
            GL.SetUniformVector3(GL.GetUniformLocation(s.Number, name), 1, new []{x, y, z});
        }

        public override void SetDoubleUniform(ShaderObject p, string name, double x, double y, double z, double w)
        {
            if (p is not Shader s)
                throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
            GL.SetUniformVector3(GL.GetUniformLocation(s.Number, name), 1, new []{x, y, z, w});
        }

        public override void SetDoubleUniform(ShaderObject p, string name, Matrix2 m)
        {
            if (p is not Shader s)
                throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
            GL.SetUniformMatrix2(GL.GetUniformLocation(s.Number, name), 1, false, m.Array);
        }

        public override void SetDoubleUniform(ShaderObject p, string name, Matrix3 m)
        {
            if (p is not Shader s)
                throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
            GL.SetUniformMatrix3(GL.GetUniformLocation(s.Number, name), 1, false, m.Array);
        }

        public override void SetDoubleUniform(ShaderObject p, string name, Matrix4 m)
        {
            if (p is not Shader s)
                throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
            GL.SetUniformMatrix4(GL.GetUniformLocation(s.Number, name), 1, false, m.Array);
        }
    }
}