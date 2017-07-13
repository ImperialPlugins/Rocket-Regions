using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using Rocket.API;
using RocketRegions.Util;

namespace RocketRegions.Model.RegionType
{
    [Serializable]
    [XmlInclude(typeof(RectangleType))]
    [XmlInclude(typeof(CircleType))]
    [XmlType(TypeName = "RegionType")]
    public abstract class RegionType
    {
        internal static readonly Dictionary<string, Type> RegisteredTypes = new Dictionary<string, Type>();

        public static RegionType RegisterRegionType(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            name = name.ToLower();

            if (!RegisteredTypes.ContainsKey(name))
                return null;
            var t = RegisteredTypes[name];
            return (RegionType)Activator.CreateInstance(t);
        }

        public static void RegisterRegionType(string name, Type t)
        {
            if (!t.IsSameOrSubclass(typeof(RegionType)))
                throw new ArgumentException(t.Name + " is not a RegionType!");

            if (RegisteredTypes.ContainsKey(name))
                //throw new ArgumentException(name + " is already registered!");
                RegisteredTypes.Remove(name);

            RegisteredTypes.Add(name, t);
        }

        public abstract Region OnCreate(IRocketPlayer player, string name, string[] args);
        public abstract bool OnRedefine(IRocketPlayer player, string[] args);
        public abstract double GetDistance(SerializablePosition pos);

        public abstract bool IsInRegion(SerializablePosition p);

        public static ReadOnlyCollection<string> GetTypes() => new ReadOnlyCollection<string>(new List<string>(RegisteredTypes.Keys));
    }
}