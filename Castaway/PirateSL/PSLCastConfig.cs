using System;

namespace Castaway.PirateSL
{
    // ReSharper disable once InconsistentNaming
    public struct PSLCastConfig
    {
        // ReSharper disable once InconsistentNaming
        public enum PSLCastConfigType
        {
            Use,
            Input,
            Output,
            Transform
        }

        public readonly PSLCastConfigType Type;
        public readonly string Name;
        public readonly string Value;

        public PSLCastConfig(PSLCastConfigType type, string name, string value)
        {
            Type = type;
            Name = name;
            Value = value;
        }

        public bool Equals(PSLCastConfig other)
        {
            return Name == other.Name && Value == other.Value && Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            return obj is PSLCastConfig other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Name, Value);
        }

        public static bool operator ==(PSLCastConfig left, PSLCastConfig right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PSLCastConfig left, PSLCastConfig right)
        {
            return !left.Equals(right);
        }
    }
}