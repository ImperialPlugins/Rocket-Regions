using System.Collections.Generic;
using System.Xml.Serialization;
using Rocket.API;
using RocketRegions.Model;

namespace RocketRegions
{
    public class RegionsConfiguration : IRocketPluginConfiguration
    {
        public int UpdateFrameCount { get; set; }
        public List<Region> Regions { get; set; }
        public List<ushort> NoEquipWeaponIgnoredItems { get; set; }
        public string UrlOpenMessage { get; set; }
        public List<ushort> NoEquipIgnoredItems { get; set; }
        public List<ushort> NoDestroyIgnoredItems { get; set; }
        public string NoDestroyIgnorePermission { get; set; }
        public string NoVehicleDamageIgnorePermission { get; set; }

        public void LoadDefaults()
        {
            Regions = new List<Region>();
            UpdateFrameCount = 10;
            UrlOpenMessage = "Visit webpage";
            NoEquipWeaponIgnoredItems = new List<ushort> {1337};
            NoEquipIgnoredItems = new List<ushort> { 1337 };
            NoDestroyIgnoredItems = new List<ushort> { 369 };
            NoDestroyIgnorePermission = "regions.nodestroy.ignore";
            NoVehicleDamageIgnorePermission = "regions.novehicledamage.ignore";
        }
    }
}
