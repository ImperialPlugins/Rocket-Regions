using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Logging;
using Rocket.Unturned.Player;
using Rocket.Unturned.Plugins;
using Safezone.Model;
using Safezone.Model.Flag;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace Safezone
{
    public class SafeZonePlugin : RocketPlugin<SafeZoneConfiguration>
    {
        public static SafeZonePlugin Instance;
        private readonly Dictionary<uint, SafeZone> _safeZonePlayers = new Dictionary<uint, SafeZone>();
        private readonly Dictionary<uint, Boolean> _godModeStates = new Dictionary<uint, bool>();
        private Timer _zombieTimer;
        protected override void Load()
        {
            Instance = this;

            // 0 is invalid, reset it
            if (Configuration.ZombieTimerSpeed == 0)
            {
                Configuration.ZombieTimerSpeed = ((SafeZoneConfiguration)Configuration.DefaultConfiguration).ZombieTimerSpeed;
                Configuration.Save();
            }
            _zombieTimer = new Timer(Configuration.ZombieTimerSpeed * 1000);
            _zombieTimer.Elapsed += delegate { OnRemoveZombies(); };
            if (Configuration.SafeZones.Count < 1) return;
            StartListening();

            //Todo: loop all players and check if they are in safezones (for the case that this plugin was loaded after the start)
        }

        public void StartListening()
        {
            if (_zombieTimer != null)
            {
                _zombieTimer.Start();
            }
            //Start listening to events
            RocketPlayerEvents.OnPlayerUpdatePosition += OnPlayerUpdatePosition;
            //RocketPlayerEvents.OnPlayerUpdateGesture += OnPlayerUpdateGesture; //not possible currently
            RocketServerEvents.OnPlayerConnected += OnPlayerConnect;
            RocketServerEvents.OnPlayerDisconnected += OnPlayerDisconnect;
        }

        public void StopListening()
        {
            if (_zombieTimer != null)
            {
                _zombieTimer.Stop();
            }
            //Stop listening to events
            RocketPlayerEvents.OnPlayerUpdatePosition -= OnPlayerUpdatePosition;
            //RocketPlayerEvents.OnPlayerUpdateGesture -= OnPlayerUpdateGesture;
            RocketServerEvents.OnPlayerConnected -= OnPlayerConnect;
            RocketServerEvents.OnPlayerDisconnected -= OnPlayerDisconnect;
        }

        internal void OnSafeZoneCreated(SafeZone safeZone)
        {
            if (Configuration.SafeZones.Count != 1) return;
            StartListening();
        }

        internal void OnSafeZoneRemoved(SafeZone safeZone)
        {
            //Update players in safezones
            foreach (uint id in GetUidsInSafeZone(safeZone))
            {
                OnPlayerLeftSafeZone(RocketPlayer.FromCSteamID(new CSteamID(id)), safeZone, false);
            }

            if (Configuration.SafeZones.Count != 0) return;
            StopListening();
        }

        private void OnRemoveZombies()
        {
            foreach (Zombie zombie in ZombieManager.ZombieRegions.SelectMany(t => (from zombie in t.Zombies let safeZone = GetSafeZoneAt(zombie.transform.position) where safeZone != null && safeZone.GetFlag(typeof (NoZombieFlag)).GetValue<bool>() select zombie)))
            {
                EPlayerKill pKill;
                zombie.askDamage(255, zombie.transform.up, out pKill);
            }
        }

        private void OnPlayerUpdateGesture(RocketPlayer player, RocketPlayerEvents.PlayerGesture gesture)
        {
            if (!_safeZonePlayers.ContainsKey(GetId(player))) return;
            switch (gesture)
            {
                case RocketPlayerEvents.PlayerGesture.Pickup:
                    if (_safeZonePlayers[GetId(player)].GetFlag(typeof(PickupAllowedFlag)).GetValue<bool>())
                    {
                        break;
                    }

                    //Todo cancel event (not possible currently?)
                    break;
            }
        }

        protected override void Unload()
        {
            foreach (var safeZone in Configuration.SafeZones)
            {
                OnSafeZoneRemoved(safeZone);
            }
            StopListening();
            Instance = null;
        }

        private void OnPlayerConnect(RocketPlayer player)
        {
            SafeZone safeZone = GetSafeZoneAt(player.Position);
            if (safeZone != null)
            {
                OnPlayerEnteredSafeZone(player, safeZone, true);    
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

            SafeZone safeZone = GetSafeZoneAt(position);
            bool bIsInSafeZone = safeZone != null;

            if (!bIsInSafeZone && _safeZonePlayers.ContainsKey(id))
            {
                OnPlayerLeftSafeZone(player, _safeZonePlayers[id], true);
            }
            else if (bIsInSafeZone && !_safeZonePlayers.ContainsKey(id))
            {
                OnPlayerEnteredSafeZone(player, safeZone, true);
            }
            else
            {
                //Player is still inside or outside a safezone, don't update anything
            }
        }

        private void OnPlayerEnteredSafeZone(RocketPlayer player, SafeZone safeZone, bool bSendMessage)
        {
            uint id = GetId(player);
            if (safeZone.GetFlag(typeof (GodmodeFlag)).GetValue<bool>())
            {
                EnableGodMode(player);
            }
            _safeZonePlayers.Add(id, safeZone);

            if (bSendMessage)
            {
                //Todo: use translation
                RocketChat.Say(player.CSteamID, "Entered safe zone: " + safeZone.Name, Color.green);
            }
        }

        internal void OnPlayerLeftSafeZone(RocketPlayer player, SafeZone safeZone, bool bSendMessage)
        {
            uint id = GetId(player);

            if (safeZone.GetFlag(typeof (GodmodeFlag)).GetValue<bool>())
            {
                DisableGodMode(player);
            }
            _safeZonePlayers.Remove(id);

            if (bSendMessage)
            {
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

        private static bool IsInSafeZone(Vector3 pos, SafeZone zone)
        {
            Position p = new Position { X = pos.x, Y = pos.z };
            return zone.Type.IsInSafeZone(p);
        }

        public SafeZone GetSafeZoneAt(Vector3 pos)
        {
            return Configuration.SafeZones.FirstOrDefault(safeZone => IsInSafeZone(pos, safeZone));
        }

        private readonly Dictionary<uint, Position> _firstPositions = new Dictionary<uint, Position>();
        private readonly Dictionary<uint, Position> _secondsPositions = new Dictionary<uint, Position>();

        public SafeZone GetSafeZone(String safeZoneName, bool exact = false)
        {
            if (Configuration.SafeZones == null || Configuration.SafeZones.Count == 0) return null;

            foreach (SafeZone safeZone in Configuration.SafeZones)
            {
                if (exact)
                {
                    if (safeZone.Name.ToLower() == safeZoneName.ToLower())
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

        public IEnumerable<uint> GetUidsInSafeZone(SafeZone zone)
        {
            return _safeZonePlayers.Keys.Where(id => _safeZonePlayers[id] == zone).ToList();
        }
    }
}