using System;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;

namespace Safezone.Util
{
    public static class PlayerUtil
    {
        public static UnturnedPlayer GetUnturnedPlayer(IRocketPlayer player)
        {
            if (player == null) return null;
            if (!(player is UnturnedPlayer))
            {
                throw new NotSupportedException("This plugin is for Unturned!");
            }

            return (UnturnedPlayer)player;
        }

        public static CSteamID GetCSteamId(IRocketPlayer player)
        {
            if (player == null) return CSteamID.Nil;
            return GetUnturnedPlayer(player).CSteamID;
        }

        public static uint GetId(IRocketPlayer player)
        {
            var id = GetCSteamId(player);
            return id.GetAccountID().m_AccountID;
        }

        public static ulong GetId(SteamPlayer player)
        {
            if (player == null) return CSteamID.Nil.m_SteamID;
            var id = player.playerID.CSteamID;
            return id.GetAccountID().m_AccountID;
        }
    }
}