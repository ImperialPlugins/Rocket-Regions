using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using Safezone.Model;
using Safezone.Model.Flag.Impl;
using Safezone.Model.Safezone;
using Safezone.Model.Safezone.Type;
using Safezone.Util;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using Flag = Safezone.Model.Flag.Flag;

namespace Safezone
{
    public class SafeZonePlugin : RocketPlugin<SafeZoneConfiguration>
    {
        public static SafeZonePlugin Instance;
        private readonly Dictionary<ulong, SafeZone> _safeZonePlayers = new Dictionary<ulong, SafeZone>();
        private readonly Dictionary<ulong, Vector3> _lastPositions = new Dictionary<ulong, Vector3>();
        internal List<SafeZone> SafeZones => Configuration?.Instance?.SafeZones ?? new List<SafeZone>();

        protected override void Load()
        {
            foreach (var untPlayer in Provider.clients.Select(p => UnturnedPlayer.FromCSteamID(p.SteamPlayerID.CSteamID)))
            {
                OnPlayerConnect(untPlayer);
            }

            Instance = this;

            SafeZoneType.RegisterSafeZoneType("rectangle", typeof(RectangleType));
            SafeZoneType.RegisterSafeZoneType("circle", typeof(CircleType));

            Flag.RegisterFlag("EnterVehicles", typeof(EnterVehiclesFlag));
            Flag.RegisterFlag("Godmode", typeof(GodmodeFlag));
            Flag.RegisterFlag("NoEnter", typeof(NoEnterFlag));
            Flag.RegisterFlag("NoLeave", typeof(NoLeaveFlag));
            Flag.RegisterFlag("NoZombies", typeof(NoZombiesFlag));
            Flag.RegisterFlag("PlaceAllowed", typeof(PlaceAllowedFlag));
            Flag.RegisterFlag("EnterMessage", typeof(EnterMessageFlag));
            Flag.RegisterFlag("LeaveMessage", typeof(LeaveMessageFlag));

            Configuration.Load();
            if (Configuration.Instance.UpdateFrameCount <= 0)
            {
                Configuration.Instance.UpdateFrameCount = 1;
                Configuration.Save();
            }

            foreach (SafeZone s in SafeZones)
            {
                s.RebuildFlags();
            }

            if (SafeZones.Count < 1) return;
            StartListening();
        }

        protected override void Unload()
        {
            if (Provider.clients != null)
            {
                foreach (
                    var untPlayer in
                        Provider.clients.Select(
                            p => UnturnedPlayer.FromCSteamID(p?.SteamPlayerID?.CSteamID ?? CSteamID.Nil)))
                {
                    OnPlayerDisconnect(untPlayer);
                }
            }

            foreach (var safeZone in SafeZones)
            {
                OnSafeZoneRemoved(safeZone);
            }
            StopListening();
            Instance = null;
            SafeZoneType.RegistereTypes?.Clear();
        }


        public void StartListening()
        {
            //Start listening to events
            UnturnedPlayerEvents.OnPlayerUpdatePosition += OnPlayerUpdatePosition;
            U.Events.OnPlayerConnected += OnPlayerConnect;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnect;
        }

        public void StopListening()
        {
            //Stop listening to events
            UnturnedPlayerEvents.OnPlayerUpdatePosition -= OnPlayerUpdatePosition;
            U.Events.OnPlayerConnected -= OnPlayerConnect;
            U.Events.OnPlayerDisconnected -= OnPlayerDisconnect;
        }

        internal void OnSafeZoneCreated(SafeZone safeZone)
        {
            if (SafeZones.Count != 1) return;
            StartListening();
        }

        internal void OnSafeZoneRemoved(SafeZone safeZone)
        {
            //Update players in safezones
            foreach (var id in GetUidsInSafeZone(safeZone))
            {
                OnPlayerLeftSafeZone(UnturnedPlayer.FromCSteamID(new CSteamID(id)), safeZone);
            }

            if (SafeZones.Count != 0) return;
            StopListening();
        }

        private void OnPlayerConnect(IRocketPlayer player)
        {
            var untPlayer = PlayerUtil.GetUnturnedPlayer(player);
            var safeZone = GetSafeZoneAt(untPlayer.Position);
            if (safeZone != null)
            {
                OnPlayerEnteredSafeZone(player, safeZone);
            }
        }

        private void OnPlayerDisconnect(IRocketPlayer player)
        {
            if (!_safeZonePlayers.ContainsKey(PlayerUtil.GetId(player))) return;
            OnPlayerLeftSafeZone(player, _safeZonePlayers[PlayerUtil.GetId(player)]);
        }

        private void OnPlayerUpdatePosition(IRocketPlayer player, Vector3 position)
        {
            if (!(player is UnturnedPlayer))
            {
                StopListening();
                throw new NotSupportedException();
            }

            var id = PlayerUtil.GetId(player);
            var untPlayer = PlayerUtil.GetUnturnedPlayer(player);

            var safeZone = GetSafeZoneAt(position);
            var bIsInSafeZone = safeZone != null;

            Vector3? lastPosition = null;
            if (_lastPositions.ContainsKey(id))
            {
                lastPosition = _lastPositions[id];
            }

            if (!bIsInSafeZone && _safeZonePlayers.ContainsKey(id))
            {
                //Left a safezone
                safeZone = _safeZonePlayers[id];
                if (safeZone.GetFlag(typeof(NoLeaveFlag)).GetValue<bool>(safeZone.GetGroup(player)) 
                    && lastPosition != null)
                {
                    //Todo: send message to player (can't leave safezone)
                    untPlayer.Teleport(lastPosition.Value, untPlayer.Rotation);
                    return;
                }
                OnPlayerLeftSafeZone(player, safeZone);
            }
            else if (bIsInSafeZone && !_safeZonePlayers.ContainsKey(id) && lastPosition != null)
            {
                //Entered a safezone
                if (safeZone.GetFlag(typeof(NoEnterFlag)).GetValue<bool>(safeZone.GetGroup(player)))
                {
                    //Todo: send message to player (can't enter safezone)
                    untPlayer.Teleport(lastPosition.Value, untPlayer.Rotation);
                    return;
                }
                OnPlayerEnteredSafeZone(player, safeZone);
            }
            else
            {
                //Player is still inside or outside a safezone
            }


            if (safeZone != null)
            {
                foreach (Flag f in safeZone.ParsedFlags)
                {
                    f.OnPlayerUpdatePosition(untPlayer, position);
                }
            }

            if (lastPosition == null)
            {
                _lastPositions.Add(id, untPlayer.Position);
            }
            else
            {
                _lastPositions[id] = untPlayer.Position;
            }
        }

        private void OnPlayerEnteredSafeZone(IRocketPlayer player, SafeZone safeZone)
        {
            var id = PlayerUtil.GetId(player);
            if(id == CSteamID.Nil.m_SteamID) throw new Exception("CSteamID is Nil");

            _safeZonePlayers.Add(id, safeZone);

            foreach (var flag in safeZone.ParsedFlags)
            {
                flag.OnSafeZoneEnter((UnturnedPlayer)player);
            }
        }

        internal void OnPlayerLeftSafeZone(IRocketPlayer player, SafeZone safeZone)
        {
            var id = PlayerUtil.GetId(player);
            if (id == CSteamID.Nil.m_SteamID) throw new Exception("CSteamID is Nil");
            _safeZonePlayers.Remove(id);

            foreach (var flag in safeZone.ParsedFlags)
            {
                flag.OnSafeZoneLeave((UnturnedPlayer)player);
            }
        }

        private static bool IsInSafeZone(Vector3 pos, SafeZone zone)
        {
            return zone.Type.IsInSafeZone(new SerializablePosition(pos));
        }

        public SafeZone GetSafeZoneAt(Vector3 pos)
        {
            return SafeZones.FirstOrDefault(safeZone => IsInSafeZone(pos, safeZone));
        }

        public SafeZone GetSafeZone(string safeZoneName, bool exact = false)
        {
            if (SafeZones == null || SafeZones.Count == 0) return null;

            foreach (var safeZone in SafeZones)
            {
                if (exact)
                {
                    if (safeZone.Name.Equals(safeZoneName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return safeZone;
                    }
                    continue;
                }

                if (safeZone.Name.ToLower().Trim().StartsWith(safeZoneName.ToLower().Trim()))
                {
                    return safeZone;
                }
            }
            return null;
        }

        public IEnumerable<ulong> GetUidsInSafeZone(SafeZone zone)
        {
            return _safeZonePlayers.Keys.Where(id => _safeZonePlayers[id] == zone).ToList();
        }

        private int _frame;
        private void Update()
        {
            _frame++;
            if (_frame % Configuration.Instance.UpdateFrameCount != 0)
                return;

            foreach (var safezone in SafeZones)
            {
                var flags = safezone.ParsedFlags;
                var players = _safeZonePlayers.Where(c => c.Value == safezone).
                    Select(player => UnturnedPlayer.FromCSteamID(new CSteamID(player.Key))).ToList();
                foreach (var flag in flags)
                {
                    flag.UpdateState(players);
                }
            }
        }
    }
}