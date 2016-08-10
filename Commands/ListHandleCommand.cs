using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.API.Extensions;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using RocketRegions.Model;
using Steamworks;
using UnityEngine;

namespace RocketRegions.Commands
{
    public abstract class ListHandleCommand : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 3)
            {
                SendUsage(caller);
                return;
            }

            var regionName = command.GetStringParameter(1);
            Region region = RegionsPlugin.Instance.GetRegion(regionName);
            if (region == null)
            {
                UnturnedChat.Say(caller, "Region \"" + regionName + "\" not found!", Color.red);
                return;
            }

            switch (command.GetStringParameter(0).ToLower())
            {
                case "add":
                {
                    string name;
                    CSteamID target = GetTarget(command.GetStringParameter(2), out name);
                    GetList(region).Add(target.m_SteamID);
                    break;
                }
                case "remove":
                {
                    string name;
                    CSteamID target = GetTarget(command.GetStringParameter(2), out name);
                    GetList(region).Add(target.m_SteamID);
                    break;
                }

                case "list":
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
                        catch (Exception)
                        {
                            
                        }

                        if (player != null)
                        {
                            UnturnedChat.Say(caller, "* " + player.DisplayName);
                            continue;
                        }
                        UnturnedChat.Say(caller, "* " + id);
                    }

                    break;

                default:
                    SendUsage(caller);
                    break;
            }
        }

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
            if(!ulong.TryParse(search, out val))
                return CSteamID.Nil;
            name = val.ToString();
            return new CSteamID(val);
        }

        private void SendUsage(IRocketPlayer caller)
        {
            UnturnedChat.Say("Usage: /" + Name + " " + Syntax);
            throw new WrongUsageOfCommandException(caller, this);
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Console;
        public abstract string Name { get; }
        public abstract string Help { get; }
        public string Syntax => "<add/remove/list> <region> <name/SteamID>";
        public abstract List<string> Aliases { get; }
        public abstract List<string> Permissions { get; }
    }
}