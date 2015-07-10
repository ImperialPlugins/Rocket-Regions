using System;
using System.Collections.Generic;
using Rocket.Unturned;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
using Rocket.Unturned.Plugins;
using Safezone.Model;
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
            
            String flagName = command.GetStringParameter(1);
            Type t = Flag.GetFlagType(flagName);
            if (t == null)
            {
                RocketChat.Say(caller.CSteamID, "Unknown flag: \"" + name + "\"", Color.red);
                return;
            }

            Flag f = zone.GetFlag(t);

            List<String> argsList = new List<string>();
            for (int i = 2; i <= command.Length -1; i++)
            {
                argsList.Add(command[i]);
            }
            string[] args = argsList.ToArray();
            if (!f.OnSetValue(caller, zone, command))
            {
                RocketChat.Say(caller.CSteamID, "Usage: /sflag " + name + " " + f.Name + " " + f.Usage, Color.red);
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