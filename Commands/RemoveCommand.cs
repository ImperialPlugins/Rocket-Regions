using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;
using Rocket.API.Extensions;
using RocketRegions.Util;

namespace RocketRegions.Commands
{
    public class RemoveCommand : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                UnturnedChat.Say(caller, "Usage: /sremove <name>", Color.red);
                return;
            }

            var name = command.GetStringParameter(0);
            var zone = RegionsPlugin.Instance.GetSafeZone(name);
            if (zone == null)
            {
                UnturnedChat.Say(caller, "Safezone \"" + name + "\" not found", Color.red);
                return;
            }

            if (!zone.IsOwner(caller) && !PermissionUtil.HasPermission(caller, "remove.override"))
            {
                UnturnedChat.Say(caller, "You're not the owner of this region!", Color.red);
                return;
            }

            RegionsPlugin.Instance.Regions.Remove(zone);
            RegionsPlugin.Instance.Configuration.Save();
            RegionsPlugin.Instance.OnRegionRemoved(zone);

            UnturnedChat.Say(caller, "Successfully removed region: " + name, Color.green);
        }

        public string Name => "regionremove";

        public string Help => "Remove a region";

        public string Syntax => "<name>";

        public List<string> Aliases => new List<string> { "rremove" };

        public List<string> Permissions => new List<string> { "regiones.remove" };

        public AllowedCaller AllowedCaller => AllowedCaller.Player;
    }
}