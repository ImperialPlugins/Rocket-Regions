using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using Safezone.Model;
using Safezone.Model.Flag;
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
        private readonly Dictionary<uint, Boolean> _godModeStates = new Dictionary<uint, bool>();
        private readonly Dictionary<uint, SerializablePosition> _lastPositions = new Dictionary<uint, SerializablePosition>();
        private readonly Dictionary<uint, bool> _lastVehicleStates = new Dictionary<uint, bool>();
 
        private Timer _zombieTimer;
        protected override void Load()
        {
            Instance = this;

            SafeZoneType.RegisterSafeZoneType("rectangle", typeof(RectangleType));
            SafeZoneType.RegisterSafeZoneType("circle", typeof(CircleType));

            Flag.RegisterFlag("EnterVehicles", typeof(EnterVehiclesFlag));
            Flag.RegisterFlag("Godmode", typeof(GodmodeFlag));
            Flag.RegisterFlag("NoEnter", typeof(NoEnterFlag));
            Flag.RegisterFlag("NoLeave", typeof(NoLeaveFlag));
            Flag.RegisterFlag("NoZombie", typeof(NoZombieFlag));

            // 0 is invalid, reset it
            if (Configuration.Instance.ZombieTimerSpeed == 0)
            {
                Configuration.Instance.ZombieTimerSpeed = ((SafeZoneConfiguration)Configuration.Instance.DefaultConfiguration).ZombieTimerSpeed;
                Configuration.Save();
            }
            _zombieTimer = new Timer(Configuration.Instance.ZombieTimerSpeed * 1000);
            _zombieTimer.Elapsed += delegate { OnRemoveZombies(); };
            if (Configuration.Instance.SafeZones.Count < 1) return;
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
            UnturnedPlayerEvents.OnPlayerUpdatePosition += OnPlayerUpdatePosition;
            U.Events.OnPlayerConnected += OnPlayerConnect;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnect;
        }

        public void StopListening()
        {
            if (_zombieTimer != null)
            {
                _zombieTimer.Stop();
            }
            //Stop listening to events
            UnturnedPlayerEvents.OnPlayerUpdatePosition -= OnPlayerUpdatePosition;
            U.Events.OnPlayerConnected -= OnPlayerConnect;
            U.Events.OnPlayerDisconnected -= OnPlayerDisconnect;
        }

        internal void OnSafeZoneCreated(SafeZone safeZone)
        {
            if (Configuration.Instance.SafeZones.Count != 1) return;
            StartListening();
        }

        internal void OnSafeZoneRemoved(SafeZone safeZone)
        {
            //Update players in safezones
            foreach (uint id in GetUidsInSafeZone(safeZone))
            {
                OnPlayerLeftSafeZone(UnturnedPlayer.FromCSteamID(new CSteamID(id)), safeZone, false);
            }

            if (Configuration.Instance.SafeZones.Count != 0) return;
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

        protected override void Unload()
        {
            foreach (var safeZone in Configuration.Instance.SafeZones)
            {
                OnSafeZoneRemoved(safeZone);
            }
            StopListening();
            Instance = null;
        }

        private void OnPlayerConnect(IRocketPlayer player)
        {
            var untPlayer = PlayerUtil.GetUnturnedPlayer(player);

            if (!player.HasPermission("info"))
            {
                UnturnedChat.Say(player, "This server is running Safezones by Trojaner", Color.gray);
            }

            SafeZone safeZone = GetSafeZoneAt(untPlayer.Position);
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

            uint id = PlayerUtil.GetId(player);
            var untPlayer = PlayerUtil.GetUnturnedPlayer(player);

            SafeZone safeZone = GetSafeZoneAt(position);
            bool bIsInSafeZone = safeZone != null;

            SerializablePosition lastPosition = null;
            if (_lastPositions.ContainsKey(id))
            {
                lastPosition = _lastPositions[id];
            }

            if (!bIsInSafeZone && _safeZonePlayers.ContainsKey(id))
            {
                //Left a safezone
                safeZone = _safeZonePlayers[id];
                if (safeZone.GetFlag(typeof (NoLeaveFlag)).GetValue<bool>() && lastPosition != null)
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
                if (safeZone.GetFlag(typeof (NoEnterFlag)).GetValue<bool>())
                {
                    //Todo: send message to player (can't enter safezone)
                    untPlayer.Teleport(new Vector3(lastPosition.X, lastPosition.Y), untPlayer.Rotation);
                    return;
                }
                OnPlayerEnteredSafeZone(player, safeZone, true);
            }
            else
            {
                //Player is still inside or outside a safezone, don't update anything
            }

            if (lastPosition == null)
            {
                _lastPositions.Add(id, new SerializablePosition(untPlayer.Position));
            }
            else
            {
                _lastPositions[id] = new SerializablePosition(untPlayer.Position);
            }

            //Todo: move this codeblock somewhere else?
            if (safeZone != null)
            {
                InteractableVehicle veh = untPlayer.Player.Movement.getVehicle();
                bool isInVeh = veh != null;

                if (!_lastVehicleStates.ContainsKey(id))
                {
                    _lastVehicleStates.Add(id, veh);
                }

                bool wasDriving = _lastVehicleStates[id];
                
                if (isInVeh && !wasDriving && !safeZone.GetFlag(typeof (EnterVehiclesFlag)).GetValue<bool>())
                {
                    byte seat = 0;
                    foreach (Passenger p in untPlayer.Player.Movement.getVehicle().passengers)
                    {
                        if (PlayerUtil.GetId(p.player) == id)
                        {
                            break;
                        }
                        seat++;
                    }  
                    veh.kickPlayer(seat);
                }
             }
        }

        private void OnPlayerEnteredSafeZone(IRocketPlayer player, SafeZone safeZone, bool bSendMessage)
        {
            uint id = PlayerUtil.GetId(player);
            if (safeZone.GetFlag(typeof (GodmodeFlag)).GetValue<bool>())
            {
                EnableGodMode(player);
            }
            _safeZonePlayers.Add(id, safeZone);

            if (bSendMessage)
            {
                //Todo: use translation
                UnturnedChat.Say(player, "Entered safe zone: " + safeZone.Name, Color.green);
            }
        }

        internal void OnPlayerLeftSafeZone(IRocketPlayer player, SafeZone safeZone, bool bSendMessage)
        {
            uint id = PlayerUtil.GetId(player);

            if (safeZone.GetFlag(typeof (GodmodeFlag)).GetValue<bool>())
            {
                DisableGodMode(player);
            }
            _safeZonePlayers.Remove(id);

            if (bSendMessage)
            {
                UnturnedChat.Say(player, "Left safe zone: " + safeZone.Name, Color.red);
            }
        }

        private void EnableGodMode(IRocketPlayer player)
        {
            if (!(player is UnturnedPlayer))
            {
                throw new NotSupportedException();
            }
            uint id = PlayerUtil.GetId(player);
            var unturnedPlayer = (UnturnedPlayer) player;
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
            uint id = PlayerUtil.GetId(player);

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

        private static bool IsInSafeZone(Vector3 pos, SafeZone zone)
        {
            return zone.Type.IsInSafeZone(new SerializablePosition(pos));
        }

        public SafeZone GetSafeZoneAt(Vector3 pos)
        {
            return Configuration.Instance.SafeZones.FirstOrDefault(safeZone => IsInSafeZone(pos, safeZone));
        }

        public SafeZone GetSafeZone(String safeZoneName, bool exact = false)
        {
            if (Configuration.Instance.SafeZones == null || Configuration.Instance.SafeZones.Count == 0) return null;

            foreach (SafeZone safeZone in Configuration.Instance.SafeZones)
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

        public IEnumerable<uint> GetUidsInSafeZone(SafeZone zone)
        {
            return _safeZonePlayers.Keys.Where(id => _safeZonePlayers[id] == zone).ToList();
        }

    }
}