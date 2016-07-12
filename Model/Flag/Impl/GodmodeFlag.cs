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

        protected override void OnValueUpdate(object oldValue, object newValue)
        {
            base.OnValueUpdate(oldValue, newValue);

            if ((bool)newValue)
            {
                foreach (var player in Provider.clients)
                {
                    if (player?.player?.transform?.position == null) continue;
                    if (Region.Type.IsInSafeZone(new SerializablePosition(player.player.transform.position)))
                    {
                        OnSafeZoneEnter(UnturnedPlayer.FromSteamPlayer(player));
                    }
                }
                return;
            }

            foreach (var player in Provider.clients)
            {
                if (player?.player?.transform?.position == null) continue;
                if (Region.Type.IsInSafeZone(new SerializablePosition(player.player.transform.position)))
                {
                    OnSafeZoneLeave(UnturnedPlayer.FromSteamPlayer(player));
                }
            }
        }

        public override void UpdateState(List<UnturnedPlayer> players)
        {
            //do nothing
        }

        public override void OnSafeZoneEnter(UnturnedPlayer player)
        {
            if(!_lastHealth.ContainsKey(player.CSteamID.m_SteamID))
                _lastHealth.Add(player.CSteamID.m_SteamID, player.Health);
            if (!GetValue<bool>(Region.GetGroup(player))) return;
            player.Features.GodMode = true;
        }

        public override void OnSafeZoneLeave(UnturnedPlayer player)
        {
            int health = player.Health;
            if (_lastHealth.ContainsKey(player.CSteamID.m_SteamID))
            {
                health = _lastHealth[player.CSteamID.m_SteamID];
                _lastHealth.Remove(player.CSteamID.m_SteamID);
            }

            player.Features.GodMode = false;

            if (!GetValue<bool>(Region.GetGroup(player))) return;
            
            var currentHealth = player.Health;
            if (currentHealth < health)
            {
                player.Heal((byte)(health - currentHealth));
            }
            else
            {
                player.Damage((byte)(currentHealth - health), Vector3.zero, EDeathCause.KILL, ELimb.SPINE, CSteamID.Nil);
            }
        }
    }
}