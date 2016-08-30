using System.Collections.Generic;
using Rocket.Unturned.Player;
using RocketRegions.Util;

namespace RocketRegions.Model.Flag.Impl
{
    public class LeaveURLFlag : StringFlag
    {
        public override string Description => "Open URL when leaving the region";
        public override string Usage => "<url>";
        public override void UpdateState(List<UnturnedPlayer> players)
        {

        }

        public override void OnRegionEnter(UnturnedPlayer player)
        {

        }

        public override void OnRegionLeave(UnturnedPlayer player)
        {
            var group = Region.GetGroup(player);
            var val = GetValue<string>(group);
            if (val == null)
                return;
            string msg = RegionsPlugin.Instance.Configuration.Instance.UrlOpenMessage;
            PlayerUtil.OpenUrl(player, msg, val);
        }
    }
}