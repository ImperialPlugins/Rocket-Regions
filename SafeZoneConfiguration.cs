using System.Collections.Generic;
using Rocket.API;
using Safezone.Model.Safezone;

namespace Safezone
{
    public class SafeZoneConfiguration : IRocketPluginConfiguration
    {
        public int UpdateFrameCount;
        public List<SafeZone> SafeZones;

        public void LoadDefaults()
        {
            SafeZones = new List<SafeZone>();
            UpdateFrameCount = 1;
        }
    }
}
