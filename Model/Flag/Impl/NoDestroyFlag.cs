using System.Collections.Generic;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RocketRegions.Model.Flag.Impl
{
    public class NoDestroyFlag : BoolFlag
    {
        public override string Description => "Allow/Disallow destruction of structures";
        public override bool SupportsGroups => false;

        public override void UpdateState(List<UnturnedPlayer> players)
        {

        }

        public override void OnRegionEnter(UnturnedPlayer player)
        {
     
        }

        public override void OnRegionLeave(UnturnedPlayer player)
        {
          
        }
    }
}