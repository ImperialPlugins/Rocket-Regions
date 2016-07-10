using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using Safezone.Util;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace Safezone.Model.Flag.Impl
{
    public class GodmodeFlag : BoolFlag
    {
        private readonly Dictionary<ulong, bool> _godModeStates = new Dictionary<ulong, bool>();
        public override string Description => "Gives players in safezone godmode";
        private readonly Dictionary<ulong, byte> _lastHealth = new Dictionary<ulong, byte>();

        public override void UpdateState(List<UnturnedPlayer> players)
        {
            //do nothing
        }

        public override void OnSafeZoneEnter(UnturnedPlayer player)
        {
            if (!_lastHealth.ContainsKey(player.CSteamID.m_SteamID))
            {
                _lastHealth.Add(player.CSteamID.m_SteamID, player.Health);
            }
            else
            {
                _lastHealth[player.CSteamID.m_SteamID] = player.Health;
            }
            if (!GetValue<bool>(SafeZone.GetGroup(player))) return;
            EnableGodMode(player);
        }

        public override void OnSafeZoneLeave(UnturnedPlayer player)
        {
            if (!GetValue<bool>(SafeZone.GetGroup(player))) return;
            DisableGodMode(player);

            if (!_lastHealth.ContainsKey(player.CSteamID.m_SteamID)) return;
            var health = _lastHealth[player.CSteamID.m_SteamID];
            var currentHealth = player.Health;
            if (currentHealth < health)
            {
                player.Heal((byte)(health - currentHealth));
            }
            else
            {
                player.Damage((byte)(currentHealth - health), Vector3.zero, EDeathCause.KILL, ELimb.SPINE, CSteamID.Nil);
            }
            _lastHealth.Remove(player.CSteamID.m_SteamID);
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
            if (_godModeStates.ContainsKey(id)) _godModeStates.Remove(id);
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
                var val = _godModeStates.ContainsKey(id) && _godModeStates[id];
                unturnedPlayer.Features.GodMode = val;
            }
            catch (Exception ex)
            {
                //Something went wrong??
                Logger.LogException(ex);
                unturnedPlayer.Features.GodMode = false;
            }

            if(_godModeStates.ContainsKey(id))
                _godModeStates.Remove(id);
        }
    }
}