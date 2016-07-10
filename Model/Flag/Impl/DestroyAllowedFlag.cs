using System.Collections.Generic;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace Safezone.Model.Flag.Impl
{
    public class DestroyAllowedFlag : BoolFlag
    {
        public override string Description => "Disallow destruction of structures";
        public override bool SupportsGroups => false;

        public override void UpdateState(List<UnturnedPlayer> players)
        {
            foreach (var region in StructureManager.StructureRegions)
            {
                if(region == null) continue;
                foreach (var structure in region.structures)
                {
                    if(structure?.structure == null) continue;
                    if(!SafeZone.Type.IsInSafeZone(new SerializablePosition(structure.point))) continue;

                    var asset = (ItemStructureAsset) Assets.find(EAssetType.ITEM, structure.structure.id);
                    if(asset == null) continue;

                    if (!(bool)Value)
                    {
                        structure.structure.health = ushort.MaxValue;
                    }
                    else if (structure.structure.health > asset.health)
                    {
                        structure.structure.health = asset.health;
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