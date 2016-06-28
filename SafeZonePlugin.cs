using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
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
        private readonly Dictionary<uint, SafeZone> _safeZonePlayers = new Dictionary<uint, SafeZone>();
        private readonly Dictionary<uint, SerializablePosition> _lastPositions = new Dictionary<uint, SerializablePosition>();
        internal List<SafeZone> SafeZones => Configuration.Instance.SafeZones;
         
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

            // 0 is invalid, reset it
            Configuration.Load();
            if (Configuration.Instance.ZombieTimerSpeed == 0)
            {
                Configuration.Instance.ZombieTimerSpeed = 5;
                Configuration.Save();
            }

            if (SafeZones.Count < 1) return;
            StartListening();

            //Todo: loop all players and check if they are in safezones (for the case that this plugin was loaded after the start)
        }

        protected override void Unload()
        {
            foreach (var untPlayer in Provider.clients.Select(p => UnturnedPlayer.FromCSteamID(p.SteamPlayerID.CSteamID)))
            {
                OnPlayerDisconnect(untPlayer);
            }

            foreach (var safeZone in SafeZones)
            {
                OnSafeZoneRemoved(safeZone);
            }
            StopListening();
            Instance = null;
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
                OnPlayerLeftSafeZone(UnturnedPlayer.FromCSteamID(new CSteamID(id)), safeZone, false);
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
                OnPlayerEnteredSafeZone(player, safeZone, true);    
            }
        }

        private void OnPlayerDisconnect(IRocketPlayer player)
        {
            if (!_safeZonePlayers.ContainsKey(PlayerUtil.GetId(player))) return;
            OnPlayerLeftSafeZone(player, _safeZonePlayers[PlayerUtil.GetId(player)], false);
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

            SerializablePosition lastPosition = null;
            if (_lastPositions.ContainsKey(id))
            {
                lastPosition = _lastPositions[id];
            }

            if (!bIsInSafeZone && _safeZonePlayers.ContainsKey(id))
            {
                //Left a safezone
                safeZone = _safeZonePlayers[id];
                if (safeZone.GetFlag(typeof (NoLeaveFlag)).GetValue<bool>(safeZone.GetGroup(player)) && lastPosition != null)
                {
                    //Todo: send message to player (can't leave safezone)
                    untPlayer.Teleport(new Vector3(lastPosition.X, lastPosition.Y), untPlayer.Rotation);
                    return;
                }
                OnPlayerLeftSafeZone(player, safeZone, true);
            }
            else if (bIsInSafeZone && !_safeZonePlayers.ContainsKey(id) && lastPosition != null)
            {
                //Entered a safezone
                if (safeZone.GetFlag(typeof (NoEnterFlag)).GetValue<bool>(safeZone.GetGroup(player)))
                {
                    //Todo: send message to player (can't enter safezone)
                    untPlayer.Teleport(new Vector3(lastPosition.X, lastPosition.Y), untPlayer.Rotation);
                    return;
                }
                OnPlayerEnteredSafeZone(player, safeZone, true);
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
                _lastPositions.Add(id, new SerializablePosition(untPlayer.Position));
            }
            else
            {
                _lastPositions[id] = new SerializablePosition(untPlayer.Position);
            }
        }

        private void OnPlayerEnteredSafeZone(IRocketPlayer player, SafeZone safeZone, bool bSendMessage)
        {
            var id = PlayerUtil.GetId(player);
            _safeZonePlayers.Add(id, safeZone);

            foreach (var flag in safeZone.ParsedFlags)
            {
                flag.OnSafeZoneEnter((UnturnedPlayer)player);
            }

            if (bSendMessage)
            {
                //Todo: use translation
                //Todo: add flag for this
                UnturnedChat.Say(player, "Entered safe zone: " + safeZone.Name, Color.green);
            }
        }

        internal void OnPlayerLeftSafeZone(IRocketPlayer player, SafeZone safeZone, bool bSendMessage)
        {
            var id = PlayerUtil.GetId(player);
            _safeZonePlayers.Remove(id);

            foreach (var flag in safeZone.ParsedFlags)
            {
                flag.OnSafeZoneLeave((UnturnedPlayer)player);
            }

            if (bSendMessage)
            {
                UnturnedChat.Say(player, "Left safe zone: " + safeZone.Name, Color.red);
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

        public IEnumerable<uint> GetUidsInSafeZone(SafeZone zone)
        {
            return _safeZonePlayers.Keys.Where(id => _safeZonePlayers[id] == zone).ToList();
        }

        private void Update()
        {
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