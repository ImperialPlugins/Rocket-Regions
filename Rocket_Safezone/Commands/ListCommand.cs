using System;
using System.Collections.Generic;
using Rocket.Unturned;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
using Rocket.Unturned.Plugins;
using Steamworks;
using UnityEngine;

namespace Rocket_Safezone.Commands
{
    public class ListCommand : IRocketCommand
    {
        public void Execute(RocketPlayer caller, string[] command)
        {
            RocketChat.Say(caller.CSteamID, "Safezones:", Color.green);

            bool bZonesFound = false;
            foreach (SafeZone zone in SafeZonePlugin.Instance.Configuration.SafeZones)
            {
                RocketChat.Say(caller.CSteamID, "• " + zone.Name, Color.green);
                bZonesFound = true;
            }

            if (!bZonesFound)
            {
                RocketChat.Say(caller.CSteamID, "No SafeZones found", Color.red);
            }
        }

        public bool RunFromConsole
        {
            get { return false; }
        }

        public string Name
        {
            get { return "safezonelist"; }
        }

        public string Help
        {
            get { return "List all safezones"; }
        }

        public string Syntax
        {
            get { return ""; }
        }

        public List<string> Aliases
        {
            get { return new List<string> { "slist" }; }
        } 
    }
}