using System;
using System.Collections;
using System.Collections.Generic;
using Rocket.Unturned;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
using Rocket.Unturned.Plugins;
using Safezone.Model;
using Steamworks;

namespace Safezone.Commands
{
    public class RedefineCommand : IRocketCommand
    {
        public void Execute(RocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                RocketChat.Say(caller.CSteamID, "Usage: /sredefine <name>", UnityEngine.Color.red);
                return;
            }

            if (!SafeZonePlugin.Instance.HasPositionSet(caller))
            {
                RocketChat.Say(caller.CSteamID, "Please set pos1 (/spos1) and pos2 (/spos2) before using this command", UnityEngine.Color.red);
                return;
            }
            String name = command.GetStringParameter(0);

            SafeZone zone = SafeZonePlugin.Instance.GetSafeZone(name, true);
            if (zone == null)
            {
                RocketChat.Say(caller.CSteamID, "Safezone \"" + name + "\" not found", UnityEngine.Color.red);
                return;
            }

            SafeZonePlugin.Instance.Configuration.SafeZones.Remove(zone);
            SafeZonePlugin.Instance.OnSafeZoneRemoved(zone);
            ArrayList args = new ArrayList(command);
            args.RemoveAt(0); // remove name
            zone.Type.Redefine(caller, args);
            SafeZonePlugin.Instance.Configuration.SafeZones.Add(zone);
            SafeZonePlugin.Instance.Configuration.Save();
            SafeZonePlugin.Instance.OnSafeZoneCreated(zone);

            RocketChat.Say(caller.CSteamID, "Successfully redefined safezone: " + name, UnityEngine.Color.green);
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