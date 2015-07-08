using System;
using System.Collections;
using System.Collections.Generic;
using Rocket.Unturned;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
using Rocket.Unturned.Plugins;
using Safezone.Model;
using UnityEngine;

namespace Safezone.Commands
{
    public class CreateCommand :IRocketCommand
    {
        public void Execute(RocketPlayer caller, string[] command)
        {
            if (command.Length < 2)
            {
                RocketChat.Say(caller.CSteamID, "Usage: /screate <type> <name>", Color.red);
                return;
            }

            String name = command.GetStringParameter(0);
            
            if (SafeZonePlugin.Instance.GetSafeZone(name, true) != null)
            {
                RocketChat.Say(caller.CSteamID, "A safezone with this name already exists!", Color.red);
                return;
            }

            SafeZoneType type = SafeZoneType.CreateSafeZoneType(name);
            if (type == null)
            {
                String types = "";
                foreach (String typeName in SafeZoneType.GetTypes())
                {
                    if (types == "")
                    {
                        types = typeName;
                        continue;
                    }

                    types = ", " + typeName;
                }
                RocketChat.Say(caller.CSteamID, "Unknown type: " + name + "! Available types: " + types, Color.red);
                return;
            }
            ArrayList args = new ArrayList(command);
            args.RemoveAt(0); // remove name

            SafeZone safeZone = type.Create(caller, name, (string[]) args.ToArray(typeof(string)));

            if (safeZone == null)
            {
                RocketChat.Say(caller.CSteamID, "Could't create safezone!", Color.red);
                return;
            }

            SafeZonePlugin.Instance.Configuration.SafeZones.Add(safeZone);
            SafeZonePlugin.Instance.Configuration.Save();
            SafeZonePlugin.Instance.OnSafeZoneCreated(safeZone);

            RocketChat.Say(caller.CSteamID, "Successfully created safezone: " + name, Color.green);
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