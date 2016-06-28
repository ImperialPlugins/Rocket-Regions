using System.Collections.Generic;
using Rocket.Unturned.Player;
using Safezone.Model.Safezone;

namespace Safezone.Model.Flag.Impl
{
    public class NoLeaveFlag : BoolFlag
    {
        public override string Description => "Disallows leaving a safezone after entering it";

        public override object DefaultValue => false;
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