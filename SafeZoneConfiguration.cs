using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Rocket.API;
using Safezone.Model.Safezone;

namespace Safezone
{
    public class SafeZoneConfiguration : IRocketPluginConfiguration
    {
        public List<SafeZone> SafeZones = new List<SafeZone>();

        public void LoadDefaults()
        {
            SafeZones = new List<SafeZone>();
            ZombieTimerSpeed = 5;
        }

        public int ZombieTimerSpeed = 5;
    }
}
