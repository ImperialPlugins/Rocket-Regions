using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Rocket.Unturned.Plugins;
using Safezone.Model.Flag;
using Safezone.Util;

namespace Safezone.Model
{
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

        public Flag.Flag GetFlag(Type t, bool createIfNotFound = true)
        {
            if (!t.IsSameOrSubclass(typeof(Flag.Flag)))
            {
                throw new ArgumentException("Can't get " + t.Name + " as flag!");
            }

            foreach (SerializableFlag serializedFlag in Flags)
            {
                if (serializedFlag == null)
                {
                    Flags.Remove(serializedFlag);
                    continue;
                }
                Type type = Flag.Flag.GetFlagType(serializedFlag.Name);
                if (type != t || type == null) continue;

                Flag.Flag deserializedFlag = (Flag.Flag)Activator.CreateInstance(type);
                deserializedFlag.Value = serializedFlag.Value; 
                return deserializedFlag;
            }

            if (!createIfNotFound) return null;
            return (Flag.Flag)Activator.CreateInstance(t);
        }

        public void SetFlag(string name, object value, bool save = true)
        {
            Type flagType = Flag.Flag.GetFlagType(name);
            if (flagType == null)
            {
                throw new ArgumentException("Unknown flag: " + name);
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

        public bool IsOwner(uint id)
        {
            return Owner == id;
        }
    }
}