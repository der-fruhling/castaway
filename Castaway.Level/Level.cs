#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using BepuPhysics;
using BepuUtilities.Memory;
using Castaway.Assets;
using Castaway.Level.Controllers;
using Castaway.Level.Physics;
using Castaway.Math;
using Castaway.Rendering;

namespace Castaway.Level
{
    public class Level : IDisposable
    {
        private readonly SimpleThreadDispatcher _dispatcher = new(Environment.ProcessorCount);
        private readonly List<LevelObject> _objects = new();

        public uint ActiveCamera = 0;
        public Simulation PhysicsSimulation;

        public Level()
        {
            PhysicsSimulation = Simulation.Create(
                new BufferPool(),
                new NarrowPhase(),
                new PoseIntegrator(new System.Numerics.Vector3(0, -10, 0)),
                new PositionLastTimestepper());
        }

        public Level(Asset asset) : this()
        {
            var doc = asset.Type.To<XmlDocument>(asset);
            var root = doc.DocumentElement;

            var node = root!.FirstChild;
            do
            {
                if (node == null) break;

                switch (node.Name)
                {
                    case "Object":
                    {
                        _objects.Add(ParseObject((node as XmlElement)!));
                        break;
                    }
                }
            } while ((node = node!.NextSibling) != null);
        }

        public LevelObject this[string i] => Get(i);

        public void Dispose()
        {
            var buf = PhysicsSimulation.BufferPool;
            PhysicsSimulation.Dispose();
            _dispatcher.Dispose();
            buf?.Clear();
        }

        private LevelObject ParseObject(XmlElement e)
        {
            var o = new LevelObject(this);
            var conts = e["Controllers"]?.ChildNodes;
            for (var i = 0; i < (conts?.Count ?? 0); i++)
                o.Controllers.Add(ParseController((conts![i] as XmlElement)!));
            o.Name = e["Name"]?.InnerText ?? throw new InvalidOperationException("All objects need unique names.");
            if (_objects.Any(obj => obj.Name == o.Name))
                throw new InvalidOperationException("All objects need *unique* names.");
            o.Position = (Vector3) Load(typeof(Vector3), e["Position"]?.InnerText ?? "0,0,0");
            o.Scale = (Vector3) Load(typeof(Vector3), e["Scale"]?.InnerText ?? "1,1,1");
            if (e["Rotation.Quaternion"] != null)
                o.Rotation = (Quaternion) Load(typeof(Quaternion), e["Rotation.Quaternion"]?.InnerText ?? "1;0,0,0");
            else
                o.Rotation =
                    Quaternion.DegreesRotation((Vector3) Load(typeof(Vector3), e["Rotation"]?.InnerText ?? "0,0,0"));
            return o;
        }

        private Controller ParseController(XmlElement e)
        {
            var t = ControllerFinder.Get(e.Name);
            var inst = Activator.CreateInstance(t);

            foreach (var on in e.ChildNodes)
            {
                var n = on as XmlNode;
                try
                {
                    var f = t.GetFields().Single(field =>
                    {
                        var a = field.GetCustomAttribute<LevelSerializedAttribute>();
                        if (a == null) return false;
                        return a.Name == n!.Name;
                    });
                    f.SetValue(inst, Load(f.FieldType, n!.InnerText));
                }
                catch (InvalidOperationException exc)
                {
                    throw new InvalidOperationException($"Cannot find option {n!.Name} for controller {e.Name}", exc);
                }
            }

            return (inst as Controller)!;
        }

        private static object Load(Type t, string v)
        {
            if (t == typeof(int)) return int.Parse(v);
            if (t == typeof(uint)) return uint.Parse(v);
            if (t == typeof(long)) return long.Parse(v);
            if (t == typeof(ulong)) return ulong.Parse(v);
            if (t == typeof(byte)) return byte.Parse(v);
            if (t == typeof(sbyte)) return sbyte.Parse(v);
            if (t == typeof(short)) return short.Parse(v);
            if (t == typeof(ushort)) return ushort.Parse(v);
            if (t == typeof(float)) return float.Parse(v);
            if (t == typeof(double)) return double.Parse(v);
            if (t == typeof(string)) return v;
            if (t == typeof(Vector2))
            {
                var p = v.Split(',');
                return new Vector2(
                    float.Parse(p[0]),
                    float.Parse(p[1]));
            }

            if (t == typeof(Vector3))
            {
                var p = v.Split(',');
                return new Vector3(
                    float.Parse(p[0]),
                    float.Parse(p[1]),
                    float.Parse(p[2]));
            }

            if (t == typeof(Vector4))
            {
                var p = v.Split(',');
                return new Vector4(
                    float.Parse(p[0]),
                    float.Parse(p[1]),
                    float.Parse(p[2]),
                    float.Parse(p[3]));
            }

            if (t == typeof(Quaternion))
            {
                var p = Regex.Split(v, "[;,]");
                return new Quaternion(
                    float.Parse(p[0]),
                    float.Parse(p[1]),
                    float.Parse(p[2]),
                    float.Parse(p[3]));
            }

            if (t == typeof(Asset)) return AssetLoader.Loader!.GetAssetByName(v);
            if (t.IsSubclassOf(typeof(Enum))) return Enum.Parse(t, v);

            throw new InvalidOperationException($"Cannot load {t.FullName} from levels.");
        }

        public void Start()
        {
            if (!_objects.Any()) return;
            foreach (var o in _objects) o.OnInit();
        }

        public void Render()
        {
            if (!_objects.Any()) return;
            foreach (var obj in _objects)
            {
                var cams = obj.GetAll<CameraController>();
                if (!cams.Any()) continue;
                foreach (var cam in cams)
                {
                    LightResolver.Ambient(cam.AmbientLight, cam.AmbientLightColor);
                    cam.PreRenderFrame(obj, null);
                    foreach (var o in _objects) o.OnPreRender(obj);
                    foreach (var o in _objects) o.OnRender(obj);
                    foreach (var o in _objects) o.OnPostRender(obj);
                    cam.PostRenderFrame(obj, null);
                    LightResolver.Clear();
                }
            }
        }

        public void Update()
        {
            if (!_objects.Any()) return;
            PhysicsSimulation.Timestep(1f / 60f, _dispatcher);
            foreach (var o in _objects) o.OnUpdate();
        }

        public void End()
        {
            if (!_objects.Any()) return;
            foreach (var o in _objects) o.OnDestroy();
        }

        public void Add(LevelObject obj)
        {
            _objects.Add(obj);
        }

        public LevelObject Get(string name)
        {
            return _objects.Single(o => o.Name == name);
        }
    }
}