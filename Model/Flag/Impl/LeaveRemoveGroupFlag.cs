using System.Collections.Generic;
using Rocket.Core;
using Rocket.Unturned.Player;

namespace RocketRegions.Model.Flag.Impl
{
    public class LeaveRemoveGroupFlag : StringFlag
    {
        public override string Description => "Removes the permission group when leaving the region";
        public override string Usage => "<permission group>";
        public override bool SupportsGroups => true;

        public override void UpdateState(List<UnturnedPlayer> players)
        {

        }

        public override void OnRegionEnter(UnturnedPlayer player)
        {

        }

        public override void OnRegionLeave(UnturnedPlayer player)
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
            R.Permissions.RemovePlayerFromGroup(rocketGroup.Id, player);
        }
    }
}