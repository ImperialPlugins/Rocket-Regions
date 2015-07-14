using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Rocket.Unturned.Player;
using Safezone.Util;

namespace Safezone.Model.Flag
{
    public abstract class Flag
    {
        private static readonly Dictionary<String, Type> RegisteredFlags = new Dictionary<String, Type>();
        public String Name;
        public Object Value;

        public abstract String Description { get;  }
        public abstract Object DefaultValue { get;  }

        public T GetValue<T>()
        {
            if (!Value.GetType().IsSameOrSubclass(typeof(T)))
            {
                throw new InvalidCastException("Can't cast " + Value.GetType().Name + " to " + typeof(T).Name);
            }
            return (T) Value;
        }

        public T GetDefaultValue<T>()
        {
            if (!DefaultValue.GetType().IsSameOrSubclass(typeof(T)))
            {
                throw new InvalidCastException("Can't cast " + Value.GetType().Name + " to " + typeof(T).Name);
            }
            return (T)DefaultValue;
        }

        public abstract bool OnSetValue(RocketPlayer caller, SafeZone safeZone, params string[] values);
        public abstract string Usage { get; }

        protected Flag(String name)
        {
            Name = name;
            Value = DefaultValue;
        }
        
        public static void RegisterFlag(String name, Type type)
        {
            name = name.ToLower();
            if (!type.IsSameOrSubclass(typeof(Flag)))
            {
                throw new ArgumentException(type.FullName + " does not extend the abstract Flag type!");
            }

            if (RegisteredFlags.ContainsKey(name))
            {
                throw new ArgumentException("\"" + name + "\" is already a registered flag type!");
            }

            RegisteredFlags.Add(name, type);
        }

        public static Type GetFlagType(String name)
        {
            name = name.ToLower();
            return !RegisteredFlags.ContainsKey(name) ? null : RegisteredFlags[name];
        }
    }
}