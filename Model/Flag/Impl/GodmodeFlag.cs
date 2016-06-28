using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using Safezone.Model.Safezone;
using Safezone.Util;

namespace Safezone.Model.Flag.Impl
{
    public class GodmodeFlag : BoolFlag
    {
        private readonly Dictionary<uint, bool> _godModeStates = new Dictionary<uint, bool>();
        public override string Description => "Gives players in safezone godmode";

        public override object DefaultValue => true;
        public override void UpdateState(List<UnturnedPlayer> players)
        {
            //do nothing
        }

        public override void OnSafeZoneEnter(UnturnedPlayer player)
        {
            if (!GetValue<bool>(SafeZone.GetGroup(player))) return;
            EnableGodMode(player);
        }

        public override void OnSafeZoneLeave(UnturnedPlayer player)
        {
            if (!GetValue<bool>(SafeZone.GetGroup(player))) return;
            DisableGodMode(player);
        }

        private void EnableGodMode(IRocketPlayer player)
        {
            if (!(player is UnturnedPlayer))
            {
                throw new NotSupportedException();
            }
            var id = PlayerUtil.GetId(player);
            var unturnedPlayer = (UnturnedPlayer)player;
            //Safe current godmode state and restore it later when the player leaves the safezone
            //this is for e.g. players who enter with /god safezones
            _godModeStates.Add(id, unturnedPlayer.Features.GodMode);
            unturnedPlayer.Features.GodMode = true;
        }

        private void DisableGodMode(IRocketPlayer player)
        {
            if (!(player is UnturnedPlayer))
            {
                throw new NotSupportedException();
            }
            var id = PlayerUtil.GetId(player);

            var unturnedPlayer = (UnturnedPlayer)player;
            try
            {
                //Try to restore previous godmode state
                unturnedPlayer.Features.GodMode = _godModeStates[id];
            }
            catch (Exception ex)
            {
                //Something went wrong??
                Logger.LogException(ex);
                unturnedPlayer.Features.GodMode = false;
            }
            _godModeStates.Remove(id);
        }
    }
}