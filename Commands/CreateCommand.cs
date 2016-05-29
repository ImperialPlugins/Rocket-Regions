using System;
using System.Collections;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Commands;
using Safezone.Model.Safezone;
using Safezone.Model.Safezone.Type;
using UnityEngine;
using Rocket.API.Extensions;

namespace Safezone.Commands
{
    public class CreateCommand :IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 2)
            {
                UnturnedChat.Say(caller, "Usage: /screate <name> <type>", Color.red);
                return;
            }

            String name = command.GetStringParameter(0);
            String typeName = command.GetStringParameter(1).ToLower();

            if (SafeZonePlugin.Instance.GetSafeZone(name, true) != null)
            {
                UnturnedChat.Say(caller, "A safezone with this name already exists!", Color.red);
                return;
            }

            SafeZoneType type = SafeZoneType.RegisterSafeZoneType(typeName);
            if (type == null)
            {
                String types = "";
                foreach (String t in SafeZoneType.GetTypes())
                {
                    if (types == "")
                    {
                        types = t;
                        continue;
                    }

                    types += ", " + t;
                }
                UnturnedChat.Say(caller, "Unknown type: " + typeName + "! Available types: " + types, Color.red);
                return;
            }
            ArrayList args = new ArrayList(command);
            args.RemoveAt(0); // remove name...
            args.RemoveAt(0); // remove type...
            SafeZone safeZone = type.OnCreate(caller, name, (string[]) args.ToArray(typeof(string)));

            if (safeZone == null)
            {
                UnturnedChat.Say(caller, "Could't create safezone!", Color.red);
                return;
            }

            SafeZonePlugin.Instance.Configuration.Instance.SafeZones.Add(safeZone);
            SafeZonePlugin.Instance.Configuration.Save();
            SafeZonePlugin.Instance.OnSafeZoneCreated(safeZone);

            UnturnedChat.Say(caller, "Successfully created safezone: " + name, Color.green);
        }
        
        public AllowedCaller AllowedCaller
        {
            get
            {
                return AllowedCaller.Player;
            }
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

        public List<string> Permissions
        {
            get { return new List<string> { "safezones.create" }; }
        }
    }
}