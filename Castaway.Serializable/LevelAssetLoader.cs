using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Castaway.Assets;
using Castaway.Core;
using Castaway.Levels;
using Castaway.Levels.Controllers.Storage;
using Castaway.Math;

namespace Castaway.Serializable
{
    public class LevelAssetLoader : IAssetLoader
    {
        private static readonly IEnumerable<Type> _types = 
            AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                .Concat(CastawayCore.StartupAssembly.GetTypes());
        
        public IEnumerable<string> FileExtensions { get; } = new[] {"lvl"};
        
        public object LoadFile(string path)
        {
            var level = new Level();
            var lines = File.ReadAllLines(path).Select(s => s.Trim());
            ReadLevel(ref level, lines.GetEnumerator());
            return level;
        }

        private static void ReadLevel(ref Level level, IEnumerator<string> lines)
        {
            while (lines.MoveNext())
            {
                var line = lines.Current;
                if(line!.StartsWith('#') || line.Length == 0) continue;
                var parts = line.Split(' ');
                switch (parts[0])
                {
                    case "InitialCamera" when parts.Length == 2:
                        level.CurrentCamera = uint.Parse(parts[1]);
                        break;

                    case "Object" when parts.Length == 1:
                    {
                        var @ref = level.Create();
                        var o = @ref.Object;
                        ReadObject(ref o, lines);
                        @ref.Object = o;
                        break;
                    }
                    
                    default:
                        throw new ApplicationException($"Invalid line in level file: `{line}`");
                }
            }
        }

        private static void ReadObject(ref LevelObject obj, IEnumerator<string> lines)
        {
            while (lines.MoveNext())
            {
                var line = lines.Current;
                if(line!.StartsWith('#') || line.Length == 0) continue;
                var parts = line.Split(' ');
                switch (parts[0])
                {
                    case "Position" when parts.Length == 3:
                        obj.Position = new Vector3(float.Parse(parts[1]), float.Parse(parts[2]), 0);
                        break;
                    case "Position" when parts.Length == 4:
                        obj.Position = new Vector3(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
                        break;
                    case "Rotation" when parts.Length == 1:
                        obj.Rotation = new Vector3(0, 0, float.Parse(parts[1]));
                        break;
                    case "Rotation" when parts.Length == 4:
                        obj.Rotation = new Vector3(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
                        break;
                    case "Scale" when parts.Length == 3:
                        obj.Scale = new Vector3(float.Parse(parts[1]), float.Parse(parts[2]), 0);
                        break;
                    case "Scale" when parts.Length == 4:
                        obj.Scale = new Vector3(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
                        break;
                    case "Controller" when parts.Length == 3 && parts[2] == "Empty":
                        obj.Add(CreateController(parts[1]));
                        break;
                    case "Controller" when parts.Length == 2:
                    {
                        var c = CreateController(parts[1]);
                        ReadController(ref c, lines);
                        obj.Add(c);
                        break;
                    }
                    case "Mesh" when parts.Length == 2:
                        obj.Add(new MeshLoaderController {Asset = parts[1]});
                        break;
                    case "End" when parts.Length == 1:
                        return;
                    default:
                        throw new ApplicationException($"Invalid line in level file: `{line}` (Object {obj.Ref.Index})");
                }
            }
        }

        private static void ReadController(ref Controller controller, IEnumerator<string> lines)
        {
            var type = controller.GetType();
            while (lines.MoveNext())
            {
                var line = lines.Current;
                if(line!.StartsWith('#') || line.Length == 0) continue;
                var parts = line.Split(' ');
                if (parts[0] == "End") return;
                
                var setting = type.GetMember(parts[0]).Single();
                if (setting.MemberType != MemberTypes.Property && setting.MemberType != MemberTypes.Field)
                    throw new ApplicationException($"Cannot set setting {parts[0]}");
                object value = null;

                var t = setting.MemberType switch
                {
                    MemberTypes.Field => ((FieldInfo)setting).FieldType,
                    MemberTypes.Property => ((PropertyInfo)setting).PropertyType,
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                if (t == typeof(byte)) value    = byte.Parse(parts[1]);
                if (t == typeof(short)) value   = short.Parse(parts[1]);
                if (t == typeof(int)) value     = int.Parse(parts[1]);
                if (t == typeof(long)) value    = long.Parse(parts[1]);
                if (t == typeof(sbyte)) value   = sbyte.Parse(parts[1]);
                if (t == typeof(ushort)) value  = ushort.Parse(parts[1]);
                if (t == typeof(uint)) value    = uint.Parse(parts[1]);
                if (t == typeof(ulong)) value   = ulong.Parse(parts[1]);
                if (t == typeof(float)) value   = float.Parse(parts[1]);
                if (t == typeof(double)) value  = double.Parse(parts[1]);
                if (t == typeof(string) && parts[1].StartsWith('"') && parts[1].EndsWith('"'))
                    value = Regex.Unescape(parts[1][1..^1]);

                switch (setting.MemberType)
                {
                    case MemberTypes.Property:
                        ((PropertyInfo) setting).SetValue(controller, value);
                        break;
                    case MemberTypes.Field:
                        ((FieldInfo) setting).SetValue(controller, value);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static Controller CreateController(string name)
        {
            try
            {
                return (Controller) Activator.CreateInstance(_types.First(t =>
                    t.FullName == name ||
                    t.FullName == $"Castaway.Levels.Controllers.{name}Controller" ||
                    t.FullName == $"Castaway.Levels.Controllers.Rendering.{name}Controller" ||
                    t.FullName == $"Castaway.Levels.Controllers.Renderers.{name}Controller" ||
                    t.FullName == $"Castaway.Levels.Controllers.Storage.{name}Controller"));
            }
            catch (InvalidOperationException e)
            {
                throw new ApplicationException($"Controller was not found: {name}", e);
            }
        }
    }
}