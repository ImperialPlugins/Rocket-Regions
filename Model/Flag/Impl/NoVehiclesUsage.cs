using System.Collections.Generic;
using Rocket.Unturned.Player;
using RocketRegions.Util;
using SDG.Unturned;

namespace RocketRegions.Model.Flag.Impl
{
    public class NoVehiclesUsage : BoolFlag
    {
        public override string Description => "Allow/Disallow usage of vehicles";

        private readonly Dictionary<ulong, bool> _lastVehicleStates = new Dictionary<ulong, bool>();

        public override void UpdateState(List<UnturnedPlayer> players)
        {
            foreach (var player in players)
            {
                var id = PlayerUtil.GetId(player);
                var veh = player.Player.Movement.getVehicle();
                var isInVeh = veh != null;

                if (!_lastVehicleStates.ContainsKey(id))
                {
                    _lastVehicleStates.Add(id, veh);
                }

                var wasDriving = _lastVehicleStates[id];

                if (!isInVeh || wasDriving ||
                    !GetValue<bool>(Region.GetGroup(player))) continue;
                sbyte index = -1;
                foreach (Passenger p in veh.passengers)
                {
                    index++;
                    if (p.player.SteamPlayerID.CSteamID == PlayerUtil.GetCSteamId(player))
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