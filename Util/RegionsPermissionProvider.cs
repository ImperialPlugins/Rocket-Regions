using System.Collections.Generic;
using Rocket.API;
using Rocket.API.Serialisation;

namespace RocketRegions.Util
{
    public class RegionsPermissionsProvider : IRocketPermissionsProvider
    {
        private readonly IRocketPermissionsProvider _basePermissionProvider;

        internal RegionsPermissionsProvider(IRocketPermissionsProvider basePermissionProvider)
        {
            _basePermissionProvider = basePermissionProvider;
        }

        public RocketPermissionsProviderResult AddGroup(RocketPermissionsGroup group) => _basePermissionProvider.AddGroup(@group);

        public RocketPermissionsProviderResult AddPlayerToGroup(string groupId, IRocketPlayer player) => _basePermissionProvider.AddPlayerToGroup(groupId, player);

        public RocketPermissionsProviderResult DeleteGroup(string groupId) => _basePermissionProvider.DeleteGroup(groupId);

        public RocketPermissionsGroup GetGroup(string groupId) => _basePermissionProvider.GetGroup(groupId);

        public List<RocketPermissionsGroup> GetGroups(IRocketPlayer player, bool includeParentGroups) => _basePermissionProvider.GetGroups(player, includeParentGroups);

        public List<Permission> GetPermissions(IRocketPlayer player) => _basePermissionProvider.GetPermissions(player);

        public List<Permission> GetPermissions(IRocketPlayer player, List<string> requestedPermissions) => _basePermissionProvider.GetPermissions(player, requestedPermissions);

        public bool HasPermission(IRocketPlayer player, List<string> requestedPermissions) => requestedPermissions.Contains("rinfo") || _basePermissionProvider.HasPermission(player, requestedPermissions);

        public void Reload() => _basePermissionProvider.Reload();

        public RocketPermissionsProviderResult RemovePlayerFromGroup(string groupId, IRocketPlayer player) => _basePermissionProvider.RemovePlayerFromGroup(groupId, player);

        public RocketPermissionsProviderResult SaveGroup(RocketPermissionsGroup group) => _basePermissionProvider.SaveGroup(@group);
    }
}