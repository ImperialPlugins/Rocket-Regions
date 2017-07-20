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
                if (player?.Player?.equipment?.useable == null)
                    continue;

                if (!GetValueSafe(Region.GetGroup(player)))
                    continue;

                if (!player.Player.equipment.isEquipped)
                    continue;

                if(RegionsPlugin.Instance.Configuration.Instance.NoEquipWeaponIgnoredItems
                    .Contains(player.Player.equipment.asset.id))
                    continue;

                if (player.Player.equipment.useable is UseableGun ||
                    player.Player.equipment.useable is UseableMelee
                    || player.Player.equipment.useable is UseableThrowable)
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