using System.Collections.Generic;
using Rocket.Unturned.Player;
using RocketRegions.Util;
using SDG.Unturned;
using UnityEngine;

namespace RocketRegions.Model.Flag.Impl
{
    public class NoVehiclesLockpickFlag : BoolFlag
    {
        public override string Description => "Allow/Disallow lockpicking in region";
        public override bool SupportsGroups => true;

        private readonly Dictionary<ulong, bool> _lastVehicleStates = new Dictionary<ulong, bool>();

        public override void UpdateState(List<UnturnedPlayer> players)
        {
            //do nothing
        }

        public override void OnRegionEnter(UnturnedPlayer p)
        {
            //do nothing
        }

        public override void OnRegionLeave(UnturnedPlayer p)
        {
            //do nothing
        }
    }
}
