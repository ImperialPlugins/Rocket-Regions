using System;
using System.Collections.Generic;
using Rocket.Unturned;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;

namespace Rocket_Safezone.Commands
{
    public class CreateCommand :IRocketCommand
    {
        public void Execute(RocketPlayer caller, string[] command)
        {
            if (!SafeZonePlugin.Instance.HasPositionSet(caller))
            {
                RocketChat.Say(caller.CSteamID, "Please set pos1 and pos2 before using this command", UnityEngine.Color.red);
                return;
            }
            String name = command.GetStringParameter(0);
            SafeZone zone = new SafeZone
            {
                Name = name,
                Position1 = SafeZonePlugin.Instance.GetPosition1(caller),
                Position2 = SafeZonePlugin.Instance.GetPosition2(caller),
                PickupAllowed = true,
                VehiclesAllowed = true,
                Active = true
            };

            SafeZonePlugin.Instance.Configuration.SafeZones.Add(zone);
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