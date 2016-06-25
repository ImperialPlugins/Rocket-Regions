using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;

namespace Safezone.Commands
{
    public class ListCommand : IRocketCommand
    {
        public void Execute(IRocketPlayer  caller, string[] command)
        {
            UnturnedChat.Say(caller, "Safezones:", Color.green);

            var bZonesFound = false;
            foreach (var zone in SafeZonePlugin.Instance.Configuration.Instance.SafeZones)
            {
                UnturnedChat.Say(caller, "• " + zone.Name, Color.green);
                bZonesFound = true;
            }

            if (!bZonesFound)
            {
                UnturnedChat.Say(caller, "No SafeZones found", Color.red);
            }
        }


        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "safezonelist";

        public string Help => "List all safezones";

        public string Syntax => "";

        public List<string> Aliases => new List<string> { "slist" };

        public List<string> Permissions => new List<string> { "safezones.list" };
    }
}