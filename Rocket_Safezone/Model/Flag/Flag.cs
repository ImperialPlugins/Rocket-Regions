using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Net;
using System.Xml.Serialization;
using Rocket.Unturned.Player;

namespace Safezone.Model.Flag
{
    [Serializable]
    [XmlType(AnonymousType = false)]
    public abstract class Flag
    {
        private static readonly Dictionary<String, Type> RegisteredFlags = new Dictionary<String, Type>();
        public String Name;
        public Object Value;

        public T GetValue<T>()
        {
            if (!(Value is T))
            {
                throw new InvalidOperationException("Can't cast " + Value.GetType().Name + " to " + typeof(T).Name);
            }
            return (T) Value;
        }

        public T GetDefaultValue<T>()
        {
            if (!(_defaultValue is T))
            {
                throw new InvalidOperationException("Can't cast " + Value.GetType().Name + " to " + typeof(T).Name);
            }
            return (T)_defaultValue;
        }

        public abstract bool OnSetValue(RocketPlayer caller, string[] args);

        private readonly Object _defaultValue;
        protected Flag(String name, Object defaultValue)
        {
            Name = name;
            Value = defaultValue;
            _defaultValue = defaultValue;
        }
        
        public static void RegisterFlag(String name, Type type)
        {
            if (!typeof (Flag).IsAssignableFrom(type))
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
            if (!RegisteredFlags.ContainsKey(name))
            {
                throw new ArgumentException("\"" + name + "\" is an unknown flag!");
            }

            return RegisteredFlags[name];
        }
    }
}