using System;
using System.Collections.Generic;
using System.Globalization;
using Rocket.API;
using Rocket.API.Extensions;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using RocketRegions.Model;
using RocketRegions.Util;
using Steamworks;
using UnityEngine;

namespace RocketRegions.Commands
{
    public abstract class ListHandleCommand : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 2)
            {
                this.SendUsage(caller);
                return;
            }

            var regionName = command.GetStringParameter(1);
            Region region = RegionsPlugin.Instance.GetRegion(regionName);
            if (region == null)
            {
                UnturnedChat.Say(caller, "Region \"" + regionName + "\" not found!", Color.red);
                return;
            }

            if (!region.IsOwner(caller))
            {
                UnturnedChat.Say(caller, "You do not have permission to manage this region!", Color.red);
                return;
            }

            switch (command.GetStringParameter(0).ToLower(CultureInfo.InvariantCulture))
            {
                case "add":
                    {
                        string name;
                        CSteamID target = GetTarget(command.GetStringParameter(2), out name);
                        if (target == CSteamID.Nil)
                        {
                            UnturnedChat.Say(caller, "Player not found", Color.red);
                            return;
                        }

                        Add(region, target.m_SteamID);
                        UnturnedChat.Say(caller, "Done", Color.green);
                        break;
                    }
                case "remove":
                    {
                        string name;
                        CSteamID target = GetTarget(command.GetStringParameter(2), out name);
                        if (target == CSteamID.Nil)
                        {
                            UnturnedChat.Say(caller, "Player not found", Color.red);
                            return;
                        }
                        Remove(region, target.m_SteamID);
                        UnturnedChat.Say(caller, "Done", Color.green);
                        break;
                    }

                case "list":
                {
                    var list = GetList(region);
                    if (list.Count == 0)
                    {
                        UnturnedChat.Say(caller, "No players found", Color.red);
                        return;
                    }

                    foreach (ulong id in list)
                    {
                        UnturnedPlayer player = null;
                        try
                        {
                            player = UnturnedPlayer.FromCSteamID(new CSteamID(id));
                            if (player?.Player == null)
                                player = null;
                        }
                        catch (Exception) { }

                        if (player != null)
                        {
                            UnturnedChat.Say(caller, "* " + player.DisplayName);
                            continue;
                        }
                        UnturnedChat.Say(caller, "* " + id);
                    }

                    break;
                }

                default:
                    this.SendUsage(caller);
                    break;
            }

            RegionsPlugin.Instance.Configuration.Save();
        }

        protected abstract void Remove(Region region, ulong mSteamID);

        protected abstract void Add(Region steamID, ulong mSteamID);

        protected abstract List<ulong> GetList(Region region);

        private CSteamID GetTarget(string search, out string name)
        {
            name = null;
            UnturnedPlayer player = UnturnedPlayer.FromName(search);
            if (player != null)
            {
                name = player.CharacterName;
                return player.CSteamID;
            }

            ulong val;
            if (!ulong.TryParse(search, out val))
                return CSteamID.Nil;
            name = val.ToString();
            return new CSteamID(val);
        }
        
        #region Properties

        public abstract AllowedCaller AllowedCaller { get; }
        
        public abstract string Name { get; }
        
        public abstract string Help { get; }
        
        public string Syntax => "<add/remove/list> <region> <name/SteamID>";
        
        public abstract List<string> Aliases { get; }
        
        public abstract List<string> Permissions { get; }
     
        #endregion
    }
}