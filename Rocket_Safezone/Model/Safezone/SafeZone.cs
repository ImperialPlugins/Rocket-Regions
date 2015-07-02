using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

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
        public List<Flag.Flag> Flags;

        public Flag.Flag GetFlag(Type t, bool createIfNotFound = true)
        {
            if (t != typeof(Flag.Flag))
            {
                throw new ArgumentException("Can't get " + t.Name + " as flag!");
            }

            foreach (Flag.Flag flag in Flags.Where(flag => flag.GetType().FullName == t.FullName))
            {
                return flag;
            }

            if (!createIfNotFound) return null;

            return (Flag.Flag)Activator.CreateInstance(t);
        }
    }
}