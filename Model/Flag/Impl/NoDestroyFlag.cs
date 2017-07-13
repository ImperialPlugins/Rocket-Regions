using System;
using System.Collections.Generic;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RocketRegions.Model.Flag.Impl
{
    public class NoDestroyFlag : BoolFlag
    {
        public override string Description => "Allow/Disallow destruction of structures";
        public override bool SupportsGroups => false;

        public override void UpdateState(List<UnturnedPlayer> players)
        {

            ushort maxHealth = ushort.MaxValue;
            if (StructureManager.regions != null)
            {
                for (var i0 = 0; i0 < StructureManager.regions.GetLength(0); i0++)
                for (var i1 = 0; i1 < StructureManager.regions.GetLength(1); i1++)
                {
                    var region = StructureManager.regions[i0, i1];
                    if (region == null) continue;
                    for (var i = 0; i < region.structures.Count; i++)
                    {
                        var structure = region.structures[i];
                        if (structure?.structure == null) continue;
                        if (!Region.Type.IsInRegion(new SerializablePosition(structure.point))) continue;

                        var asset = (ItemStructureAsset) Assets.find(EAssetType.ITEM, structure.structure.id);
                        if (asset == null) continue;

                        if (GetValueSafe() && structure.structure.health < maxHealth)
                        {
                            structure.structure.health = maxHealth;
                        }
                        else if (!GetValueSafe() && structure.structure.health > asset.health)
                        {
                            structure.structure.health = asset.health;
                        }
                    }
                }
            }

            if (BarricadeManager.regions == null) return;
            {
                for (var i0 = 0; i0 < BarricadeManager.regions.GetLength(0); i0++)
                for (var i1 = 0; i1 < BarricadeManager.regions.GetLength(1); i1++)
                {
                    var region = BarricadeManager.regions[i0, i1];
                    if (region == null) continue;
                    for (var i = 0; i < region.barricades.Count; i++)
                    {
                        var barricade = region.barricades[i];
                        if (barricade?.barricade == null) continue;
                        if (!Region.Type.IsInRegion(new SerializablePosition(barricade.point))) continue;

                        var asset = (ItemBarricadeAsset) Assets.find(EAssetType.ITEM, barricade.barricade.id);
                        if (asset == null) continue;

                        if (asset.build == EBuild.FARM)
                            continue;

                        if (GetValueSafe() && barricade.barricade.health < maxHealth)
                        {
                            barricade.barricade.health = maxHealth;
                        }
                        else if (!GetValueSafe() && barricade.barricade.health > asset.health)
                        {
                            barricade.barricade.health = asset.health;
                        }
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