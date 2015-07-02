using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Rocket.Unturned.Player;

namespace Safezone.Model
{
    [XmlInclude(typeof(RectangleType))]
    public abstract class SafeZoneType
    {
        public SafeZoneType()
        {
            // dummy
        }

        private static readonly Dictionary<String, Type> RegistereTypes = new Dictionary<String, Type>();

        public static SafeZoneType CreateSafeZoneType(String name)
        {
            if (!RegistereTypes.ContainsKey(name))
            {
                return null;
            }
            Type t = RegistereTypes[name];
            return (SafeZoneType)Activator.CreateInstance(t);
        }

        public static void RegisterSafeZoneType(String name, Type t)
        {
            if(t != typeof(SafeZoneType))
            {
                throw new ArgumentException(t.Name + " is not a SafeZoneType!");
            }

            if (RegistereTypes.ContainsKey(name))
            {
                throw new ArgumentException(name + " is already registered!");
            }

            RegistereTypes.Add(name, t);
        }

        public abstract String GetName();
        public abstract SafeZone Create(RocketPlayer player, String name, ArrayList args);
        public abstract bool IsInSafeZone(Position p);
        public abstract bool Redefine(RocketPlayer player, ArrayList args);
    }
}