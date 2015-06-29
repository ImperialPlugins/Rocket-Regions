using System;
using System.Collections.Generic;
using Rocket.Unturned;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
using Rocket.Unturned.Plugins;

namespace Safezone.Commands
{
    public class CreateCommand :IRocketCommand
    {
        public void Execute(RocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                RocketChat.Say(caller.CSteamID, "Usage: /screate <name>", UnityEngine.Color.red);
                return;
            }

            if (!SafeZonePlugin.Instance.HasPositionSet(caller))
            {
                RocketChat.Say(caller.CSteamID, "Please set pos1 (/spos1) and pos2 (/spos2) before using this command", UnityEngine.Color.red);
                return;
            }
            String name = command.GetStringParameter(0);
            
            if (SafeZonePlugin.Instance.GetSafeZone(name, true) != null)
            {
                RocketChat.Say(caller.CSteamID, "A safezone with this name already exists!", UnityEngine.Color.red);
                return;
            }
            
            SafeZone zone = new SafeZone
            {
                Name = name,
                Position1 = SafeZonePlugin.Instance.GetPosition1(caller),
                Position2 = SafeZonePlugin.Instance.GetPosition2(caller),
                PickupAllowed = true,
                VehiclesAllowed = true,
                NoZombies = true
            };

            SafeZonePlugin.Instance.Configuration.SafeZones.Add(zone);
            SafeZonePlugin.Instance.Configuration.Save();
            SafeZonePlugin.Instance.OnSafeZoneCreated(zone);

            RocketChat.Say(caller.CSteamID, "Successfully created safezone: " + name, UnityEngine.Color.green);
        }

        public bool RunFromConsole
        {
            get { return false; }
        }

        public string Name
        {
            get { return "safezonecreate"; }
        }

        public string Help
        {
            get { return "Create a safezone"; }
        }

        public string Syntax
        {
            get { return "<name>"; }
        }

        public List<string> Aliases
        {
            get { return new List<string> { "screate" }; }
        }
    }
}