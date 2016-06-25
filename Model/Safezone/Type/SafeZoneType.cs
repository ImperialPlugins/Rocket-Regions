using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using Rocket.API;
using Safezone.Util;

namespace Safezone.Model.Safezone.Type
{
    [Serializable]
    [XmlInclude(typeof(RectangleType))]
    [XmlInclude(typeof(CircleType))]
    [XmlType(TypeName = "Type")]
    public abstract class SafeZoneType
    {
        private static readonly Dictionary<string, System.Type> RegistereTypes = new Dictionary<string, System.Type>();

        public static SafeZoneType RegisterSafeZoneType(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            name = name.ToLower();

            if (!RegistereTypes.ContainsKey(name))
            {
                return null;
            }
            var t = RegistereTypes[name];
            return (SafeZoneType)Activator.CreateInstance(t);
        }

        public static void RegisterSafeZoneType(string name, System.Type t)
        {
            if(!t.IsSameOrSubclass(typeof(SafeZoneType)))
            {
                throw new ArgumentException(t.Name + " is not a SafeZoneType!");
            }

            if (RegistereTypes.ContainsKey(name))
            {
                throw new ArgumentException(name + " is already registered!");
            }

            RegistereTypes.Add(name, t);
        }

        public abstract SafeZone OnCreate(IRocketPlayer player, string name, string[] args);
        public abstract bool OnRedefine(IRocketPlayer player, string[] args);
        public abstract double GetDistance(SerializablePosition pos);

        public abstract bool IsInSafeZone(SerializablePosition p);

        public static ReadOnlyCollection<string> GetTypes()
        {
            return new ReadOnlyCollection<string>(new List<string>(RegistereTypes.Keys));
        }
    }
}