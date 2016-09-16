using System.Collections.Generic;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;

namespace RocketRegions.Model.Flag.Impl
{
    public class EnterMessageFlag : StringFlag
    {
        public override string Usage => "<message>";
        public override string Description => "Message shown when entering the region";

        public override void UpdateState(List<UnturnedPlayer> players)
        {
    
        }

        public override void OnRegionEnter(UnturnedPlayer player)
        {
            var parsedValue = GetValue<string>(Region.GetGroup(player));
            if (parsedValue == null)
                return;
            parsedValue = parsedValue.Replace("{0}", Region.Name);
            UnturnedChat.Say(player, parsedValue);
        }

        public override void OnRegionLeave(UnturnedPlayer player)
        {
 
        }
    }
}