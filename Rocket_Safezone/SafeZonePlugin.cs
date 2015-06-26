using System;
using System.Collections.Generic;
using System.Linq;
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
                RocketChat.Say(player.CSteamID, "Entered safe zone: " + safeZone.Name, UnityEngine.Color.green);
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
                RocketChat.Say(player.CSteamID, "Left safe zone: " + safeZone.Name, UnityEngine.Color.red);
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

            float x2 = p1.X;
            float y2 = p4.X;
            float x3 = p4.X;
            float y3 = p1.Y;

            Position p2 = new Position() {X = x2, Y = y2};
            Position p3 = new Position() {X = x3, Y = y3 };

            return IsInPolygon(new[] { p1, p2, p3, p4 }, p);
        }

        public static bool IsInPolygon(Position[] poly, Position point)
        {
            var coef = poly.Skip(1).Select((p, i) =>
                                            (point.Y - poly[i].Y) * (p.X - poly[i].X)
                                          - (point.X - poly[i].X) * (p.Y - poly[i].Y))
                                    .ToList();

            if (coef.Any(p => p == 0))
                return true;

            for (int i = 1; i < coef.Count(); i++)
            {
                if (coef[i] * coef[i - 1] < 0)
                    return false;
            }
            return true;
        }

        private readonly Dictionary<uint, Position> _firstPositions = new Dictionary<uint, Position>();
        private readonly Dictionary<uint, Position> _secondsPositions = new Dictionary<uint, Position>();

        public void SetPosition1(RocketPlayer player, Position pos)
        {
            if (_firstPositions.ContainsKey(GetId(player)))
            {
                _firstPositions[GetId(player)] = pos;
                return;
            }
            _firstPositions.Add(GetId(player), pos);
        }

        public void SetPosition2(RocketPlayer player, Position pos)
        {
            if (_secondsPositions.ContainsKey(GetId(player)))
            {
                _secondsPositions[GetId(player)] = pos;
                return;
            }
            _secondsPositions.Add(GetId(player), pos);
        }

        public bool HasPositionSet(RocketPlayer player)
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