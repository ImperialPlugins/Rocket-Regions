using System.Collections.Generic;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;

namespace Safezone.Model.Flag.Impl
{
    public class EnterMessageFlag : StringFlag
    {
        public override string Usage => "<message>";
        public override string Description => "Message shown when entering a safezone";

        public override void UpdateState(List<UnturnedPlayer> players)
        {
    
        }

        public override void OnSafeZoneEnter(UnturnedPlayer player)
        {
            if (Value == null) return;
            var parsedValue = (string) Value;
            parsedValue = parsedValue.Replace("{0}", SafeZone.Name);
            UnturnedChat.Say(player, parsedValue);
        }

        public override void OnSafeZoneLeave(UnturnedPlayer player)
        {
 
        }
    }
}