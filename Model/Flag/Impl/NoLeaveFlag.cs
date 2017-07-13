using System.Collections.Generic;
using Rocket.Unturned.Player;

namespace RocketRegions.Model.Flag.Impl
{
    public class NoLeaveFlag : BoolFlag
    {
        // code is handled in RegionsPlugin.cs because it needs to hook before these events

        public override string Description => "Allow/Disallow leaving the region after entering it";

        public override void UpdateState(List<UnturnedPlayer> players)
        {
            //do nothing
        }

        public override void OnRegionEnter(UnturnedPlayer player)
        {
            //do nothing
        }

        public override void OnRegionLeave(UnturnedPlayer player)
        {
            //do nothing
        }
    }
}