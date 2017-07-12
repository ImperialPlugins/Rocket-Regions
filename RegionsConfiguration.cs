using System.Collections.Generic;
using Rocket.API;
using RocketRegions.Model;

namespace RocketRegions
{
    public class RegionsConfiguration : IRocketPluginConfiguration
    {
        public int UpdateFrameCount;
        public List<Region> Regions;
        public string UrlOpenMessage;

        public void LoadDefaults()
        {
            Regions = new List<Region>();
            UpdateFrameCount = 10;
            UrlOpenMessage = "Visit webpage";
        }
    }
}
