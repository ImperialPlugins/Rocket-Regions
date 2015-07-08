using System.Collections.Generic;
using Rocket.Unturned;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
using UnityEngine;

namespace Safezone.Commands
{
    public class InfoCommand : IRocketCommand
    {
        public void Execute(RocketPlayer caller, string[] command)
        {
            RocketChat.Say(caller.CSteamID, "Rocket SafeZone by Trojaner", Color.cyan);
            RocketChat.Say(caller.CSteamID, "Available under GNU Affero General Public License v3", Color.cyan);
            RocketChat.Say(caller.CSteamID, "Copyright © 2015 http://static-interface.de", Color.cyan);
            RocketChat.Say(caller.CSteamID, "Source code available under http://github.com/Trojaner25/Rocket-Safezone", Color.cyan);
        }

        public bool RunFromConsole
        {
            get { return true; }
        }

        public string Name
        {
            get { return "safezoneinfo"; }
        }

        public string Help
        {
            get { return "Show license info and link to source code of the SafeZone Plugin"; }
        }

        public string Syntax
        {
            get { return ""; }
        }

        public List<string> Aliases
        {
            get { return new List<string> { "sinfo" }; }
        }
    }
}