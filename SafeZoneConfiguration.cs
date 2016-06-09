using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Rocket.API;
using Rocket.Core.Assets;
using Safezone.Model.Safezone;

namespace Safezone
{
    [Serializable]
    public class SafeZoneConfiguration : IRocketPluginConfiguration
    {
        [XmlArrayItem(ElementName = "SafeZones")]
        public List<SafeZone> SafeZones = new List<SafeZone>();

        public void LoadDefaults()
        {
            SafeZones = new List<SafeZone>();
            ZombieTimerSpeed = 5;
        }

        [XmlArrayItem(ElementName = "ZombieTimerSpeed")] 
        public int ZombieTimerSpeed = 5;

    }
}
