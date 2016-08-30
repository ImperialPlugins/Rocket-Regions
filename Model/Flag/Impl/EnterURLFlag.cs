using System.Collections.Generic;
using Rocket.Unturned.Player;
using RocketRegions.Util;

namespace RocketRegions.Model.Flag.Impl
{
    public class EnterURLFlag : StringFlag
    {
        public override string Description => "Open URL when entering the region";
        public override string Usage => "<url>";
        public override void UpdateState(List<UnturnedPlayer> players)
        {

        }

        public override void OnRegionEnter(UnturnedPlayer player)
        {
            var group = Region.GetGroup(player);
            var val = GetValue<string>(group);
            if(val == null)
                return;
            string msg = RegionsPlugin.Instance.Configuration.Instance.UrlOpenMessage;
            PlayerUtil.OpenUrl(player, msg, val);
        }

        public override void OnRegionLeave(UnturnedPlayer player)
        {
      
        }
    }
}