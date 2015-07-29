using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;

namespace Safezone.Commands
{
    public class InfoCommand : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedChat.Say(caller, "Rocket SafeZone by Trojaner", Color.cyan);
            UnturnedChat.Say(caller, "Available under GNU Affero General Public License v3", Color.cyan);
            UnturnedChat.Say(caller, "Copyright © 2015 http://static-interface.de", Color.cyan);
            UnturnedChat.Say(caller, "Source code available under http://github.com/Trojaner25/Rocket-Safezone", Color.cyan);
        }

        public bool AllowFromConsole
        {
            get { return false; }
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

        public List<string> Permissions
        {
            get { return new List<string>(); }
        }
    }
}