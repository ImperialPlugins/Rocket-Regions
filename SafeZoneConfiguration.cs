using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Rocket.API;
using Safezone.Model.Safezone;

namespace Safezone
{
    public class SafeZoneConfiguration : IRocketPluginConfiguration
    {
        public List<SafeZone> SafeZones;

        public void LoadDefaults()
        {
            SafeZones = new List<SafeZone>();
            ZombieTimerSpeed = 1;
        }

        public int ZombieTimerSpeed;
    }
}
