using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Safezone.Model.Safezone;
using UnityEngine;

namespace Safezone.Commands
{
    public class ListCommand : IRocketCommand
    {
        public void Execute(IRocketPlayer  caller, string[] command)
        {
            UnturnedChat.Say(caller, "Safezones:", Color.green);

            bool bZonesFound = false;
            foreach (SafeZone zone in SafeZonePlugin.Instance.Configuration.Instance.SafeZones)
            {
                UnturnedChat.Say(caller, "• " + zone.Name, Color.green);
                bZonesFound = true;
            }

            if (!bZonesFound)
            {
                UnturnedChat.Say(caller, "No SafeZones found", Color.red);
            }
        }


        public AllowedCaller AllowedCaller
        {
            get
            {
                return AllowedCaller.Player;
            }
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

        public List<string> Permissions
        {
            get { return new List<string> { "safezones.list" }; }
        }
    }
}