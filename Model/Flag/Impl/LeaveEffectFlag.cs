using System.Collections.Generic;
using Rocket.Unturned.Player;

namespace RocketRegions.Model.Flag.Impl
{
    public class LeaveEffectFlag : IDFlag
    {
        public override string Description => "Play effect on leave";
        public override void UpdateState(List<UnturnedPlayer> players)
        {

        }

        public override void OnRegionEnter(UnturnedPlayer player)
        {

        }

        public override void OnRegionLeave(UnturnedPlayer player)
        {
            var val = GetValue<ushort?>();
            if (val == null)
                return;
            player.TriggerEffect(val.Value);
        }
    }
}