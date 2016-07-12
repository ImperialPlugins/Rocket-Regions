using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Rocket.API;
using RocketRegions.Model.Flag;
using RocketRegions.Model.Safezone.Type;
using RocketRegions.Util;

namespace RocketRegions.Model.Safezone
{
    public class Region
    {
        public string Name;
        public List<ulong> Owners;
        public RegionType Type;
        public List<SerializableFlag> Flags;
        public List<ulong> Members;

        [XmlIgnore]
        private List<Flag.RegionFlag> _flags;

        [XmlIgnore]
        public ReadOnlyCollection<Flag.RegionFlag> ParsedFlags
        {
            get
            {
                if (_flags != null) return _flags.AsReadOnly();
                if (Flags == null) return new List<Flag.RegionFlag>().AsReadOnly();

                _flags = new List<Flag.RegionFlag>();

                foreach (var serializedFlag in Flags)
                {
                    if (serializedFlag == null)
                    {
                        Flags.Remove(null);
                        continue;
                    }

                    DeserializeFlag(serializedFlag);
                }

                return _flags.AsReadOnly();
            }
        }

        public void RebuildFlags()
        {
            _flags = new List<Flag.RegionFlag>();

            foreach (var serializedFlag in Flags)
            {
                DeserializeFlag(serializedFlag);
            }
        }

        public Flag.RegionFlag GetFlag(System.Type t, bool createIfNotFound = true)
        {
            if (!t.IsSameOrSubclass(typeof(Flag.RegionFlag)))
            {
                throw new ArgumentException("Can't get type " + t.Name + " as flag!");
            }

            if (ParsedFlags != null && ParsedFlags.Count > 0)
            {
                var match = ParsedFlags.FirstOrDefault(c => c.GetType() == t);
                if (match != null) return match;
            }

            if (!createIfNotFound) return null;
            var flag = (Flag.RegionFlag)Activator.CreateInstance(t);
            flag.Name = Flag.RegionFlag.GetFlagName(t);
            return flag;
        }

        public Group GetGroup(IRocketPlayer player)
        {
            var id = PlayerUtil.GetId(player);
            return GetAllMembers().Any(member => member == id) ? Group.MEMBERS : Group.NONMEMBERS;
        }

        public List<ulong> GetAllMembers()
        {
            var allMembers = Owners;
            if (Members == null) Members = new List<ulong>();
            allMembers.AddRange(Members);
            return allMembers;
        }

        public void SetFlag(string name, object value, List<GroupValue> groupValues, bool save = true)
        {
            name = Flag.RegionFlag.GetPrimaryFlagName(name);
            var flagType = Flag.RegionFlag.GetFlagType(name);
            if (flagType == null)
            {
                throw new ArgumentException("Unknown flag: " + name);
            }

            if (Flags == null)
            {
                Flags = new List<SerializableFlag>();
            }

            foreach (var f in Flags.Where(f => f.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)))
            {
                Flags.Remove(f);
                break;
            }

            var flag = new SerializableFlag
            {
                Name = name,
                Value = value,
                GroupValues = groupValues
            };
            Flags.Add(flag);
            if (save)
            {
                RegionsPlugin.Instance.Configuration.Save();
            }

            if (_flags == null) _flags = new List<Flag.RegionFlag>();
            foreach (Flag.RegionFlag f in ParsedFlags.Where(f => f.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)))
            {
                f.Value = value;
                return;
            }

            var deserializedFlag = DeserializeFlag(flag);
            deserializedFlag.Value = value;
        }

        private Flag.RegionFlag DeserializeFlag(SerializableFlag flag)
        {
            flag.Name = Flag.RegionFlag.GetPrimaryFlagName(flag.Name);

            var type = Flag.RegionFlag.GetFlagType(flag.Name);

            var deserializedFlag = (Flag.RegionFlag)Activator.CreateInstance(type);
            deserializedFlag.Region = this;
            deserializedFlag.Name = Flag.RegionFlag.GetFlagName(type);
            deserializedFlag.Value = flag.Value;

            foreach (var value in flag.GroupValues)
            {
                deserializedFlag.SetValue(value.Value, GroupExtensions.GetGroup(value.GroupName));
            }

            deserializedFlag.GroupValues = flag.GroupValues ?? new List<GroupValue>();
            _flags.Add(deserializedFlag);
            return deserializedFlag;
        }

        public bool IsOwner(IRocketPlayer player)
        {
            if (player is ConsolePlayer)
            {
                return true;
            }
            return IsOwner(PlayerUtil.GetId(player));
        }

        public bool IsOwner(ulong id)
        {
            return Owners.Any(owner => owner == id);
        }
    }
}