using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Castaway.Math;

namespace Castaway.Rendering
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

        private static ShaderObject? _pushedShader;
        private static ImmutableArray<PointLight> _pushedPointLights;
        private static float _pushedAmbientLight;
        private static Vector3 _pushedAmbientLightColor;
        
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
            var g = Graphics.Current;
            var p = g.BoundShader!;
            
            if (_pushedShader == p &&
                PointLights.SequenceEqual(_pushedPointLights) &&
                System.Math.Abs(_pushedAmbientLight - _ambientLight) < 0.00025f &&
                _pushedAmbientLightColor == _ambientLightColor) return;
            
            g.SetFloatUniform(p, UniformType.AmbientLight, _ambientLight);
            g.SetFloatUniform(p, UniformType.AmbientLightColor, _ambientLightColor);
            g.SetIntUniform(p, UniformType.PointLightCount, PointLights.Count);
            for (var i = 0; i < PointLights.Count; i++)
            {
                var l = PointLights[i];
                g.SetUniform(p, i, UniformType.PointLightPositionIndexed, l.Position);
                g.SetUniform(p, i, UniformType.PointLightColorIndexed, l.Color);
            }

            _pushedShader = p;
            _pushedPointLights = PointLights.ToImmutableArray();
            _pushedAmbientLight = _ambientLight;
            _pushedAmbientLightColor = _ambientLightColor;
        }

        public static void Clear()
        {
            _ambientLight = .1f;
            PointLights.Clear();
        }
    }
}