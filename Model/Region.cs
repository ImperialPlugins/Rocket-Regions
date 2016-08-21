using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Rocket.API;
using RocketRegions.Model.Flag;
using RocketRegions.Util;

namespace RocketRegions.Model
{
    public class Region
    {
        public string Name;
        public List<ulong> Owners;
        public RegionType.RegionType Type;
        public List<SerializableFlag> Flags;
        public List<ulong> Members;

        public void AddOwnerSafe(ulong owner)
        {
            if (Members.Contains(owner))
                Members.Remove(owner);

            if (Owners.Contains(owner))
                return;
            Owners.Add(owner);
        }

        public void AddMemberSafe(ulong member)
        {
            if (Owners.Contains(member) || Members.Contains(member))
                return;
            Members.Add(member);
        }

        public List<ulong> GetOwnersSafe() => Owners.Distinct().ToList();

        public List<ulong> GetMembersSafe()
        {
            foreach (var owner in Owners)
            {
                if(!Members.Contains(owner))
                    continue;
                Members.Remove(owner);
            }
            return Members.Distinct().ToList();
        }
        [XmlIgnore]
        private List<RegionFlag> _flags;

        [XmlIgnore]
        public ReadOnlyCollection<RegionFlag> ParsedFlags
        {
            get
            {
                if (_flags != null) return _flags.Where(c => RegionFlag.IsRegistered(c.Name)).ToList().AsReadOnly();
                if (Flags == null) return new List<RegionFlag>().AsReadOnly();

                _flags = new List<RegionFlag>();

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
            _flags = new List<RegionFlag>();

            foreach (var serializedFlag in Flags)
            {
                DeserializeFlag(serializedFlag);
            }
        }

        public RegionFlag GetFlag(Type t, bool createIfNotFound = true)
        {
            if (!t.IsSameOrSubclass(typeof(RegionFlag)))
            {
                throw new ArgumentException("Can't get type " + t.Name + " as flag!");
            }

            if (ParsedFlags != null && ParsedFlags.Count > 0)
            {
                var match = ParsedFlags.FirstOrDefault(c => c.GetType() == t);
                if (match != null) return match;
            }

            if (!createIfNotFound) return null;
            var flag = (RegionFlag)Activator.CreateInstance(t);
            flag.Name = RegionFlag.GetFlagName(t);
            return flag;
        }

        public Group GetGroup(IRocketPlayer player)
        {
            if(IsOwner(player)) return Group.OWNERS;
            var id = PlayerUtil.GetId(player);
            return GetAllMembers().Any(member => member == id) ? Group.MEMBERS : Group.NONMEMBERS;
        }

        public List<ulong> GetAllMembers()
        {
            var allMembers = Owners.ToList();
            if(Members != null)
                allMembers.AddRange(Members?.ToList());
            return allMembers.Distinct().ToList();
        }

        public void SetFlag(string name, object value, List<GroupValue> groupValues, bool save = true)
        {
            name = RegionFlag.GetPrimaryFlagName(name);
            var flagType = RegionFlag.GetFlagType(name);
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

            if (_flags == null) _flags = new List<RegionFlag>();
            foreach (RegionFlag f in ParsedFlags.Where(f => f.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)))
            {
                f.Value = value;
                return;
            }

            var deserializedFlag = DeserializeFlag(flag);
            if(deserializedFlag != null)
                deserializedFlag.Value = value;
        }

        private RegionFlag DeserializeFlag(SerializableFlag flag)
        {
            try
            {
                flag.Name = RegionFlag.GetPrimaryFlagName(flag.Name);

                var type = RegionFlag.GetFlagType(flag.Name);

                var deserializedFlag = (RegionFlag) Activator.CreateInstance(type);
                deserializedFlag.Region = this;
                deserializedFlag.Name = RegionFlag.GetFlagName(type);
                deserializedFlag.Value = flag.Value;

                foreach (var value in flag.GroupValues)
                {
                    deserializedFlag.SetValue(value.Value, GroupExtensions.GetGroup(value.GroupName));
                }

                deserializedFlag.GroupValues = flag.GroupValues ?? new List<GroupValue>();
                _flags.Add(deserializedFlag);
                return deserializedFlag;
            }
            catch (Exception)
            {
                //ignored
            }
            return null;
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