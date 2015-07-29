using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Commands;
using Safezone.Model.Safezone;
using Safezone.Util;
using UnityEngine;

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

            String name = command.GetStringParameter(0);
            SafeZone zone = SafeZonePlugin.Instance.GetSafeZone(name, false);
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

            SafeZonePlugin.Instance.Configuration.Instance.SafeZones.Remove(zone);
            SafeZonePlugin.Instance.Configuration.Save();
            SafeZonePlugin.Instance.OnSafeZoneRemoved(zone);

            UnturnedChat.Say(caller, "Successfully removed safezone: " + name, Color.green);
        }

        public bool AllowFromConsole
        {
            get { return false; }
        }

        public string Name
        {
            get { return "safezoneremove"; }
        }

        public string Help
        {
            get { return "Remove a safezone"; }
        }

        public string Syntax
        {
            get { return "<name>"; }
        }

        public List<string> Aliases
        {
            get { return new List<string> { "sremove" }; }
        }

        public List<string> Permissions
        {
            get { return new List<string> { "safezones.remove" }; }
        }
    }
}