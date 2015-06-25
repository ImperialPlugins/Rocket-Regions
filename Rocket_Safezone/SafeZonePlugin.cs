using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Logging;
using Rocket.Unturned.Player;
using Rocket.Unturned.Plugins;
using Steamworks;
using UnityEngine;

namespace Rocket_Safezone
{
    public class SafeZonePlugin : RocketPlugin<SafeZoneConfiguration>
    {
        public static SafeZonePlugin Instance;
        private readonly Dictionary<uint, SafeZone> _safeZonePlayers = new Dictionary<uint, SafeZone>();
        private readonly Dictionary<uint, Boolean> _godModeStates = new Dictionary<uint, bool>(); 

        protected override void Load()
        {
            Instance = this;
            RocketPlayerEvents.OnPlayerUpdatePosition += OnPlayerUpdatePosition;
            //RocketPlayerEvents.OnPlayerUpdateGesture += OnPlayerUpdateGesture; //not possible currently
            RocketServerEvents.OnPlayerConnected += OnPlayerConnect;
            RocketServerEvents.OnPlayerDisconnected += OnPlayerDisconnect;
            //Todo: loop all players and check if they are in safezones (for the case that this plugin was loaded after the start)
        }

        private void OnPlayerUpdateGesture(RocketPlayer player, RocketPlayerEvents.PlayerGesture gesture)
        {
            if (!_safeZonePlayers.ContainsKey(GetId(player))) return;
            switch (gesture)
            {
                case RocketPlayerEvents.PlayerGesture.Pickup:
                    if (_safeZonePlayers[GetId(player)].PickupAllowed)
                    {
                        break;
                    }

                    //Todo cancel event (not possible currently?)
                    break;
            }
        }

        protected override void Unload()
        {
            foreach (uint id in _safeZonePlayers.Keys)
            {
                RocketPlayer player = RocketPlayer.FromCSteamID(new CSteamID(id));
                OnPlayerLeftSafeZone(player, _safeZonePlayers[id], false);
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
            if (!_safeZonePlayers.ContainsKey(GetId(player))) return;
            OnPlayerLeftSafeZone(player, _safeZonePlayers[GetId(player)], false);
        }

        private void OnPlayerUpdatePosition(RocketPlayer player, Vector3 position)
        {
            uint id = GetId(player);
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

            if (!bIsInSafeZone && _safeZonePlayers.ContainsKey(id))
            {
                OnPlayerLeftSafeZone(player, _safeZonePlayers[id], true);
            }
            else if (bIsInSafeZone && !_safeZonePlayers.ContainsKey(id))
            {
                OnPlayerEnteredSafeZone(player, safeZone, true);
                return;
            }

            //Player is still inside or outside a safezone, don't update anything
        }

        private void OnPlayerEnteredSafeZone(RocketPlayer player, SafeZone safeZone, bool bSendMessage)
        {
            uint id = GetId(player);
            if(safeZone == null) throw new ArgumentNullException("safeZone");
            EnableGodMode(player);
            _safeZonePlayers.Add(id, safeZone);

            if (bSendMessage)
            {
                //Todo: use translation
                RocketChat.Say(player.CSteamID, "Entered safe zone: " + safeZone.Name, Color.green);
            }
        }

        private void OnPlayerLeftSafeZone(RocketPlayer player, SafeZone safeZone, bool bSendMessage)
        {
            uint id = GetId(player);
            DisableGodMode(player);
            _safeZonePlayers.Remove(id);

            if (bSendMessage)
            {
                //Todo: use translation
                RocketChat.Say(player.CSteamID, "Left safe zone: " + safeZone.Name, Color.red);
            }
        }

        private void EnableGodMode(RocketPlayer player)
        {
            uint id = GetId(player);
            //Safe current godmode state and restore it later when the player leaves the safezone
            //this is for e.g. players who enter with /god safezones
            _godModeStates.Add(id, player.Features.GodMode);
            player.Features.GodMode = true;
        }

        private void DisableGodMode(RocketPlayer player)
        {
            uint id = GetId(player);
            try
            {
                //Try to restore previous godmode state
                player.Features.GodMode = _godModeStates[id];
            }
            catch (Exception ex)
            {
                //Something went wrong??
                Logger.LogException(ex);
                player.Features.GodMode = false;
            }
            _godModeStates.Remove(id);
        }

        private static bool ContainsPosition(SafeZone zone, Vector3 pos)
        {
            Position p = new Position() { X = pos.x, Y = pos.z };
            
            Position p1 = zone.Position1;
            Position p4 = zone.Position2;
            Position p2 = new Position() {X = p4.X, Y = p1.Y};
            Position p3 = new Position() {X = p1.X, Y = p4.Y };

            return IsPointInPolygon(new[] {p1, p2, p3, p4}, p);
        }

        private static bool IsPointInPolygon(Position[] polygon, Position point)
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

        private readonly Dictionary<uint, Position> _firstPositions = new Dictionary<uint, Position>();
        private readonly Dictionary<uint, Position> _secondsPositions = new Dictionary<uint, Position>();

        public void SetPosition1([NotNull] RocketPlayer player, [NotNull] Position pos)
        {
            _firstPositions.Add(GetId(player), pos);
        }

        public void SetPosition2([NotNull] RocketPlayer player, [NotNull] Position pos)
        {
            _secondsPositions.Add(GetId(player), pos);
        }

        public bool HasPositionSet([NotNull] RocketPlayer player)
        {
            return _firstPositions.ContainsKey(GetId(player)) && _secondsPositions.ContainsKey(GetId(player));
        }

        public static uint GetId(RocketPlayer player)
        {
            CSteamID id = player.CSteamID;
            return id.GetAccountID().m_AccountID;
        }

        public Position GetPosition1(RocketPlayer caller)
        {
            return _firstPositions[GetId(caller)];
        }

        public Position GetPosition2(RocketPlayer caller)
        {
            return _secondsPositions[GetId(caller)];
        }
    }
}