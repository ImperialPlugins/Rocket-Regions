using System.Collections.Generic;
using System.Linq;
using Rocket.Unturned.Player;
using Safezone.Util;

namespace Safezone.Model.Flag.Impl
{
    public class EnterVehiclesFlag : BoolFlag
    {
        public override string Description => "Allows entering vehicles in a safezone";

        public override object DefaultValue => true;

        private readonly Dictionary<uint, bool> _lastVehicleStates = new Dictionary<uint, bool>();

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
                    GetValue<bool>(SafeZone.GetGroup(player))) continue;
                byte seat = 0;
                foreach (var p in player.Player.Movement.getVehicle().passengers.TakeWhile(p => PlayerUtil.GetId(p?.player) != id))
                {
                    seat++;
                }
                veh.kickPlayer(seat);
            }
        }

        public override void OnSafeZoneEnter(UnturnedPlayer p)
        {
            //do nothing
        }

        public override void OnSafeZoneLeave(UnturnedPlayer p)
        {
            //do nothing
        }
    }
}