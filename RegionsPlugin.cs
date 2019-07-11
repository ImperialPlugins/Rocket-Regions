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

        public object InteractionVehicle { get; private set; }

        private IRocketPermissionsProvider _defaultPermissionsProvider;
        public event RegionsLoaded OnRegionsLoaded;
        public const string VERSION = "1.5.0.0";

        public delegate void OnHandleStructureDamage(Region region, Transform structureTransform, EDamageOrigin damageOrigin, ref bool shouldHandle);
        public event OnHandleStructureDamage HandleStructureDamage;
        
        public delegate void OnHandleBarricadeDamage(Region region, Transform barricadeTransform, EDamageOrigin damageOrigin, ref bool shouldHandle);
        public event OnHandleBarricadeDamage HandleBarricadeDamage;
        
        public delegate void OnHandleVehicleDamage(Region region, InteractableVehicle vehicle, EDamageOrigin damageOrigin, ref bool shouldHandle);
        public event OnHandleVehicleDamage HandleVehicleDamage;

        public delegate void OnRegionEnter(UnturnedPlayer player, Region region);
        public event OnRegionEnter RegionEnter;

        public delegate void OnRegionLeave(UnturnedPlayer player, Region region);
        public event OnRegionLeave RegionLeave;

        protected override void Load()
        {
            Logger.Log($"Regions v{VERSION}", ConsoleColor.Cyan);
            Instance = this;

            if (Configuration.Instance.NoEquipIgnoredItems == null)
                Configuration.Instance.NoEquipIgnoredItems = new List<ushort>();

            if (Configuration.Instance.NoEquipWeaponIgnoredItems == null)
                Configuration.Instance.NoEquipWeaponIgnoredItems = new List<ushort>();

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
            RegionFlag.RegisterFlag("NoDecay", typeof(NoDecayFlag));
            RegionFlag.RegisterFlag("EnterAddGroup", typeof(EnterAddGroupFlag));
            RegionFlag.RegisterFlag("EnterRemoveGroup", typeof(EnterRemoveGroupFlag));
            RegionFlag.RegisterFlag("LeaveAddGroup", typeof(LeaveAddGroupFlag));
            RegionFlag.RegisterFlag("LeaveRemoveGroup", typeof(LeaveRemoveGroupFlag));
            RegionFlag.RegisterFlag("Decay", typeof(DecayFlag));
            RegionFlag.RegisterFlag("NoVehicleDamage", typeof(NoVehiclesDamageFlag));
            RegionFlag.RegisterFlag("NoVehiclesLockpick", typeof(NoVehiclesLockpickFlag));
            Configuration.Load();

            _defaultPermissionsProvider = R.Permissions;
            R.Permissions = new RegionsPermissionsProvider(_defaultPermissionsProvider);
            R.Plugins.OnPluginsLoaded += OnPluginsLoaded;

            foreach (var untPlayer in Provider.clients.Select(p => UnturnedPlayer.FromCSteamID(p.playerID.steamID)))
                OnPlayerConnect(untPlayer);
        }

        private void OnPluginsLoaded()
        {
            Configuration.Load();
            if (Configuration.Instance.UpdateFrameCount <= 0)
            {
                Configuration.Instance.UpdateFrameCount = 1;
                Configuration.Save();
            }

            foreach (Region r in Regions)
                r.RebuildFlags();

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
                    OnPlayerDisconnect(untPlayer);
            }

            foreach (Region r in Regions)
                OnRegionRemoved(r);

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
            StructureManager.onDamageStructureRequested += OnDamageStructure;
            BarricadeManager.onDamageBarricadeRequested += OnDamageBarricade;
            VehicleManager.onDamageVehicleRequested += OnDamageVehicle;
            VehicleManager.onVehicleLockpicked += OnLockpickVehicle;

            UnturnedEvents.OnPlayerDamaged += OnPlayerDamaged;
            U.Events.OnPlayerConnected += OnPlayerConnect;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnect;
        }

        public void StopListening()
        {
            //Stop listening to events
            //UnturnedPlayerEvents.OnPlayerUpdatePosition -= OnPlayerUpdatePosition;
            StructureManager.onDamageStructureRequested -= OnDamageStructure;
            BarricadeManager.onDamageBarricadeRequested -= OnDamageBarricade;
            VehicleManager.onDamageVehicleRequested -= OnDamageVehicle;
            VehicleManager.onVehicleLockpicked -= OnLockpickVehicle;
            U.Events.OnPlayerConnected -= OnPlayerConnect;
            U.Events.OnPlayerDisconnected -= OnPlayerDisconnect;
        }

        private void OnLockpickVehicle(InteractableVehicle vehicle, Player instigatingPlayer, ref bool allow)
        {
            var currentRegion = GetRegionAt(vehicle.transform.position);

            if (currentRegion == null)
            {
                return;
            }

            IRocketPlayer rPlayer = (IRocketPlayer)instigatingPlayer;

            if (currentRegion.Flags.Exists(fg => fg.Name.Equals("NoVehiclesLockpick", StringComparison.OrdinalIgnoreCase)))
            {
                if (currentRegion.GetFlag<NoVehiclesLockpickFlag>()?.GetValueSafe(currentRegion.GetGroup(rPlayer)) ?? false)
                {
                    allow = false;
                }
            }
        }

        //This is to prevent other plugins overwriting god mode by accident
        private void OnPlayerDamaged(UnturnedPlayer player, ref EDeathCause cause, ref ELimb limb, ref UnturnedPlayer killer, ref Vector3 direction, ref float damage, ref float times, ref bool canDamage)
        {
            if (player.GodMode) return;

            var untPlayer = PlayerUtil.GetUnturnedPlayer(player);

            var currentRegion = GetRegionAt(untPlayer.Position);
            if (currentRegion == null) return; //No region? Not our problem!
            //Flag is stop the process of this function because the user opted to disabled the auto-protection feature
            if (currentRegion.Flags.Exists(fg => fg.Name.Equals("DisableGodModeProtection", StringComparison.OrdinalIgnoreCase)))
                return;
            try //This is just here to prevent the console logging at a lot of errors because exceptions will be generated by using this way
            {
                if (currentRegion != null)
                    OnPlayerEnteredRegion(player, currentRegion);
            } catch (Exception) { }
        }

        private void OnDamageVehicle(CSteamID instigatorSteamID, InteractableVehicle vehicle, ref ushort pendingTotalDamage, ref bool canRepair, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            var currentRegion = GetRegionAt(vehicle.transform.position);
            
            if (currentRegion == null)
                return;
            
            bool shouldHandle = true;
            HandleVehicleDamage?.Invoke(currentRegion, vehicle, damageOrigin, ref shouldHandle);
  
            if(!shouldHandle)
               return;
            
            if (currentRegion.Flags.Exists(fg => fg.Name.Equals("NoVehicleDamage", StringComparison.OrdinalIgnoreCase)) && !R.Permissions.HasPermission(new RocketPlayer(instigatorSteamID.m_SteamID.ToString()), Configuration.Instance.NoVehicleDamageIgnorePermission) && !Configuration.Instance.NoDestroyIgnoredItems.Exists(k => k == vehicle.id))
            {
                UnturnedPlayer dealer = UnturnedPlayer.FromCSteamID(instigatorSteamID);

                if (dealer == null)
                    return;

                if (dealer.HasPermission(Configuration.Instance.NoDestroyIgnorePermission) || Configuration.Instance.NoDestroyIgnoredItems.Exists(k => k == vehicle.id))
                    return;

                shouldAllow = false;
            }
        }

        private void OnDamageStructure(CSteamID instigatorSteamID, Transform structureTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            StructureManager.tryGetInfo(structureTransform, out byte x, out byte y, out ushort Index, out StructureRegion StRegion);
            var currentRegion = GetRegionAt(StRegion.structures[Index].point);
            if (currentRegion == null)
                return;

            bool shouldHandle = true;
            HandleStructureDamage?.Invoke(currentRegion, structureTransform, damageOrigin, ref shouldHandle);
  
            if(!shouldHandle)
               return;

            if (currentRegion.Flags.Exists(fg => fg.Name.Equals("NoDestroy", StringComparison.OrdinalIgnoreCase)))
            {
                UnturnedPlayer dealer = UnturnedPlayer.FromCSteamID(instigatorSteamID);

                if (dealer == null)
                    return;
                
                if (dealer.HasPermission(Configuration.Instance.NoDestroyIgnorePermission) || Configuration.Instance.NoDestroyIgnoredItems.Exists(k => k == StRegion.structures[Index].structure.id))
                    return;

                shouldAllow = false;
            }
            else
                return;
        }

        private void OnDamageBarricade(CSteamID instigatorSteamID, Transform barricadeTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            BarricadeManager.tryGetInfo(barricadeTransform, out byte x, out byte y, out ushort plant, out ushort Index, out BarricadeRegion BarRegion);
            var currentRegion = GetRegionAt(BarRegion.barricades[Index].point);
            if (currentRegion == null)
                return;
                
            bool shouldHandle = true;
            HandleBarricadeDamage?.Invoke(currentRegion, barricadeTransform, damageOrigin, ref shouldHandle);
  
            if(!shouldHandle)
               return;

            if (currentRegion.Flags.Exists(fg => fg.Name.Equals("NoDestroy", StringComparison.OrdinalIgnoreCase)))
            {
                UnturnedPlayer dealer = UnturnedPlayer.FromCSteamID(instigatorSteamID);

                if (dealer == null)
                    return;
                
                if (dealer.HasPermission(Configuration.Instance.NoDestroyIgnorePermission) || Configuration.Instance.NoDestroyIgnoredItems.Exists(k => k == BarRegion.barricades[Index].barricade.id))
                    return;

                shouldAllow = false;
            }
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
                OnPlayerLeftRegion(UnturnedPlayer.FromCSteamID(new CSteamID(id)), region);

            if (Regions.Count != 0) return;
            StopListening();
        }

        private void OnPlayerConnect(IRocketPlayer player)
        {
            var untPlayer = PlayerUtil.GetUnturnedPlayer(player);
            _lastPositions.Add(PlayerUtil.GetId(player), untPlayer.Position);
            var region = GetRegionAt(untPlayer.Position);
            if (region != null)
                OnPlayerEnteredRegion(player, region);
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
                lastPosition = _lastPositions[id];

            if (oldRegion != null && oldRegion != currentRegion)
            {
                //Left a region
                var flag = oldRegion.GetFlag<NoLeaveFlag>();
                var value = flag?.GetValueSafe(oldRegion.GetGroup(player));
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
                    f?.OnPlayerUpdatePosition(untPlayer, position);
            }

            if (lastPosition == null)
                _lastPositions.Add(id, untPlayer.Position);
            else
                _lastPositions[id] = untPlayer.Position;
        }

        private void OnPlayerEnteredRegion(IRocketPlayer player, Region region)
        {
            var id = PlayerUtil.GetId(player);
            if (!_playersInRegions.ContainsKey(id))
                _playersInRegions.Add(id, region);

            RegionEnter?.Invoke((UnturnedPlayer)player, region);

            foreach (RegionFlag f in region.ParsedFlags)
                f.OnRegionEnter((UnturnedPlayer)player);
        }

        internal void OnPlayerLeftRegion(IRocketPlayer player, Region region)
        {
            var id = PlayerUtil.GetId(player);
            _playersInRegions.Remove(id);

            RegionLeave?.Invoke((UnturnedPlayer)player, region);

            foreach (RegionFlag f in region.ParsedFlags)
            {
                try
                {
                    f.OnRegionLeave((UnturnedPlayer) player);
                }
                catch(Exception e)
                {
#if DEBUG
                    Logger.LogException(e);
#endif
                }
            }
        }

        private static bool IsInRegion(Vector3 pos, Region region) => region.Type.IsInRegion(new SerializablePosition(pos));

        public Region GetRegionAt(Vector3 pos) => Regions.FirstOrDefault(region => IsInRegion(pos, region));

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
                    return region;
            }
            return null;
        }

        public IEnumerable<ulong> GetUidsInRegion(Region region) => _playersInRegions.Keys.Where(id => _playersInRegions[id] == region).ToList();

        private int _frame;
        private void Update()
        {
            if (Configuration?.Instance == null || Provider.clients == null || _lastPositions == null || _playersInRegions == null)
                return;

            _frame++;
            if (_frame % Configuration.Instance.UpdateFrameCount != 0)
                return;

            foreach (var player in Provider.clients)
            {
                if (player?.playerID?.steamID == null)
                    continue;

                if (player?.player?.transform == null)
                    continue;

                var id = player.playerID.steamID.m_SteamID;
                if (!_lastPositions.ContainsKey(id))
                {
                    _lastPositions.Add(id, player.player.transform.position);
                    continue;
                }
                var lastPos = _lastPositions[id];
                if (player.player.transform.position != lastPos)
                    OnPlayerUpdatePosition(UnturnedPlayer.FromSteamPlayer(player), player.player.transform.position);
            }

            foreach (var region in Regions)
            {
                var flags = region?.ParsedFlags;
                if(flags == null)
                    continue;

                var players = _playersInRegions.Where(c => c.Value == region)
                    .Select(player => UnturnedPlayer.FromCSteamID(new CSteamID(player.Key))).ToList();
                foreach (RegionFlag f in flags)
                    f?.UpdateState(players.Where(c => c.Player != null).ToList());
            }
        }
    }

    public delegate void RegionsLoaded(RegionsPlugin plugin, List<Region> regions);
}
