using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Safezone.Model.Flag;

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
            if (t != typeof(Flag.Flag))
            {
                throw new ArgumentException("Can't get " + t.Name + " as flag!");
            }

            foreach (SerializableFlag serializedFlag in Flags)
            {
                Type type = Flag.Flag.GetFlagType(serializedFlag.Name);
                if (type != t) continue;

                Flag.Flag deserializedFlag = (Flag.Flag)Activator.CreateInstance(type);
                deserializedFlag.Value = serializedFlag.Value; 
                return deserializedFlag;
            }

            if (!createIfNotFound) return null;
            return (Flag.Flag)Activator.CreateInstance(t);
        }
    }
}