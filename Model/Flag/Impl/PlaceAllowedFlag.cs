using System.Collections.Generic;
using System.Linq;
using Rocket.Unturned.Player;
using Safezone.Util;
using SDG.Unturned;
using Steamworks;

namespace Safezone.Model.Flag.Impl
{
    public class PlaceAllowedFlag : BoolFlag
    {
        public override string Description => "Allows placing of structures";

        public override object DefaultValue => true;
        public override void UpdateState(List<UnturnedPlayer> players)
        {
            foreach (var p in from p in players
                              where PlayerUtil.GetCSteamId(p) != CSteamID.Nil
                              let playerGroup = SafeZone.GetGroup(p)
                              let equippedItem = p.Player.Equipment.useable as UseableBarricade
                              where equippedItem != null && !SafeZone.GetFlag(typeof (PlaceAllowedFlag)).GetValue<bool>(playerGroup)
                              select p)
            {
                p.Player.equipment.dequip();
            }
        }

        public override void OnSafeZoneEnter(UnturnedPlayer player)
        {
            //do nothing
        }

        public override void OnSafeZoneLeave(UnturnedPlayer player)
        {
            //do nothing
        }
    }
}