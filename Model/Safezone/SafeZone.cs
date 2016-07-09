using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Rocket.API;
using Safezone.Model.Flag;
using Safezone.Model.Safezone.Type;
using Safezone.Util;

namespace Safezone.Model.Safezone
{
    public class SafeZone
    {
        public string Name;
        public List<ulong> Owners;
        public SafeZoneType Type;
        public List<SerializableFlag> Flags;
        public List<ulong> Members;

        [XmlIgnore]
        private List<Flag.Flag> _flags;

        [XmlIgnore]
        public ReadOnlyCollection<Flag.Flag> ParsedFlags
        {
            get
            {
                if (_flags != null) return _flags.AsReadOnly();
                if (Flags == null) return new List<Flag.Flag>().AsReadOnly();

                _flags = new List<Flag.Flag>();

                foreach (var serializedFlag in Flags)
                {
                    if (serializedFlag == null)
                    {
                        Flags.Remove(null);
                        continue;
                    }

                    _flags.Add(DeserializeFlag(serializedFlag));
                }

                return _flags.AsReadOnly();
            }
        }

        public void RebuildFlags()
        {
            _flags = new List<Flag.Flag>();

            foreach (var serializedFlag in Flags)
            {
                _flags.Add(DeserializeFlag(serializedFlag));
            }
        }

        public Flag.Flag GetFlag(System.Type t, bool createIfNotFound = true)
        {
            if (!t.IsSameOrSubclass(typeof(Flag.Flag)))
            {
                throw new ArgumentException("Can't get type " + t.Name + " as flag!");
            }

            if (ParsedFlags != null && ParsedFlags.Count > 0)
            {
                var match = ParsedFlags.FirstOrDefault(c => c.GetType() == t);
                if (match != null) return match;
            }

            if (!createIfNotFound) return null;
            var flag = (Flag.Flag)Activator.CreateInstance(t);
            flag.Name = Flag.Flag.GetFlagName(t);
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
            var flagType = Flag.Flag.GetFlagType(name);
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
                SafeZonePlugin.Instance.Configuration.Save();
            }

            if (_flags == null) _flags = new List<Flag.Flag>();
            foreach (Flag.Flag f in ParsedFlags.Where(f => f.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)))
            {
                f.Value = value;
                return;
            }

            var deserializedFlag = DeserializeFlag(flag);
            deserializedFlag.Value = value;
            _flags.Add(deserializedFlag);
        }

        private Flag.Flag DeserializeFlag(SerializableFlag flag)
        {
            var type = Flag.Flag.GetFlagType(flag.Name);

            var deserializedFlag = (Flag.Flag)Activator.CreateInstance(type);
            deserializedFlag.Name = Flag.Flag.GetFlagName(type);
            deserializedFlag.Value = flag.Value;

            foreach (var value in flag.GroupValues)
            {
                deserializedFlag.SetValue(value.Value, GroupExtensions.GetGroup(value.GroupName));
            }

            deserializedFlag.GroupValues = flag.GroupValues ?? new List<GroupValue>();
            deserializedFlag.SafeZone = this;
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