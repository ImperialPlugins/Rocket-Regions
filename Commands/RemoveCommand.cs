using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Safezone.Util;
using UnityEngine;
using Rocket.API.Extensions;

namespace Safezone.Commands
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
            var zone = SafeZonePlugin.Instance.GetSafeZone(name);
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

            SafeZonePlugin.Instance.SafeZones.Remove(zone);
            SafeZonePlugin.Instance.Configuration.Save();
            SafeZonePlugin.Instance.OnSafeZoneRemoved(zone);

            UnturnedChat.Say(caller, "Successfully removed safezone: " + name, Color.green);
        }

        public string Name => "safezoneremove";

        public string Help => "Remove a safezone";

        public string Syntax => "<name>";

        public List<string> Aliases => new List<string> { "sremove" };

        public List<string> Permissions => new List<string> { "safezones.remove" };

        public AllowedCaller AllowedCaller => AllowedCaller.Player;
    }
}