using System;
using System.Collections.Generic;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RocketRegions.Model.Flag.Impl
{
    public class DecayFlag : IntFlag
    {
        public override string Description => "Decreases health of barricades and structures by 10 every x milliseconds";
        public override string Usage => "<interval in seconds>";
        public override bool SupportsGroups => false;

        private DateTime _lastRun = DateTime.Now;

        public override void UpdateState(List<UnturnedPlayer> players)
        {
            if (GetValue<int>() == 0)
                return;

            if ((DateTime.Now - _lastRun).TotalSeconds < GetValue<int>())
            {
                return;
            }

            _lastRun = DateTime.Now;

            if (StructureManager.regions != null)
            {
                foreach (var region in StructureManager.regions)
                {
                    if (region == null) continue;
                    foreach (var structure in region.structures)
                    {
                        if (structure?.structure == null) continue;
                        if (!Region.Type.IsInRegion(new SerializablePosition(structure.point))) continue;

                        var asset = (ItemStructureAsset)Assets.find(EAssetType.ITEM, structure.structure.id);
                        if (asset == null) continue;

                        structure.structure.health -= 10;
                    }
                }
            }

            if (BarricadeManager.regions == null) return;
            {
                foreach (var region in BarricadeManager.regions)
                {
                    if (region == null) continue;
                    foreach (var barricade in region.barricades)
                    {
                        if (barricade?.barricade == null) continue;
                        if (!Region.Type.IsInRegion(new SerializablePosition(barricade.point))) continue;

                        var asset = (ItemBarricadeAsset)Assets.find(EAssetType.ITEM, barricade.barricade.id);
                        if (asset == null) continue;

                        barricade.barricade.health -= 10;
                    }
                }
            }
        }

        public override void OnRegionEnter(UnturnedPlayer player)
        {
            
        }

        public override void OnRegionLeave(UnturnedPlayer player)
        {
            
        }
    }
}