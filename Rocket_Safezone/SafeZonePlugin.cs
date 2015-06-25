using System;
using System.Collections.Generic;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Logging;
using Rocket.Unturned.Player;
using Rocket.Unturned.Plugins;
using UnityEngine;

namespace Rocket_Safezone
{
    public class SafeZonePlugin : RocketPlugin<SafeZoneConfiguration>
    {
        public static SafeZonePlugin Instance;
        private readonly Dictionary<RocketPlayer, SafeZone> _safeZonePlayers = new Dictionary<RocketPlayer, SafeZone>();
        private readonly Dictionary<RocketPlayer, Boolean> _godModeStates = new Dictionary<RocketPlayer, bool>(); 

        protected override void Load()
        {
            Instance = this;
            RocketPlayerEvents.OnPlayerUpdatePosition += OnPlayerUpdatePosition;
            RocketServerEvents.OnPlayerConnected += OnPlayerConnect;
            RocketServerEvents.OnPlayerDisconnected += OnPlayerDisconnect;
            //Todo: loop all players and check if they are in safezones (for the case that this plugin was loaded after the start)
        }

        protected override void Unload()
        {
            foreach (RocketPlayer player in _safeZonePlayers.Keys)
            {
                OnPlayerLeftSafeZone(player, _safeZonePlayers[player], false);
            }
            Instance = null;
        }

        private void OnPlayerConnect(RocketPlayer player)
        {
            foreach (SafeZone zone in Configuration.SafeZones)
            {
                if (ContainsPosition(zone, player.Position))
                {
                    OnPlayerEnteredSafeZone(player, zone, true);        
                    return;
                }
            }
        }

        private void OnPlayerDisconnect(RocketPlayer player)
        {
            if (!_safeZonePlayers.ContainsKey(player)) return;
            OnPlayerLeftSafeZone(player, _safeZonePlayers[player], false);
        }

        private void OnPlayerUpdatePosition(RocketPlayer player, Vector3 position)
        {
            bool bIsInSafeZone = false;
            SafeZone safeZone = null;
            foreach (SafeZone zone in Configuration.SafeZones)
            {
                if (ContainsPosition(zone, position))
                {
                    bIsInSafeZone = true;
                    safeZone = zone;
                    break;
                }
            }

            if (!bIsInSafeZone && _safeZonePlayers.ContainsKey(player))
            {
                OnPlayerLeftSafeZone(player, _safeZonePlayers[player], true);
            }
            else if (bIsInSafeZone && !_safeZonePlayers.ContainsKey(player))
            {
                OnPlayerEnteredSafeZone(player, safeZone, true);
                return;
            }

            //Player is still inside or outside a safezone, don't update anything
        }

        private void OnPlayerEnteredSafeZone(RocketPlayer player, SafeZone safeZone, bool bSendMessage)
        {
            if(safeZone == null) throw new ArgumentNullException("safeZone");
            EnableGodMode(player);
            _safeZonePlayers.Add(player, safeZone);

            if (bSendMessage)
            {
                //Todo: use translation
                RocketChat.Say(player.CSteamID, "Entered safe zone: " + safeZone.Name, Color.green);
            }
        }

        private void OnPlayerLeftSafeZone(RocketPlayer player, SafeZone safeZone, bool bSendMessage)
        {
            DisableGodMode(player);
            _safeZonePlayers.Remove(player);

            if (bSendMessage)
            {
                //Todo: use translation
                RocketChat.Say(player.CSteamID, "Left safe zone: " + safeZone.Name, Color.red);
            }
        }

        private void EnableGodMode(RocketPlayer player)
        {
            //Safe current godmode state and restore it later when the player leaves the safezone
            //this is for e.g. players who enter with /god safezones
            _godModeStates.Add(player, player.Features.GodMode);
            player.Features.GodMode = true;
        }

        private void DisableGodMode(RocketPlayer player)
        {
            try
            {
                //Try to restore previous godmode state
                player.Features.GodMode = _godModeStates[player];
            }
            catch (Exception ex)
            {
                //Something went wrong??
                Logger.LogException(ex);
                player.Features.GodMode = false;
            }
            _godModeStates.Remove(player);
        }

        private bool ContainsPosition(SafeZone zone, Vector3 playerPosition)
        {
            Position p = new Position() {X = playerPosition.x, Y = playerPosition.z};
            
            Position p1 = zone.Position1;
            Position p4 = zone.Position2;
            Position p2 = new Position() {X = p4.X, Y = p1.Y};
            Position p3 = new Position() {X = p1.X, Y = p4.Y };

            return IsPointInPolygon(new Position[] {p1, p2, p3, p4}, p);
        }

        private bool IsPointInPolygon(Position[] polygon, Position point)
        {
            bool isInside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if (((polygon[i].Y > point.Y) != (polygon[j].Y > point.Y)) &&
                (point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                {
                    isInside = !isInside;
                }
            }
            return isInside;
        }
    }
}