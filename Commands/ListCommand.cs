using System.Collections.Generic;
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

            var bZonesFound = false;
            foreach (var zone in RegionsPlugin.Instance.Regions)
            {
                UnturnedChat.Say(caller, "• " + zone.Name, Color.green);
                bZonesFound = true;
            }

            if (!bZonesFound)
            {
                UnturnedChat.Say(caller, "No regions found", Color.red);
            }
        }


        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "regionlist";

        public string Help => "List all regions";

        public string Syntax => "";

        public List<string> Aliases => new List<string> { "rlist" };

        public List<string> Permissions => new List<string> { "regiones.list" };
    }
}