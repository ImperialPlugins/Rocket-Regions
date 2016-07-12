using System.Collections.Generic;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;

namespace RocketRegions.Model.Flag.Impl
{
    public class LeaveMessageFlag : StringFlag
    {
        public override string Usage => "<message>";
        public override string Description => "Message shown when leaving the region";

        public override void UpdateState(List<UnturnedPlayer> players)
        {

        }

        public override void OnSafeZoneEnter(UnturnedPlayer player)
        {

        }

        public override void OnSafeZoneLeave(UnturnedPlayer player)
        {
            if (Value == null) return;
            var parsedValue = (string)Value;
            parsedValue = parsedValue.Replace("{0}", Region.Name);
            UnturnedChat.Say(player, parsedValue);
        }
    }
}