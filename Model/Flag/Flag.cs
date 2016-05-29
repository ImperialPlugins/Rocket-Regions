using System;
using System.Collections.Generic;
using Rocket.API;
using Safezone.Model.Safezone;
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

        public virtual bool SupportsGroups
        {
            get { return true;  }
        }

        public Dictionary<string, object> GroupValues = new Dictionary<string, object>(); 

        public virtual T GetValue<T>(Group group)
        {
            if (!SupportsGroups)
            {
                throw new InvalidOperationException("Flag does not support group specific");
            }
            if (!Value.GetType().IsSameOrSubclass(typeof(T)))
            {
                throw new InvalidCastException("Can't cast " + Value.GetType().Name + " to " + typeof(T).Name);
            }

            String name = group.GetInternalGroupName();
            if (GroupValues != null && GroupValues.ContainsKey(name))
            {
                return (T) GroupValues[name];
            }
            return (T) Value;
        }

        public virtual T GetValue<T>()
        {
            if (!Value.GetType().IsSameOrSubclass(typeof(T)))
            {
                throw new InvalidCastException("Can't cast " + Value.GetType().Name + " to " + typeof(T).Name);
            }

            return (T)Value;
        }

        public virtual T GetDefaultValue<T>()
        {
            if (!DefaultValue.GetType().IsSameOrSubclass(typeof(T)))
            {
                throw new InvalidCastException("Can't cast " + Value.GetType().Name + " to " + typeof(T).Name);
            }
            return (T)DefaultValue;
        }

        public abstract bool OnSetValue(IRocketPlayer caller, SafeZone safeZone, string rawValue, Group group = Group.NONE);
        public abstract string Usage { get; }

        protected void SetValue(object value, Group group)
        {
            if (group == Group.NONE)
            {
                Value = value;
                return;
            }

            String groupName = group.GetInternalGroupName();
            if (GroupValues.ContainsKey(groupName))
            {
                GroupValues[groupName] = value;
                return;
            }
            GroupValues.Add(groupName, value);
        }

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