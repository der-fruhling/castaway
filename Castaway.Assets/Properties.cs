using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Castaway.Assets
{
    public static class PropertyReaders
    {
        public class Generic : IPropertyReader
        {
            public delegate object Func(string value);
            
            public Type ValidType { get; }
            private readonly Func _func;

            public object ReadValue(string value) => _func(value);

            public Generic(Type validType, Func func)
            {
                ValidType = validType;
                _func = func;
            }
        }

        public static readonly IPropertyReader Int16 = new Generic(typeof(short), new Int16Converter().ConvertFromString);
        public static readonly IPropertyReader Int32 = new Generic(typeof(int), new Int32Converter().ConvertFromString);
        public static readonly IPropertyReader Int64 = new Generic(typeof(long), new Int64Converter().ConvertFromString);
        public static readonly IPropertyReader UInt16 = new Generic(typeof(ushort), new UInt16Converter().ConvertFromString);
        public static readonly IPropertyReader UInt32 = new Generic(typeof(uint), new UInt32Converter().ConvertFromString);
        public static readonly IPropertyReader UInt64 = new Generic(typeof(ulong), new UInt64Converter().ConvertFromString);
        public static readonly IPropertyReader Bool = new Generic(typeof(bool), new BooleanConverter().ConvertFromString);
        public static readonly IPropertyReader String = new Generic(typeof(string), s => s);
    }
    
    public interface IPropertyReader
    {
        public Type ValidType { get; }
        
        public object ReadValue(string value);
    }

    public class PropertyException : ApplicationException
    {
        public PropertyException(string message) : base(message) { }
    }

    public class Properties<TEnum> where TEnum : struct, Enum
    {
        public struct Settings
        {
            public IPropertyReader PropertyReader;

            public Settings(IPropertyReader propertyReader)
            {
                PropertyReader = propertyReader;
            }
        }
        
        private readonly Dictionary<TEnum, object> _values = new Dictionary<TEnum, object>();
        private readonly Dictionary<TEnum, Settings> _settings;

        public Properties(Dictionary<TEnum, Settings> settings)
        {
            _settings = settings;
        }

        private void CheckValid(TEnum e)
        {
            if (!_settings.ContainsKey(e))
                throw new PropertyException($"{e} is not a complete property.");
        }

        public void Load(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                if(line.Length == 0) continue;

                var parts = line.Split("=");
                if (parts.Length != 2) throw new PropertyException($"Invalid line: {line}");
                
                if (!Enum.TryParse(parts[0], out TEnum e))
                    throw new PropertyException($"{e} is not a complete property.");
                CheckValid(e);
                
                _values[e] = _settings[e].PropertyReader.ReadValue(parts[1]);
            }
        }
        
        public T Get<T>(TEnum e)
        {
            var o = _values[e];
            CheckValid(e);
            if (o.GetType() != typeof(T))
                throw new PropertyException($"{o} does not have a valid type for {e}.");
            return (T) o;
        }

        public void Set<T>(TEnum e, T t)
        {
            CheckValid(e);
            var vt = _settings[e].PropertyReader.ValidType;
            if (t.GetType() != vt)
                throw new PropertyException($"Cannot set {e} to a value of type {t.GetType().FullName}. " +
                                            $"(requires {vt.FullName})");
            _values[e] = t;
        }
    }
}