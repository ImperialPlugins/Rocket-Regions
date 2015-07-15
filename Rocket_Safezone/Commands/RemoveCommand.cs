using System;
using System.Collections.Generic;
using Rocket.Unturned;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
using Rocket.Unturned.Plugins;
using Safezone.Model;
using Safezone.Model.Safezone;
using Safezone.Util;
using UnityEngine;

namespace Safezone.Commands
{
    public class RemoveCommand : IRocketCommand
    {
        public void Execute(RocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                RocketChat.Say(caller.CSteamID, "Usage: /sremove <name>", Color.red);
                return;
            }

            String name = command.GetStringParameter(0);
            SafeZone zone = SafeZonePlugin.Instance.GetSafeZone(name, false);
            if (zone == null)
            {
                RocketChat.Say(caller.CSteamID, "Safezone \"" + name + "\" not found", Color.red);
                return;
            }

            if (!zone.IsOwner(SafeZonePlugin.GetId(caller)) && !PermissionUtil.HasPermission(caller, "remove.override"))
            {
                RocketChat.Say(caller.CSteamID, "You're not the owner of this region!", Color.red);
                return;
            }

            SafeZonePlugin.Instance.Configuration.SafeZones.Remove(zone);
            SafeZonePlugin.Instance.Configuration.Save();
            SafeZonePlugin.Instance.OnSafeZoneRemoved(zone);

            RocketChat.Say(caller.CSteamID, "Successfully removed safezone: " + name, Color.green);
        }

        public bool RunFromConsole
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
    }
}