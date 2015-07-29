using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using Rocket.API;
using Rocket.Unturned.Player;
using Safezone.Util;

namespace Safezone.Model.Safezone.Type
{
    [Serializable]
    [XmlInclude(typeof(RectangleType))]
    [XmlInclude(typeof(CircleType))]
    [XmlType(TypeName = "Type")]
    public abstract class SafeZoneType
    {
        private static readonly Dictionary<String, System.Type> RegistereTypes = new Dictionary<String, System.Type>();

        public static SafeZoneType RegisterSafeZoneType(String name)
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
            System.Type t = RegistereTypes[name];
            return (SafeZoneType)Activator.CreateInstance(t);
        }

        public static void RegisterSafeZoneType(String name, System.Type t)
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

        public abstract SafeZone OnCreate(IRocketPlayer player, String name, string[] args);
        public abstract bool OnRedefine(IRocketPlayer player, string[] args);
        public abstract double GetDistance(SerializablePosition pos);

        public abstract bool IsInSafeZone(SerializablePosition p);

        public static ReadOnlyCollection<String> GetTypes()
        {
            return new ReadOnlyCollection<string>(new List<string>(RegistereTypes.Keys));
        }
    }
}