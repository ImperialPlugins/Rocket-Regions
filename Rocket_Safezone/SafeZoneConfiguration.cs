using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Rocket.API;

namespace Rocket_Safezone
{
    public class SafeZoneConfiguration : IRocketPluginConfiguration
    {
        [XmlArrayItem(ElementName = "SafeZones")]
        public List<SafeZone> SafeZones;

        public IRocketPluginConfiguration DefaultConfiguration
        {
            get
            {
                SafeZoneConfiguration config = new SafeZoneConfiguration();
                config.SafeZones = new List<SafeZone>();
                return config;
            }
        }
    }

    public class SafeZone
    {
        [XmlAttribute("Name")]
        public string Name;
        [XmlAttribute("Active")]
        public bool Active;
        [XmlElement("Position1")]
        public Position Position1;
        [XmlElement("Position2")]
        public Position Position2;

        //Todo: implement these
        [XmlAttribute("PickupAllowed")]
        public bool PickupAllowed;
        [XmlAttribute("VehiclesAllowed")]
        public bool VehiclesAllowed;
    }

    [Serializable]
    [XmlTypeAttribute(AnonymousType = true)]
    public class Position
    {
        [XmlAttribute("x")]
        public float X;
        [XmlAttribute("y")]
        public float Y;
    }
}
