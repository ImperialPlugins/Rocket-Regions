using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using RocketRegions.Model;
using RocketRegions.Model.Flag;
using RocketRegions.Model.Flag.Impl;
using RocketRegions.Model.Safezone;
using RocketRegions.Model.Safezone.Type;
using RocketRegions.Util;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace RocketRegions
{
    public class RegionsPlugin : RocketPlugin<RegionsConfiguration>
    {
        public static RegionsPlugin Instance;
        private readonly Dictionary<ulong, Region> _playersInRegions = new Dictionary<ulong, Region>();
        private readonly Dictionary<ulong, Vector3> _lastPositions = new Dictionary<ulong, Vector3>();
        internal List<Region> Regions => Configuration?.Instance?.Regions ?? new List<Region>();

        protected override void Load()
        {
            foreach (var untPlayer in Provider.clients.Select(p => UnturnedPlayer.FromCSteamID(p.SteamPlayerID.CSteamID)))
            {
                OnPlayerConnect(untPlayer);
            }

            Instance = this;

            RegionType.RegisterSafeZoneType("rectangle", typeof(RectangleType));
            RegionType.RegisterSafeZoneType("circle", typeof(CircleType));


            RegionFlag.RegisterFlag("Godmode", typeof(GodmodeFlag));
            RegionFlag.RegisterFlag("NoEnter", typeof(NoEnterFlag));
            RegionFlag.RegisterFlag("NoLeave", typeof(NoLeaveFlag));
            RegionFlag.RegisterFlag("NoZombies", typeof(NoZombiesFlag));
            RegionFlag.RegisterFlag("NoPlace", typeof(NoPlaceFlag));
            RegionFlag.RegisterFlag("NoDestroy", typeof(NoDestroy));
            RegionFlag.RegisterFlag("NoVehiclesUsage", typeof(NoVehiclesUsage));

            RegionFlag.RegisterFlag("EnterMessage", typeof(EnterMessageFlag));
            RegionFlag.RegisterFlag("LeaveMessage", typeof(LeaveMessageFlag));
            
            Configuration.Load();
            if (Configuration.Instance.UpdateFrameCount <= 0)
            {
                Configuration.Instance.UpdateFrameCount = 1;
                Configuration.Save();
            }

            foreach (Region s in Regions)
            {
                s.RebuildFlags();
            }

            if (Regions.Count < 1) return;
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

            foreach (var safeZone in Regions)
            {
                OnRegionRemoved(safeZone);
            }
            StopListening();
            Instance = null;
            RegionType.RegistereTypes?.Clear();
            RegionFlag.RegisteredFlags.Clear();
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

        internal void OnRegionCreated(Region region)
        {
            if (Regions.Count != 1) return;
            StartListening();
        }

        internal void OnRegionRemoved(Region region)
        {
            //Update players in regions
            foreach (var id in GetUidsInSafeZone(region))
            {
                OnPlayerLeftSafeZone(UnturnedPlayer.FromCSteamID(new CSteamID(id)), region);
            }

            if (Regions.Count != 0) return;
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
            if (!_playersInRegions.ContainsKey(PlayerUtil.GetId(player))) return;
            OnPlayerLeftSafeZone(player, _playersInRegions[PlayerUtil.GetId(player)]);
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
            var bIsInRegion = safeZone != null;

            Vector3? lastPosition = null;
            if (_lastPositions.ContainsKey(id))
            {
                lastPosition = _lastPositions[id];
            }

            if (!bIsInRegion && _playersInRegions.ContainsKey(id))
            {
                //Left a region
                safeZone = _playersInRegions[id];
                if (safeZone.GetFlag(typeof(NoLeaveFlag)).GetValue<bool>(safeZone.GetGroup(player)) 
                    && lastPosition != null)
                {
                    //Todo: send message to player (can't leave region)
                    untPlayer.Teleport(lastPosition.Value, untPlayer.Rotation);
                    return;
                }
                OnPlayerLeftSafeZone(player, safeZone);
            }
            else if (bIsInRegion && !_playersInRegions.ContainsKey(id) && lastPosition != null)
            {
                //Entered a region
                if (safeZone.GetFlag(typeof(NoEnterFlag)).GetValue<bool>(safeZone.GetGroup(player)))
                {
                    //Todo: send message to player (can't enter region)
                    untPlayer.Teleport(lastPosition.Value, untPlayer.Rotation);
                    return;
                }
                OnPlayerEnteredSafeZone(player, safeZone);
            }
            else
            {
                //Player is still inside or outside a region
            }


            if (safeZone != null)
            {
                foreach (RegionFlag f in safeZone.ParsedFlags)
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

        private void OnPlayerEnteredSafeZone(IRocketPlayer player, Region region)
        {
            var id = PlayerUtil.GetId(player);
            if(id == CSteamID.Nil.m_SteamID) throw new Exception("CSteamID is Nil");

            _playersInRegions.Add(id, region);

            foreach (var flag in region.ParsedFlags)
            {
                flag.OnSafeZoneEnter((UnturnedPlayer)player);
            }
        }

        internal void OnPlayerLeftSafeZone(IRocketPlayer player, Region region)
        {
            var id = PlayerUtil.GetId(player);
            if (id == CSteamID.Nil.m_SteamID) throw new Exception("CSteamID is Nil");
            _playersInRegions.Remove(id);

            foreach (var flag in region.ParsedFlags)
            {
                flag.OnSafeZoneLeave((UnturnedPlayer)player);
            }
        }

        private static bool IsInSafeZone(Vector3 pos, Region zone)
        {
            return zone.Type.IsInSafeZone(new SerializablePosition(pos));
        }

        public Region GetSafeZoneAt(Vector3 pos)
        {
            return Regions.FirstOrDefault(safeZone => IsInSafeZone(pos, safeZone));
        }

        public Region GetSafeZone(string safeZoneName, bool exact = false)
        {
            if (Regions == null || Regions.Count == 0) return null;

            foreach (var safeZone in Regions)
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

        public IEnumerable<ulong> GetUidsInSafeZone(Region zone)
        {
            return _playersInRegions.Keys.Where(id => _playersInRegions[id] == zone).ToList();
        }

        private int _frame;
        private void Update()
        {
            _frame++;
            if (_frame % Configuration.Instance.UpdateFrameCount != 0)
                return;

            foreach (var region in Regions)
            {
                var flags = region.ParsedFlags;
                var players = _playersInRegions.Where(c => c.Value == region).
                    Select(player => UnturnedPlayer.FromCSteamID(new CSteamID(player.Key))).ToList();
                foreach (var flag in flags)
                {
                    flag.UpdateState(players);
                }
            }
        }
    }
}