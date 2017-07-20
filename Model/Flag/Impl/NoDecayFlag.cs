using System;
using System.Collections.Generic;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RocketRegions.Model.Flag.Impl
{
    public class NoDecayFlag : BoolFlag
    {
        private DateTime _lastRun = DateTime.Now;
        public override string Description => "Prevents decay of barricades";
        public override void UpdateState(List<UnturnedPlayer> players)
        {
            if ((DateTime.Now - _lastRun).TotalSeconds < 60)
                return;

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

                        structure.objActiveDate = Provider.time;

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

                        barricade.objActiveDate = Provider.time;
                    }
                }
            }

            _lastRun = DateTime.Now;
        }

        public override void OnRegionEnter(UnturnedPlayer player)
        {
            
        }

        public override void OnRegionLeave(UnturnedPlayer player)
        {
            
        }
    }
}