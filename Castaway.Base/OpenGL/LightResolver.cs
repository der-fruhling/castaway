using System.Collections.Generic;
using Castaway.Math;
using Castaway.Rendering;

namespace Castaway.OpenGL
{
    public enum LightType
    {
        Point,
    }
    
    public abstract class Light {}
    
    public class PointLight : Light
    {
        public Vector3 Position;
        public Vector3 Color;

        public PointLight(Vector3 position, Vector3 color)
        {
            Position = position;
            Color = color;
        }
    }
    
    public static class LightResolver
    {
        private static readonly List<PointLight> PointLights = new();
        private static float _ambientLight = .1f;
        private static Vector3 _ambientLightColor = new(1, 1, 1);
        
        public static void Add(PointLight light)
        {
            PointLights.Add(light);
        }

        public static void Ambient(float ambient, Vector3 color)
        {
            _ambientLight = ambient;
            _ambientLightColor = color;
        }

        public static void Push()
        {
            var g = OpenGL.Get();
            var p = g.BoundProgram!.Value;
            g.SetUniform(p, UniformType.AmbientLight, _ambientLight);
            g.SetUniform(p, UniformType.AmbientLightColor, _ambientLightColor);
            g.SetUniform(p, UniformType.PointLightCount, PointLights.Count);
            for (var i = 0; i < PointLights.Count; i++)
            {
                var l = PointLights[i];
                g.SetUniform(p, i, UniformType.PointLightPositionIndexed, l.Position);
                g.SetUniform(p, i, UniformType.PointLightColorIndexed, l.Color);
            }
        }

        public static void Clear()
        {
            _ambientLight = .1f;
            PointLights.Clear();
        }
    }
}