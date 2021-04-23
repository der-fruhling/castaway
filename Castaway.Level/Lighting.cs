using System.Collections.Generic;
using Castaway.Math;
using Castaway.Render;

namespace Castaway.Levels
{
    public static class Lighting
    {
        public class LightConfig
        {
            public Vector3 Ambient, Diffuse, Specular;

            public LightConfig(Vector3 ambient, Vector3 diffuse, Vector3 specular)
            {
                Ambient = ambient;
                Diffuse = diffuse;
                Specular = specular;
            }
        }
        
        public class DirectionalLightConfig : LightConfig
        {
            public Vector3 Direction;

            public DirectionalLightConfig(Vector3 ambient, Vector3 diffuse, Vector3 specular, Vector3 direction) : base(ambient, diffuse, specular)
            {
                Direction = direction;
            }
        }

        public class PointLightConfig : LightConfig
        {
            public Vector3 Position;
            public float Constant, Linear, Quadratic;

            public PointLightConfig(Vector3 ambient, Vector3 diffuse, Vector3 specular, Vector3 position, float constant, float linear, float quadratic) : base(ambient, diffuse, specular)
            {
                Position = position;
                Constant = constant;
                Linear = linear;
                Quadratic = quadratic;
            }
        }

        public class SpotlightConfig : LightConfig
        {
            public Vector3 Position, Direction;
            public float CutOff, OuterCutOff;

            public SpotlightConfig(Vector3 ambient, Vector3 diffuse, Vector3 specular, Vector3 position, Vector3 direction, float cutOff, float outerCutOff) : base(ambient, diffuse, specular)
            {
                Position = position;
                Direction = direction;
                CutOff = cutOff;
                OuterCutOff = outerCutOff;
            }
        }

        private static readonly List<DirectionalLightConfig> DirectionalLights = new List<DirectionalLightConfig>();
        private static readonly List<PointLightConfig> PointLights = new List<PointLightConfig>();
        private static readonly List<SpotlightConfig> Spotlights = new List<SpotlightConfig>();

        public static void Add(DirectionalLightConfig v) => DirectionalLights.Add(v);
        public static void Add(PointLightConfig v) => PointLights.Add(v);
        public static void Add(SpotlightConfig v) => Spotlights.Add(v);

        internal static void Reset()
        {
            DirectionalLights.Clear();
            PointLights.Clear();
            Spotlights.Clear();
        }

        internal static void Finish()
        {
            if(ShaderManager.ActiveHandle == null) return;
            var a = ShaderManager.ActiveHandle;
            a.SetProperty("Lights[D].Count", DirectionalLights.Count);
            a.SetProperty("Lights[P].Count", PointLights.Count);
            a.SetProperty("Lights[S].Count", Spotlights.Count);
            for (var i = 0; i < DirectionalLights.Count; i++)
            {
                var d = DirectionalLights[i];
                a.SetProperty(i, "Lights[D].Direction", d.Direction);
                a.SetProperty(i, "Lights[D].Ambient", d.Ambient);
                a.SetProperty(i, "Lights[D].Diffuse", d.Diffuse);
                a.SetProperty(i, "Lights[D].Specular", d.Specular);
            }
            for (var i = 0; i < PointLights.Count; i++)
            {
                var p = PointLights[i];
                a.SetProperty(i, "Lights[P].Position", p.Position);
                a.SetProperty(i, "Lights[P].Constant", p.Constant);
                a.SetProperty(i, "Lights[P].Linear", p.Linear);
                a.SetProperty(i, "Lights[P].Quadratic", p.Quadratic);
                a.SetProperty(i, "Lights[P].Ambient", p.Ambient);
                a.SetProperty(i, "Lights[P].Diffuse", p.Diffuse);
                a.SetProperty(i, "Lights[P].Specular", p.Specular);
            }
            for (var i = 0; i < Spotlights.Count; i++)
            {
                var s = Spotlights[i];
                a.SetProperty(i, "Lights[S].Direction", s.Direction);
                a.SetProperty(i, "Lights[S].Position", s.Position);
                a.SetProperty(i, "Lights[S].CutOff", s.CutOff);
                a.SetProperty(i, "Lights[S].OuterCutOff", s.OuterCutOff);
                a.SetProperty(i, "Lights[S].Ambient", s.Ambient);
                a.SetProperty(i, "Lights[S].Diffuse", s.Diffuse);
                a.SetProperty(i, "Lights[S].Specular", s.Specular);
            }
        }
    }
}