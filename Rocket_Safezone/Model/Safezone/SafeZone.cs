using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Rocket.API;
using Safezone.Model.Flag;
using Safezone.Model.Safezone.Type;
using Safezone.Util;

namespace Safezone.Model.Safezone
{
    [Serializable]
    [XmlType(TypeName = "Safezone")] 
    public class SafeZone
    {
        [XmlAttribute("Name")]
        public string Name;
        [XmlAttribute("Owner")]
        public uint Owner;
        [XmlElement("Type")]
        public SafeZoneType Type;

        [XmlArray("Flags")]
        public List<SerializableFlag> Flags;

        public Flag.Flag GetFlag(System.Type t, bool createIfNotFound = true)
        {
            if (!t.IsSameOrSubclass(typeof(Flag.Flag)))
            {
                throw new ArgumentException("Can't get " + t.Name + " as flag!");
            }

            if (Flags != null && Flags.Count > 0)
            {
                foreach (SerializableFlag serializedFlag in Flags)
                {
                    if (serializedFlag == null)
                    {
                        Flags.Remove(serializedFlag);
                        continue;
                    }
                    System.Type type = Flag.Flag.GetFlagType(serializedFlag.Name);
                    if (type != t || type == null) continue;

                    Flag.Flag deserializedFlag = (Flag.Flag) Activator.CreateInstance(type);
                    deserializedFlag.Value = serializedFlag.Value;
                    return deserializedFlag;
                }
            }

            if (!createIfNotFound) return null;
            return (Flag.Flag)Activator.CreateInstance(t);
        }

        public void SetFlag(string name, object value, bool save = true)
        {
            System.Type flagType = Flag.Flag.GetFlagType(name);
            if (flagType == null)
            {
                throw new ArgumentException("Unknown flag: " + name);
            }

            if (Flags == null)
            {
                Flags = new List<SerializableFlag>();
            }

            foreach (SerializableFlag f in Flags)
            {
                if (f.Name != name) continue;
                Flags.Remove(f);
                break;
            }

            SerializableFlag flag = new SerializableFlag {Name = name, Value = value};
            Flags.Add(flag);
            if (save)
            {
                SafeZonePlugin.Instance.Configuration.Save();
            }
        }

        public bool IsOwner(IRocketPlayer player)
        {
            if (player is ConsolePlayer)
            {
                return true;
            }
            return IsOwner(SafeZonePlugin.GetId(player));
        }

        public bool IsOwner(uint id)
        {
            return Owner == id;
        }
    }
}