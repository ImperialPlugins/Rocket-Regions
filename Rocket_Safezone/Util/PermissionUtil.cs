using System;
using Rocket.API;

namespace Safezone.Util
{
    public static class PermissionUtil
    {
        public static bool HasPermission(IRocketPlayer player, String permission)
        {
            return player.HasPermission("safezones." + permission);
        }
    }
}