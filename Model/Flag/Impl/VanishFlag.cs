using System.Collections.Generic;
using Rocket.Unturned.Player;

namespace RocketRegions.Model.Flag.Impl
{
    public class VanishFlag : BoolFlag
    {
        public override string Description => "Sets vanish for any player who enters region";

        public override void UpdateState(List<UnturnedPlayer> players)
        {
            //do nothing
        }

        public override void OnRegionEnter(UnturnedPlayer player)
        {
            if (!GetValueSafe(Region.GetGroup(player))) return;
            player.Features.VanishMode = true;
        }

        public override void OnRegionLeave(UnturnedPlayer player)
        {
            if (!GetValueSafe(Region.GetGroup(player))) return;
            player.Features.VanishMode = false;
        }
    }
}