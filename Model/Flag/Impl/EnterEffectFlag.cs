using System.Collections.Generic;
using Rocket.Unturned.Player;

namespace RocketRegions.Model.Flag.Impl
{
    public class EnterEffectFlag : IDFlag
    {
        public override string Description => "Play effect when entering the region";
        public override void UpdateState(List<UnturnedPlayer> players)
        {

        }

        public override void OnRegionEnter(UnturnedPlayer player)
        {
            if (Value == null)
                return;
            var val = GetValue<ushort>(Region.GetGroup(player));
            player.TriggerEffect(val);
        }

        public override void OnRegionLeave(UnturnedPlayer player)
        {
           
        }
    }
}