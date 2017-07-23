using System.Collections.Generic;
using Rocket.Core;
using Rocket.Unturned.Player;

namespace RocketRegions.Model.Flag.Impl
{
    public class EnterAddGroupFlag : StringFlag
    {
        public override string Description => "Sets the permission group when entering the region";
        public override string Usage => "<permission group>";
        public override bool SupportsGroups => true;

        public override void UpdateState(List<UnturnedPlayer> players)
        {
            
        }

        public override void OnRegionEnter(UnturnedPlayer player)
        {
            var group = Region.GetGroup(player);
            string pgroup = GetValue<string>(group);
            if (pgroup == null)
            {
                return;
            }

            var rocketGroup = R.Permissions.GetGroup(pgroup);
            if (rocketGroup == null)
                return;
            R.Permissions.AddPlayerToGroup(rocketGroup.Id, player);
        }

        public override void OnRegionLeave(UnturnedPlayer player)
        {
            
        }
    }
}