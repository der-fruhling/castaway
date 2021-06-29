using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Castaway.Base;
using static Castaway.Base.Ex;

namespace Castaway.Data
{
    [AttributeUsage(AttributeTargets.Field)]
    public class UnitAttribute : Attribute
    {
    }

    public abstract class SaveFormat
    {
        private static readonly byte[] Magic = {54, 46, 12, 0x1a};
        private FieldInfo[] _fields;
        private bool _init;

        private void Init()
        {
            _fields = GetType().GetFields()
                .Where(HasAttribute<UnitAttribute>)
                .ToArray();
            _init = true;
        }

        public async Task Save(string path)
        {
            await Task.Run(async () =>
            {
                var logger = CastawayGlobal.GetLogger(typeof(SaveFormat));

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                logger.Debug("Starting save of {$Type}", this);

                if (!_init) Init();
                await using var file = File.OpenWrite(path);
                var i = 0;
                var d = new Dictionary<string, (int Index, int Size)>();
                var toSave = _fields.Where(HasAttribute<UnitAttribute>).ToArray();

                await file.WriteAsync(Magic
                    .Concat(BitConverter.GetBytes(toSave.Length))
                    .ToArray());
                logger.Verbose("Finished writing header");

                logger.Verbose("{Count} units to push", toSave.Length);
                logger.Verbose("Starting unit list");

                foreach (var f in toSave)
                {
                    var c = f.GetCustomAttribute<UnitAttribute>();

                    var size = f.GetValue(this) switch
                    {
                        string s => s.Length,
                        { } v => Marshal.SizeOf(v)
                    };
                    d[f.Name] = (i, size);

                    await file.WriteAsync(
                        BitConverter.GetBytes(f.Name.Length)
                            .Concat(Encoding.UTF8.GetBytes(f.Name))
                            .Concat(BitConverter.GetBytes(i))
                            .Concat(BitConverter.GetBytes(size))
                            .ToArray());

                    i += size;
                }

                logger.Verbose("Starting unit data");
                var basePos = file.Position;
                foreach (var f in toSave)
                {
                    var (index, size) = d[f.Name];
                    file.Position = basePos + index;
                    var v = f.GetValue(this);
                    var ptr = Marshal.AllocHGlobal(size);
                    if (v is string s) Marshal.Copy(Encoding.UTF8.GetBytes(s), 0, ptr, size);
                    else Marshal.StructureToPtr(v!, ptr, false);
                    var bytes = new byte[size].Select((_, j) => Marshal.ReadByte(ptr, j)).ToArray();
                    await file.WriteAsync(bytes.AsMemory());
                    logger.Verbose(
                        "Wrote data ({Size} bytes) for unit {Unit}({Type}) at {Index} (actually {ActualPos})",
                        size, f.Name, f.FieldType, index, basePos + index);
                }

                await file.FlushAsync();
                stopwatch.Stop();

                logger.Information("Wrote save data {$This} in {Time}ms", this, stopwatch.Elapsed.TotalMilliseconds);
            });
        }

        public async Task Load(string path)
        {
            await Task.Run(async () =>
            {
                var logger = CastawayGlobal.GetLogger(typeof(SaveFormat));

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                logger.Debug("Reading save data {$This} from {Path}", this, path);

                if (!_init) Init();
                await using var file = File.OpenRead(path);

                var readMagic = new byte[Magic.Length];
                await file.ReadAsync(readMagic.AsMemory());
                if (!readMagic.SequenceEqual(Magic)) throw new InvalidOperationException("Cannot read non save file.");

                var countBytes = new byte[sizeof(int)];
                await file.ReadAsync(countBytes.AsMemory());
                var count = BitConverter.ToInt32(countBytes);

                var d = new Dictionary<FieldInfo, (int Index, int Size)?>();

                foreach (var f in GetType().GetFields().Where(HasAttribute<UnitAttribute>)) d[f] = null;

                logger.Verbose("Searching for {Count} units...", count);
                for (var i = 0; i < count; i++)
                {
                    await file.ReadAsync(countBytes.AsMemory());
                    var unitNameSize = BitConverter.ToInt32(countBytes);

                    var unitNameBytes = new byte[unitNameSize];
                    await file.ReadAsync(unitNameBytes.AsMemory());
                    var unitName = Encoding.UTF8.GetString(unitNameBytes);

                    var indexBytes = new byte[sizeof(int)];
                    await file.ReadAsync(indexBytes.AsMemory());
                    var index = BitConverter.ToInt32(indexBytes);

                    var sizeBytes = new byte[sizeof(int)];
                    await file.ReadAsync(sizeBytes.AsMemory());
                    var size = BitConverter.ToInt32(sizeBytes);

                    var f = GetType().GetField(unitName);
                    if (f != null) d[f] = (index, size);
                    else logger.Warning("Invalid unit name {Unit}, in {Path} : {$This}", unitName, path, this);
                }

                var basePos = file.Position;
                foreach (var (f, tup) in d)
                {
                    if (tup == null)
                    {
                        try
                        {
                            f.SetValue(this, null);
                            logger.Warning("No value for unit {Unit} from save file {Path} : {$This}, set to null",
                                f.Name, path, this);
                        }
                        catch (ArgumentException e)
                        {
                            throw new AggregateException(
                                $"Required unit {f.Name} was not present in this save, and cannot be set to null.",
                                e);
                        }

                        continue;
                    }

                    var (index, size) = ((int, int)) tup;
                    file.Position = basePos + index;
                    var bytes = new byte[size];
                    await file.ReadAsync(bytes.AsMemory());

                    logger.Verbose("Reading {Name}({Type}) containing {Count} bytes from {Index}",
                        f.Name, f.FieldType, size, index);
                    if (f.FieldType.IsValueType)
                        unsafe
                        {
                            fixed (void* p = &bytes[0])
                            {
                                f.SetValue(this, Marshal.PtrToStructure((IntPtr) p, f.FieldType));
                            }
                        }
                    else if (f.FieldType == typeof(string)) f.SetValue(this, Encoding.UTF8.GetString(bytes));
                    else throw new InvalidOperationException($"Unknown readable type {f.FieldType}");
                }

                stopwatch.Stop();
                logger.Information("Read save data {$This} in {Time}ms", this, stopwatch.Elapsed.TotalMilliseconds);
            });
        }
    }
}