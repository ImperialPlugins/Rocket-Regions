using System.Collections.Generic;
using Rocket.Unturned.Player;

namespace RocketRegions.Model.Flag.Impl
{
    public class NoEquipFlag : BoolFlag
    {
        public override string Description => "Allow/Disallow equipping items";

        public override void UpdateState(List<UnturnedPlayer> players)
        {
            for (var i = 0; i < players.Count; i++)
            {
                var player = players[i];
                if (player?.Player?.equipment?.useable == null)
                    continue;
                if (!player.Player.equipment.isEquipped)
                    continue;
                if (!GetValueSafe(Region.GetGroup(player)))
                    continue;
                player.Player.equipment.dequip();
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