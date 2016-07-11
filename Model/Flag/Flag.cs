using System;
using System.Collections.Generic;
using Rocket.API;
using Safezone.Model.Safezone;
using Safezone.Util;
using System.Linq;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using Steamworks;
using UnityEngine;

namespace Safezone.Model.Flag
{
    public abstract class Flag
    {
        private SafeZone _safezone;
        public SafeZone SafeZone
        {
            get
            {
                return _safezone;
            }
            internal set
            {
                if (value == null) throw new ArgumentNullException(nameof(value), "SafeZone value can not be null");
                if (_safezone == null)
                    _safezone = value;
                else 
                    Logger.Log($"Flag \"{Name}\": Tried to set new SafeZone value! Old value: {_safezone.Name}, new value: {value.Name}");
            } 
        }

        internal static readonly Dictionary<string, Type> RegisteredFlags = new Dictionary<string, Type>();
        public string Name;
        private object _value;

        public virtual object Value
        {
            get
            {
                return _value;
            }
            internal set
            {
                OnValueUpdate(_value, value);
                _value = value;
            }
        }

        public abstract string Description { get; }

        public virtual bool SupportsGroups => true;

        public List<GroupValue> GroupValues = new List<GroupValue>();

        public virtual T GetValue<T>(Group group)
        {
            if (!SupportsGroups)
            {
                throw new InvalidOperationException("Flag does not support group specific");
            }
            if (Value != null && !Value.GetType().IsSameOrSubclass(typeof(T)))
            {
                throw new InvalidCastException("Can't cast " + Value.GetType().Name + " to " + typeof(T).Name);
            }

            var name = group.GetSerializableName();
            var v = GroupValues?.FirstOrDefault(g => g?.GroupName == name);
            if (GroupValues != null && v != null)
            {
                if (v.Value == null)
                {
                    return default(T);
                }
                return (T)v.Value;
            }

            if (Value == null)
            {
                return default(T);
            }

            return (T)Value;
        }

        public virtual T GetValue<T>()
        {
            if (Value != null && !Value.GetType().IsSameOrSubclass(typeof(T)))
            {
                throw new InvalidCastException("Can't cast " + Value.GetType().Name + " to " + typeof(T).Name);
            }

            return (T)Value;
        }

        public abstract bool ParseValue(IRocketPlayer caller, SafeZone safeZone, string rawValue, Group group = Group.NONE);

        protected virtual void OnValueUpdate(object oldValue, object newValue)
        {

        }

        public abstract string Usage { get; }

        public void SetValue(object value, Group group)
        {
            if (group == Group.NONE)
            {
                Value = value;
                return;
            }

            var groupName = group.GetSerializableName();

            var v = GroupValues.FirstOrDefault(g => g.GroupName == groupName);
            if (v != null)
            {
                v.Value = value;
            }
            else
            {
                GroupValues.Add(new GroupValue() { GroupName = groupName, Value = value });
            }
        }

        public static void RegisterFlag(string name, Type type)
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

        public static Type GetFlagType(string name)
        {
            return (from f in RegisteredFlags where f.Key.Equals(name, StringComparison.CurrentCultureIgnoreCase) select f.Value).FirstOrDefault();
        }

        public abstract void UpdateState(List<UnturnedPlayer> players);
        public abstract void OnSafeZoneEnter(UnturnedPlayer player);
        public abstract void OnSafeZoneLeave(UnturnedPlayer player);

        public static string GetFlagName(Type type)
        {
            return RegisteredFlags.FirstOrDefault(c => c.Value == type).Key;
        }

        public virtual void OnPlayerUpdatePosition(UnturnedPlayer player, Vector3 position)
        {

        }
    }
}