using System;
using Rocket.Unturned.Player;

namespace Safezone.Util
{
    public static class PermissionUtil
    {
        public static bool HasPermission(RocketPlayer player, String permission)
        {
            return player.HasPermission("safezone" + permission) || player.HasPermission("s" + permission);
        }
    }
}