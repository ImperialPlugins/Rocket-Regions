using Rocket.API;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using RocketRegions.Model;
using RocketRegions.Model.Flag;
using RocketRegions.Model.Flag.Impl;
using RocketRegions.Model.RegionType;
using RocketRegions.Util;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace RocketRegions
{
    public class RegionsPlugin : RocketPlugin<RegionsConfiguration>
    {
        public static RegionsPlugin Instance;
        private readonly Dictionary<ulong, Region> _playersInRegions = new Dictionary<ulong, Region>();
        private readonly Dictionary<ulong, Vector3> _lastPositions = new Dictionary<ulong, Vector3>();
        public List<Region> Regions => Configuration?.Instance?.Regions ?? new List<Region>();
        private IRocketPermissionsProvider _defaultPermissionsProvider;
        public event RegionsLoaded OnRegionsLoaded;
        public const string VERSION = "1.3.0.0";

        protected override void Load()
        {
            Logger.Log($"Regions v{VERSION}", ConsoleColor.Cyan);
            Instance = this;

            RegionType.RegisterRegionType("rectangle", typeof(RectangleType));
            RegionType.RegisterRegionType("circle", typeof(CircleType));

            RegionFlag.RegisterFlag("Godmode", typeof(GodmodeFlag));
            RegionFlag.RegisterFlag("NoEnter", typeof(NoEnterFlag));
            RegionFlag.RegisterFlag("NoLeave", typeof(NoLeaveFlag));
            RegionFlag.RegisterFlag("NoZombies", typeof(NoZombiesFlag));
            RegionFlag.RegisterFlag("NoPlace", typeof(NoPlaceFlag));
            RegionFlag.RegisterFlag("NoDestroy", typeof(NoDestroyFlag));
            RegionFlag.RegisterFlag("NoVehiclesUsage", typeof(NoVehiclesUsageFlag));
            RegionFlag.RegisterFlag("NoEquip", typeof(NoEquipFlag));
            RegionFlag.RegisterFlag("NoEquipWeapon", typeof(NoEquipWeaponFlag));
            RegionFlag.RegisterFlag("EnterMessage", typeof(EnterMessageFlag));
            RegionFlag.RegisterFlag("LeaveMessage", typeof(LeaveMessageFlag));
            RegionFlag.RegisterFlag("Teleport", typeof(TeleportFlag));
            RegionFlag.RegisterFlag("UnlimitedGenerator", typeof(UnlimitedGeneratorFlag));
            RegionFlag.RegisterFlag("EnterEffect", typeof(EnterEffectFlag));
            RegionFlag.RegisterFlag("LeaveEffect", typeof(LeaveEffectFlag));
            RegionFlag.RegisterFlag("EnterURL", typeof(EnterURLFlag));
            RegionFlag.RegisterFlag("LeaveURL", typeof(LeaveURLFlag));
            RegionFlag.RegisterFlag("VanishFlag", typeof(VanishFlag));

            Configuration.Load();

            _defaultPermissionsProvider = R.Permissions;
            R.Permissions = new RegionsPermissionsProvider(_defaultPermissionsProvider);
            R.Plugins.OnPluginsLoaded += OnPluginsLoaded;

            foreach (var untPlayer in Provider.clients.Select(p => UnturnedPlayer.FromCSteamID(p.playerID.steamID)))
            {
                OnPlayerConnect(untPlayer);
            }
        }

        private void OnPluginsLoaded()
        {
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
            OnRegionsLoaded?.Invoke(this, Regions);
        }

        protected override void Unload()
        {
            if (Provider.clients != null)
            {
                foreach (
                    var untPlayer in
                        Provider.clients.Select(
                            p => UnturnedPlayer.FromCSteamID(p?.playerID?.steamID ?? CSteamID.Nil)))
                {
                    OnPlayerDisconnect(untPlayer);
                }
            }

            foreach (var region in Regions)
            {
                OnRegionRemoved(region);
            }

            R.Permissions = _defaultPermissionsProvider;
            R.Plugins.OnPluginsLoaded -= OnPluginsLoaded;
            StopListening();
            Instance = null;
            RegionType.RegisteredTypes?.Clear();
            RegionFlag.RegisteredFlags?.Clear();
            _playersInRegions.Clear();
            _lastPositions.Clear();
        }


        public void StartListening()
        {
            //Start listening to events
            //UnturnedPlayerEvents.OnPlayerUpdatePosition += OnPlayerUpdatePosition;
            U.Events.OnPlayerConnected += OnPlayerConnect;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnect;
        }

        public void StopListening()
        {
            //Stop listening to events
            //UnturnedPlayerEvents.OnPlayerUpdatePosition -= OnPlayerUpdatePosition;
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
            foreach (var id in GetUidsInRegion(region))
            {
                OnPlayerLeftRegion(UnturnedPlayer.FromCSteamID(new CSteamID(id)), region);
            }

            if (Regions.Count != 0) return;
            StopListening();
        }

        private void OnPlayerConnect(IRocketPlayer player)
        {
            var untPlayer = PlayerUtil.GetUnturnedPlayer(player);
            _lastPositions.Add(PlayerUtil.GetId(player), untPlayer.Position);
            var region = GetRegionAt(untPlayer.Position);
            if (region != null)
            {
                OnPlayerEnteredRegion(player, region);
            }
        }

        private void OnPlayerDisconnect(IRocketPlayer player)
        {
            _lastPositions.Remove(PlayerUtil.GetId(player));
            if (!_playersInRegions.ContainsKey(PlayerUtil.GetId(player))) return;
            OnPlayerLeftRegion(player, _playersInRegions[PlayerUtil.GetId(player)]);
        }

        private void OnPlayerUpdatePosition(IRocketPlayer player, Vector3 position)
        {
            var id = PlayerUtil.GetId(player);
            var untPlayer = PlayerUtil.GetUnturnedPlayer(player);
            if (untPlayer == null)
            {
#if DEBUG
                Logger.LogError("untplayer == null OnPlayerUpdatePosition");
#endif
                return;
            }


            var currentRegion = GetRegionAt(position);
            var oldRegion = _playersInRegions.ContainsKey(id) ? _playersInRegions[id] : null;

            Vector3? lastPosition = null;
            if (_lastPositions.ContainsKey(id))
            {
                lastPosition = _lastPositions[id];
            }

            if (oldRegion != null && oldRegion != currentRegion)
            {
                //Left a region
                var flag = oldRegion.GetFlag<NoLeaveFlag>();
                var value = flag?.GetValueSafe(currentRegion.GetGroup(player));
                if (value.HasValue && value.Value && lastPosition != null)
                {
                    //Todo: send message to player (can't leave region)
                    untPlayer.Teleport(lastPosition.Value, untPlayer.Rotation);
                    return;
                }
                OnPlayerLeftRegion(player, oldRegion);
            }
            else if (oldRegion == null && currentRegion != null)
            {
                //Entered a region
                var flag = currentRegion.GetFlag<NoEnterFlag>();
                if (flag != null && flag.GetValueSafe(currentRegion.GetGroup(player))
                    && lastPosition != null)
                {
                    //Todo: send message to player (can't enter region)
                    untPlayer.Teleport(lastPosition.Value, untPlayer.Rotation);
                    return;
                }
                OnPlayerEnteredRegion(player, currentRegion);
            }

            if (currentRegion != null)
            {
                foreach (RegionFlag f in currentRegion.ParsedFlags)
                {
                    f?.OnPlayerUpdatePosition(untPlayer, position);
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

        private void OnPlayerEnteredRegion(IRocketPlayer player, Region region)
        {
            var id = PlayerUtil.GetId(player);
            _playersInRegions.Add(id, region);

            foreach (var flag in region.ParsedFlags)
            {
                flag.OnRegionEnter((UnturnedPlayer)player);
            }
        }

        internal void OnPlayerLeftRegion(IRocketPlayer player, Region region)
        {
            var id = PlayerUtil.GetId(player);
            _playersInRegions.Remove(id);

            foreach (var flag in region.ParsedFlags)
            {
                flag.OnRegionLeave((UnturnedPlayer)player);
            }
        }

        private static bool IsInRegion(Vector3 pos, Region region)
        {
            return region.Type.IsInRegion(new SerializablePosition(pos));
        }

        public Region GetRegionAt(Vector3 pos)
        {
            return Regions.FirstOrDefault(region => IsInRegion(pos, region));
        }

        public Region GetRegion(string regionName, bool exact = false)
        {
            if (Regions == null || Regions.Count == 0) return null;

            foreach (var region in Regions)
            {
                if (exact)
                {
                    if (region.Name.Equals(regionName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return region;
                    }
                    continue;
                }

                if (region.Name.ToLower().Trim().StartsWith(regionName.ToLower().Trim()))
                {
                    return region;
                }
            }
            return null;
        }

        public IEnumerable<ulong> GetUidsInRegion(Region region)
        {
            return _playersInRegions.Keys.Where(id => _playersInRegions[id] == region).ToList();
        }

        private int _frame;
        private void Update()
        {
            _frame++;
            if (_frame % Configuration.Instance.UpdateFrameCount != 0)
                return;

            if (Level.isLoading || !Level.isLoaded)
                return;

            foreach (var player in Provider.clients)
            {              
                var lastPos = _lastPositions[player.playerID.steamID.m_SteamID];
                if (player.player.transform.position != lastPos)
                {
                    OnPlayerUpdatePosition(UnturnedPlayer.FromSteamPlayer(player), player.player.transform.position);
                }
            }

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

    public delegate void RegionsLoaded(RegionsPlugin plugin, List<Region> regions);
}