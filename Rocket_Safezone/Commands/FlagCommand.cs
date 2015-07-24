using System;
using System.Collections.Generic;
using Rocket.Unturned;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
using Rocket.Unturned.Plugins;
using Safezone.Model;
using Safezone.Model.Safezone;
using Safezone.Util;
using SDG.Unturned;
using UnityEngine;
using Flag = Safezone.Model.Flag.Flag;

namespace Safezone.Commands
{
    public class FlagCommand : IRocketCommand
    {
        public void Execute(RocketPlayer caller, string[] command)
        {
            String name = command.GetStringParameter(0);
            SafeZone zone = SafeZonePlugin.Instance.GetSafeZone(name, true);
            if (zone == null)
            {
                RocketChat.Say(caller.CSteamID, "Safezone \"" + name + "\" not found", Color.red);
                return;
            }

            if (!zone.IsOwner(SafeZonePlugin.GetId(caller)) && !PermissionUtil.HasPermission(caller, "flag.override"))
            {
                RocketChat.Say(caller.CSteamID, "You're not the owner of this region!", Color.red);
                return;
            }
            
            String flagName = command.GetStringParameter(1);

            Type t = Flag.GetFlagType(flagName);
            if (t == null)
            {
                RocketChat.Say(caller.CSteamID, "Unknown flag: \"" + flagName + "\"", Color.red);
                return;
            }

            Flag f = zone.GetFlag(t);
            if (f == null)
            {
                RocketChat.Say(caller.CSteamID, "An unexpected error occurred: flag instance equals null but type was registered. Please report this", Color.red);
                return;
            }
            bool hasFlagPermission = PermissionUtil.HasPermission(caller, "flag." + flagName);
            String usage = "Usage: /sflag " + name + " " + f.Name + " " + f.Usage;
            if (command.Length == 2)
            {
                String description = f.Description;
                System.Object defaultValue = f.DefaultValue;
                System.Object value = f.Value;

                RocketChat.Say(caller.CSteamID, "Flag: " + f.Name, Color.blue);
                RocketChat.Say(caller.CSteamID, "Description: " + description, Color.blue);
                if (hasFlagPermission)
                {
                    RocketChat.Say(caller.CSteamID, "Usage: " + usage);
                    RocketChat.Say(caller.CSteamID, "Value: " + value, Color.blue);
                }
                RocketChat.Say(caller.CSteamID, "Default: " + defaultValue, Color.blue);
                return;
            }

            if (!hasFlagPermission)
            {
                RocketChat.Say(caller.CSteamID, "You don't have access to this flag!", Color.red);
                return;
            }

            List<String> argsList = new List<string>();
            for (int i = 2; i <= command.Length -1; i++)
            {
                argsList.Add(command[i]);
            }
            string[] args = argsList.ToArray();
            if (!f.OnSetValue(caller, zone, args))
            {
                RocketChat.Say(caller.CSteamID, usage, Color.red);
                return;
            }
            zone.SetFlag(f.Name, f.Value);
        }

        public bool RunFromConsole
        {
            get { return false; }
        }

        public string Name
        {
            get { return "safezoneflag"; }
        }

        public string Help
        {
            get { return "Modify flags for safezones"; }
        }

        public string Syntax
        {
            get { return "<region> <flag> <value>"; }
        }

        public List<string> Aliases
        {
            get { return new List<string> { "sflag" }; }
        }
    }
}