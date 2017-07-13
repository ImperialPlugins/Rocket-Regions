﻿using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;

namespace RocketRegions.Commands
{
    public class ListCommand : IRocketCommand
    {
        public void Execute(IRocketPlayer  caller, string[] command)
        {
            UnturnedChat.Say(caller, "Regions:", Color.green);

            var bRegionsFound = false;
            for (var i = 0; i < RegionsPlugin.Instance.Regions.Count; i++)
            {
                UnturnedChat.Say(caller, "• " + RegionsPlugin.Instance.Regions[i].Name, Color.green);
                bRegionsFound = true;
            }

            if (!bRegionsFound)
                UnturnedChat.Say(caller, "No regions found", Color.red);
        }

        #region Properties
        
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "regionlist";

        public string Help => "List all regions";

        public string Syntax => "";

        public List<string> Aliases => new List<string> { "rlist" };

        public List<string> Permissions => new List<string> { "regions.command.list" };
        
        #endregion
    }
}