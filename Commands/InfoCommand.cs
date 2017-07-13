using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;

namespace RocketRegions.Commands
{
    public class InfoCommand : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedChat.Say(caller, "Rocket Regions by Trojaner [SteamID: ]", Color.cyan);
            UnturnedChat.Say(caller, "Available under GNU Affero General Public License v3", Color.cyan);
            UnturnedChat.Say(caller, "Copyright © 2015-2017 Trojaner", Color.cyan);
            UnturnedChat.Say(caller, "Source code available under http://github.com/Trojaner25/Rocket-Regions", Color.cyan);
        }
        
        #region Properties
        
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "regionsinfo";

        public string Help => "Show license info and link to source code of the Regions Plugin";

        public string Syntax => "";

        public List<string> Aliases => new List<string> { "rinfo" };

        public List<string> Permissions => new List<string>{"regions.command.info"};
        
        #endregion
    }
}