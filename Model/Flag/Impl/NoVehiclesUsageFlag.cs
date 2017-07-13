using System.Collections.Generic;
using Rocket.Unturned.Player;
using RocketRegions.Util;
using SDG.Unturned;

namespace RocketRegions.Model.Flag.Impl
{
    public class NoVehiclesUsageFlag : BoolFlag
    {
        public override string Description => "Allow/Disallow usage of vehicles in region";

        private readonly Dictionary<ulong, bool> _lastVehicleStates = new Dictionary<ulong, bool>();

        public override void UpdateState(List<UnturnedPlayer> players)
        {
            for (int i = 0; i < players.Count; i++)
            {
                var player = players[i];
                var id = PlayerUtil.GetId(player);
                var veh = player.Player.movement.getVehicle();
                var isInVeh = veh != null;

                if (!_lastVehicleStates.ContainsKey(id))
                    _lastVehicleStates.Add(id, veh);

                var wasDriving = _lastVehicleStates[id];

                if (!isInVeh || wasDriving ||
                    !GetValueSafe(Region.GetGroup(player))) continue;
                sbyte index = -1;
                for (var i1 = 0; i1 < veh.passengers.Length; i1++)
                {
                    Passenger p = veh.passengers[i1];
                    index++;
                    if (p.player.playerID.steamID == PlayerUtil.GetCSteamId(player))
                    {
                        break;
                    }
                }

                veh.kickPlayer((byte) index);
            }
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