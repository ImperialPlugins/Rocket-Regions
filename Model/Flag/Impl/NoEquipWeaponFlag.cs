using System.Collections.Generic;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RocketRegions.Model.Flag.Impl
{
    public class NoEquipWeaponFlag : BoolFlag
    {
        public override string Description => "Allow/Disallow equipping weapons";

        public override void UpdateState(List<UnturnedPlayer> players)
        {
            foreach (var player in players)
            {
                if(player?.Player?.Equipment?.useable == null)
                    continue;

                if(!GetValueSafe(Region.GetGroup(player)))
                    continue;

                if (!player.Player.Equipment.isEquipped) continue;
                if (player.Player.Equipment.useable is UseableGun ||
                    player.Player.Equipment.useable is UseableMelee
                    || player.Player.Equipment.useable is UseableThrowable)
                    player.Player.Equipment.dequip();
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