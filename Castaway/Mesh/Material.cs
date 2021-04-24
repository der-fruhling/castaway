using System.Diagnostics.CodeAnalysis;
using Castaway.Math;

namespace Castaway.Mesh
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Material
    {
        public string Name;
        public Vector3 Ambient = Vector3.Zero;
        public Vector3 Diffuse = Vector3.Zero;
        public Vector3 Specular = Vector3.Zero;
        public float SpecularExponent = 0;
        public float Dissolve = 0;
        public float IndexOfRefraction = 0;
        public IllumMode Mode = IllumMode.Color;

        public enum IllumMode
        {
            Color = 0,
            ColorAmbient = 1,
            Highlight = 2,
            RayTrace = 3,
            GlassRayTrace = 4,
            ReflectionFresnel = 5,
            RefractionRayTrace = 6,
            RefractionFresnel = 7,
            Reflection = 8,
            Glass = 9,
            InvisibleCast = 10
        }
    }
}