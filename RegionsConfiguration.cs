using System.Collections.Generic;
using Rocket.API;
using RocketRegions.Model.Safezone;

namespace RocketRegions
{
    public class RegionsConfiguration : IRocketPluginConfiguration
    {
        public int UpdateFrameCount;
        public List<Region> Regions;

        public void LoadDefaults()
        {
            Regions = new List<Region>();
            UpdateFrameCount = 1;
        }
    }
}
