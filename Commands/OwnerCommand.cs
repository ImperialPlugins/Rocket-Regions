using System.Collections.Generic;
using RocketRegions.Model;

namespace RocketRegions.Commands
{
    public class OwnerCommand : ListHandleCommand
    {
        protected override void Remove(Region region, ulong mSteamID)
            => region.RemoveOwner(mSteamID);

        protected override void Add(Region region, ulong mSteamID)
            => region.AddOwner(mSteamID);

        protected override List<ulong> GetList(Region region)
            => region.GetOwners();

        #region Properties
        
        public override List<string> Aliases => new List<string> { "regionsowner" };
        
        public override List<string> Permissions => new List<string>() { "regions.command.owner" };
        
        public override string Name => "rowner";
        
        public override string Help => "Add or remove owners from a region";
        
        #endregion
    }
}