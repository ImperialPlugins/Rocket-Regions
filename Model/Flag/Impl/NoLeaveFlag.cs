using System.Collections.Generic;
using Rocket.Unturned.Player;
using Steamworks;

namespace Safezone.Model.Flag.Impl
{
    public class NoLeaveFlag : BoolFlag
    {
        // code is handled in SafeZonePlugin.cs because it needs to hook before these events

        public override string Description => "Allow/Disallow leaving the given safezone after entering it";

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