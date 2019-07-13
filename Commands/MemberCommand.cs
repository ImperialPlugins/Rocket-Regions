using System.Collections.Generic;
using Rocket.API;
using RocketRegions.Model;

namespace RocketRegions.Commands
{
    public class MemberCommand : ListHandleCommand
    {
        protected override void Remove(Region region, ulong mSteamID)
            => region.RemoveMember(mSteamID);

        protected override void Add(Region region, ulong mSteamID)
            => region.AddMember(mSteamID);

        protected override List<ulong> GetList(Region region)
            => region.GetMembers();
        
        #region Properties

        public override AllowedCaller AllowedCaller => AllowedCaller.Both;

        public override List<string> Aliases => new List<string> { "regionsmember" };
        
        public override List<string> Permissions => new List<string>() { "regions.command.member" };
        
        public override string Name => "rmember";
        
        public override string Help => "Add or remove members from a region";
        
        #endregion
    }
}