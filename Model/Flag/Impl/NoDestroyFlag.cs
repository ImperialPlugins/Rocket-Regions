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
                foreach (var region in StructureManager.regions)
                {
                    if (region == null) continue;
                    foreach (var structure in region.structures)
                    {
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

            if (BarricadeManager.regions!= null)
            {
                foreach (var region in BarricadeManager.regions)
                {
                    if (region == null) continue;
                    foreach (var barricade in region.barricades)
                    {
                        if (barricade?.barricade== null) continue;
                        if (!Region.Type.IsInRegion(new SerializablePosition(barricade.point))) continue;

                        var asset = (ItemBarricadeAsset)Assets.find(EAssetType.ITEM, barricade.barricade.id);
                        if (asset == null) continue;

                        if(asset.build == EBuild.FARM)
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