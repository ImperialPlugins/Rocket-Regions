using System.Collections.Generic;
using System.Xml.Serialization;
using Rocket.API;
using Safezone.Model;
using Safezone.Model.Safezone;

namespace Safezone
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
                config.ZombieTimerSpeed = 5;
                return config;
            }
        }

        [XmlArrayItem(ElementName = "ZombieTimerSpeed")] 
        public int ZombieTimerSpeed;
    }

 
}
