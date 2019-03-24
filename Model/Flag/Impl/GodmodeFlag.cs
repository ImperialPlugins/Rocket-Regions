using System.Collections.Generic;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace RocketRegions.Model.Flag.Impl
{
    public class GodmodeFlag : BoolFlag
    {
        public override string Description => "Gives players godmode";
        private readonly Dictionary<ulong, byte> _lastHealth = new Dictionary<ulong, byte>();
        private readonly Dictionary<ulong, bool> _isBleed = new Dictionary<ulong, bool>();

        public override void UpdateState(List<UnturnedPlayer> players)
        {
            //do nothing
        }

        public override void OnRegionEnter(UnturnedPlayer player)
        {
            if(!_lastHealth.ContainsKey(player.CSteamID.m_SteamID))
                _lastHealth.Add(player.CSteamID.m_SteamID, player.Health);
            if (player.Bleeding && !_isBleed.ContainsKey(player.CSteamID.m_SteamID))
            {
                _isBleed.Add(player.CSteamID.m_SteamID, true);
                player.Bleeding = false;
            }
            if (!GetValueSafe(Region.GetGroup(player))) return;
            player.Features.GodMode = true;
        }

        public override void OnRegionLeave(UnturnedPlayer player)
        {
            int health = player.Health;
            if (_lastHealth.ContainsKey(player.CSteamID.m_SteamID))
            {
                health = _lastHealth[player.CSteamID.m_SteamID];
                _lastHealth.Remove(player.CSteamID.m_SteamID);
            }

            player.Features.GodMode = false;

            if (!GetValueSafe(Region.GetGroup(player))) return;

            if (_isBleed.ContainsKey(player.CSteamID.m_SteamID))
            {
                _isBleed.Remove(player.CSteamID.m_SteamID);
                player.Bleeding = true;
            }
            else player.Bleeding = false; // Just incase they're bleed while they're coming out of region.

            var currentHealth = player.Health;
            if (currentHealth < health)
                player.Heal((byte)(health - currentHealth));
            else
                player.Damage((byte)(currentHealth - health), Vector3.zero, EDeathCause.KILL, ELimb.SPINE, CSteamID.Nil);
        }
    }
}