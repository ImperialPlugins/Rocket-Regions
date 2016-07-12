using System.Collections.Generic;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace Safezone.Model.Flag.Impl
{
    public class NoDestroy : BoolFlag
    {
        public override string Description => "Allow/Disallow destruction of structures in the given safezone";
        public override bool SupportsGroups => false;

        public override void UpdateState(List<UnturnedPlayer> players)
        {

            ushort maxHealth = ushort.MaxValue - 1;
            if (StructureManager.StructureRegions != null)
            {
                foreach (var region in StructureManager.StructureRegions)
                {
                    if (region == null) continue;
                    foreach (var structure in region.structures)
                    {
                        if (structure?.structure == null) continue;
                        if (!SafeZone.Type.IsInSafeZone(new SerializablePosition(structure.point))) continue;

                        var asset = (ItemStructureAsset) Assets.find(EAssetType.ITEM, structure.structure.id);
                        if (asset == null) continue;

                        if ((Value != null && (bool) Value) && structure.structure.health < maxHealth)
                        {
                            structure.structure.health = maxHealth;
                        }
                        else if (structure.structure.health > asset.health)
                        {
                            structure.structure.health = asset.health;
                        }
                    }
                }
            }

            if (BarricadeManager.BarricadeRegions!= null)
            {
                foreach (var region in BarricadeManager.BarricadeRegions)
                {
                    if (region == null) continue;
                    foreach (var barricade in region.barricades)
                    {
                        if (barricade?.barricade== null) continue;
                        if (!SafeZone.Type.IsInSafeZone(new SerializablePosition(barricade.point))) continue;

                        var asset = (ItemBarricadeAsset)Assets.find(EAssetType.ITEM, barricade.barricade.id);
                        if (asset == null) continue;

                        if ((Value != null && (bool)Value) && barricade.barricade.health < maxHealth)
                        {
                            barricade.barricade.health = maxHealth;
                        }
                        else if (barricade.barricade.health > asset.health)
                        {
                            barricade.barricade.health = asset.health;
                        }
                    }
                }
            }
        }

        public override void OnSafeZoneEnter(UnturnedPlayer player)
        {
     
        }

        public override void OnSafeZoneLeave(UnturnedPlayer player)
        {
          
        }
    }
}