using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Core;
using Rocket.Core.Permissions;

namespace RocketRegions.Util
{
    public class RegionsPermissionsProvider : IRocketPermissionsProvider
    {
        private readonly IRocketPermissionsProvider _basePermissionProvider;

        internal RegionsPermissionsProvider(IRocketPermissionsProvider basePermissionProvider)
        {
            _basePermissionProvider = basePermissionProvider;
        }

        public RocketPermissionsProviderResult AddGroup(RocketPermissionsGroup group)
        {
            return _basePermissionProvider.AddGroup(group);
        }

        public RocketPermissionsProviderResult AddPlayerToGroup(string groupId, IRocketPlayer player)
        {
            return _basePermissionProvider.AddPlayerToGroup(groupId, player);
        }

        public RocketPermissionsProviderResult DeleteGroup(string groupId)
        {
            return _basePermissionProvider.DeleteGroup(groupId);
        }

        public RocketPermissionsGroup GetGroup(string groupId)
        {
            return _basePermissionProvider.GetGroup(groupId);
        }

        public List<RocketPermissionsGroup> GetGroups(IRocketPlayer player, bool includeParentGroups)
        {
            return _basePermissionProvider.GetGroups(player, includeParentGroups);
        }

        public List<Permission> GetPermissions(IRocketPlayer player)
        {
            return _basePermissionProvider.GetPermissions(player);
        }

        public List<Permission> GetPermissions(IRocketPlayer player, List<string> requestedPermissions)
        {
            return _basePermissionProvider.GetPermissions(player, requestedPermissions);
        }

        public bool HasPermission(IRocketPlayer player, List<string> requestedPermissions)
        {
            if (requestedPermissions.Contains("rinfo")) return true; //PATCHED
            return _basePermissionProvider.HasPermission(player, requestedPermissions);
        }

        public void Reload()
        {
            _basePermissionProvider.Reload();
        }

        public RocketPermissionsProviderResult RemovePlayerFromGroup(string groupId, IRocketPlayer player)
        {
            return _basePermissionProvider.RemovePlayerFromGroup(groupId, player);
        }

        public RocketPermissionsProviderResult SaveGroup(RocketPermissionsGroup group)
        {
            return _basePermissionProvider.SaveGroup(group);
        }
    }
}