using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Castaway.Assets
{
    /// <summary>
    /// A collection of built-in <see cref="IPropertyReader"/> instances to
    /// load a bunch of primitive types.
    /// </summary>
    public static class PropertyReaders
    {
        private class Generic : IPropertyReader
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
    
    /// <summary>
    /// Interface class for defining a way to read a property value from text.
    /// </summary>
    public interface IPropertyReader
    {
        /// <summary>
        /// The type of object this reader reads.
        /// </summary>
        public Type ValidType { get; }
        
        /// <summary>
        /// Reads a property value from a string.
        /// </summary>
        /// <param name="value">Text value of a property.</param>
        /// <returns>Object of type <see cref="ValidType"/>, created from
        /// <paramref name="value"/></returns>
        public object ReadValue(string value);
    }

    /// <summary>
    /// Stores object values based on an enum.
    /// </summary>
    /// <typeparam name="TEnum">Enum type to use.</typeparam>
    public class Properties<TEnum> where TEnum : struct, Enum
    {
        /// <summary>
        /// Property reading settings for an enum value.
        /// </summary>
        public struct Settings
        {
            /// <summary>
            /// Reader to use for this key.
            /// </summary>
            public IPropertyReader PropertyReader;

            public Settings(IPropertyReader propertyReader)
            {
                PropertyReader = propertyReader;
            }
        }
        
        private readonly Dictionary<TEnum, object> _values = new Dictionary<TEnum, object>();
        private readonly Dictionary<TEnum, Settings> _settings;

        /// <summary>
        /// Constructs a new <see cref="Properties{TEnum}"/> collection.
        /// </summary>
        /// <param name="settings">A dictionary of <see cref="Settings"/>
        /// values to use. Every enum value should have an entry here.
        /// </param>
        public Properties(Dictionary<TEnum, Settings> settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Checks if an enum value is valid.
        /// </summary>
        /// <param name="e">Value to check.</param>
        /// <exception cref="ApplicationException">Thrown if
        /// <paramref name="e"/> does not have a settings entry.</exception>
        private void CheckValid(TEnum e)
        {
            if (!_settings.ContainsKey(e))
                throw new ApplicationException($"{e} is not a complete property.");
        }

        /// <summary>
        /// Loads some properties from <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">Lines to load from.</param>
        /// 
        /// <exception cref="ApplicationException">Thrown if an invalid line is
        /// encountered</exception>
        /// <exception cref="ApplicationException">Thrown if an invalid property
        /// is used.</exception>
        /// <exception cref="ApplicationException">Thrown by
        /// <see cref="CheckValid"/></exception>
        public void Load(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                if(line.Length == 0) continue;

                var parts = line.Split("=");
                if (parts.Length != 2) throw new ApplicationException($"Invalid property line: {line}");
                
                if (!Enum.TryParse(parts[0], out TEnum e))
                    throw new ApplicationException($"{e} is not a valid property.");
                CheckValid(e);
                
                _values[e] = _settings[e].PropertyReader.ReadValue(parts[1]);
            }
        }
        
        /// <summary>
        /// Gets a values of type <typeparamref name="T"/> from property
        /// <paramref name="e"/>.
        /// </summary>
        /// <param name="e">Property to load from.</param>
        /// <typeparam name="T">Type of output.</typeparam>
        /// <returns>Value of property <paramref name="e"/></returns>
        /// <exception cref="ApplicationException">Thrown if the types do not
        /// match.</exception>
        public T Get<T>(TEnum e)
        {
            var o = _values[e];
            CheckValid(e);
            if (o.GetType() != typeof(T))
                throw new ApplicationException($"{o} is not a valid type for property {e}. (requires " +
                                               $"{_settings[e].PropertyReader.ValidType.FullName}");
            return (T) o;
        }

        /// <summary>
        /// Sets the value of property <paramref name="e"/> to value
        /// <paramref name="t"/>.
        /// </summary>
        /// <param name="e">Property to set.</param>
        /// <param name="t">Value to set <paramref name="e"/> to.</param>
        /// <typeparam name="T">Type of <paramref name="t"/>.</typeparam>
        /// <exception cref="ApplicationException">Thrown if the types do not
        /// match.</exception>
        public void Set<T>(TEnum e, T t)
        {
            CheckValid(e);
            var vt = _settings[e].PropertyReader.ValidType;
            if (t.GetType() != vt)
                throw new ApplicationException($"Cannot set property {e} to a value of type {t.GetType().FullName}. " +
                                            $"(requires {vt.FullName})");
            _values[e] = t;
        }
    }
}