using System.Collections.Generic;
using Rocket.Unturned.Player;
using Steamworks;

namespace RocketRegions.Model.Flag.Impl
{
    public class NoEnterFlag : BoolFlag
    {
        // code is handled in SafeZonePlugin.cs because it needs to hook before these events

        public override string Description => "Allow/Disallow entering the region";

        public override void UpdateState(List<UnturnedPlayer> players)
        {
            //do nothing
        }

        public override void OnSafeZoneEnter(UnturnedPlayer player)
        {
            //do nothing
        }

        public override void OnSafeZoneLeave(UnturnedPlayer player)
        {
            //do nothing
        }
    }
}