using System;
using System.Collections;
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
    public class RedefineCommand : IRocketCommand
    {
        public void Execute(RocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                RocketChat.Say(caller.CSteamID, "Usage: /sredefine <name> [...]", Color.red);
                return;
            }

            String name = command.GetStringParameter(0);

            SafeZone zone = SafeZonePlugin.Instance.GetSafeZone(name, true);
            if (zone == null)
            {
                RocketChat.Say(caller.CSteamID, "Safezone \"" + name + "\" not found", Color.red);
                return;
            }

            if (!zone.IsOwner(SafeZonePlugin.GetId(caller)) && !PermissionUtil.HasPermission(caller, "redefine.override"))
            {
                RocketChat.Say(caller.CSteamID, "You're not the owner of this region!", Color.red);
                return;
            }

            ArrayList args = new ArrayList(command);
            args.RemoveAt(0);

            if (zone.Type.OnRedefine(caller, (string[]) args.ToArray(typeof (string))))
            {
                SafeZonePlugin.Instance.Configuration.SafeZones.Remove(zone);
                SafeZonePlugin.Instance.Configuration.Save();
                SafeZonePlugin.Instance.OnSafeZoneRemoved(zone);
                SafeZonePlugin.Instance.Configuration.SafeZones.Add(zone);
                SafeZonePlugin.Instance.Configuration.Save();
                SafeZonePlugin.Instance.OnSafeZoneCreated(zone);
            }

            RocketChat.Say(caller.CSteamID, "Successfully redefined safezone: " + name, Color.green);
        }

        public bool RunFromConsole
        {
            get { return false; }
        }

        public string Name
        {
            get { return "safezoneredefine"; }
        }

        public string Help
        {
            get { return "Redefine a safezone"; }
        }

        public string Syntax
        {
            get { return "<name>"; }
        }

        public List<string> Aliases
        {
            get { return new List<string> { "sredefine" }; }
        }
    }
}