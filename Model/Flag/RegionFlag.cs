using System;
using System.Collections.Generic;
using Rocket.API;
using RocketRegions.Util;
using System.Linq;
using Rocket.Unturned.Player;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace RocketRegions.Model.Flag
{
    public abstract class RegionFlag
    {
        private Region _region;
        public Region Region
        {
            get { return _region; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value), "Region value can not be null");
                if (_region == null)
                    _region = value;
                else
                    Logger.Log($"Flag \"{Name}\": Tried to set new region value! Old value: {_region.Name}, new value: {value.Name}");
            }
        }

        internal static readonly Dictionary<string, Type> RegisteredFlags = new Dictionary<string, Type>();
        public string Name;
        private object _value;

        public virtual object Value
        {
            get { return _value; }
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

            if (group == Group.ALL)
                return (T)Value;

            var name = group.GetSerializableName();
            var v = GroupValues?.FirstOrDefault(g => g?.GroupName == name);

            if (v?.Value != null)
            {
                return (T)v.Value;
            }

            return group != Group.ALL ? GetValue<T>(Group.ALL) : default(T);
        }

        public virtual T GetValue<T>()
        {
            if (Value != null && !Value.GetType().IsSameOrSubclass(typeof(T)))
            {
                throw new InvalidCastException("Can't cast " + Value.GetType().Name + " to " + typeof(T).Name);
            }

            return (T)Value;
        }

        public abstract bool ParseValue(IRocketPlayer caller, Region region, string[] command, out string valueShown, Group group = Group.ALL);

        protected virtual void OnValueUpdate(object oldValue, object newValue)
        {

        }

        public abstract string Usage { get; }

        public void SetValue(object value, Group group)
        {
            if (group == Group.ALL)
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

        public static void RegisterFlag(string name, Type type, List<string> aliases = null)
        {
            name = name.Trim().ToLower();
            if (!type.IsSameOrSubclass(typeof(RegionFlag)))
            {
                throw new ArgumentException(type.FullName + " does not extend the abstract Flag type!");
            }

            if (RegisteredFlags.ContainsKey(name))
            {
                //throw new ArgumentException("\"" + name + "\" is already a registered flag type!");
                RegisteredFlags.Remove(name);
            }

            RegisteredFlags.Add(name, type);
            if (aliases == null || aliases.Count == 0) return;
            foreach (string s in aliases)
            {
                RegisteredFlags.Add(s.ToLower(), type);
            }
        }

        public static Type GetFlagType(string name) => (from f in RegisteredFlags where f.Key.Equals(name, StringComparison.CurrentCultureIgnoreCase) select f.Value).FirstOrDefault();

        public static string GetPrimaryFlagName(string alias)
        {
            var type = GetFlagType(alias);
            return GetFlagName(type) ?? alias;
        }

        public abstract void UpdateState(List<UnturnedPlayer> players);
        public abstract void OnRegionEnter(UnturnedPlayer player);
        public abstract void OnRegionLeave(UnturnedPlayer player);

        public static string GetFlagName(Type type) => RegisteredFlags.FirstOrDefault(c => c.Value == type).Key;

        public static bool IsRegistered(string name) => RegisteredFlags.ContainsKey(name.ToLower().Trim());

        public static void UnregisterFlag(string name)
        {
            name = name.ToLower().Trim();
            RegisteredFlags.Remove(name);
        }

        public virtual void OnPlayerUpdatePosition(UnturnedPlayer player, Vector3 position)
        {

        }
    }
}