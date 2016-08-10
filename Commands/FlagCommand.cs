using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;
using Rocket.API.Extensions;
using Rocket.Core.Logging;
using RocketRegions.Model;
using RocketRegions.Model.Flag;
using RocketRegions.Util;

namespace RocketRegions.Commands
{
    public class FlagCommand : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            var name = command.GetStringParameter(0);
            var region = RegionsPlugin.Instance.GetRegion(name, true);
            if (region == null)
            {
                UnturnedChat.Say(caller, "Region \"" + name + "\" not found", Color.red);
                return;
            }

            if (!region.IsOwner(caller) && !PermissionUtil.HasPermission(caller, "flag.override"))
            {
                UnturnedChat.Say(caller, "You're not the owner of this region!", Color.red);
                return;
            }
            
            var flagName = command.GetStringParameter(1);

            var t = RegionFlag.GetFlagType(flagName);
            if (t == null)
            {
                UnturnedChat.Say(caller, "Unknown flag: \"" + flagName + "\"", Color.red);
                return;
            }

            var f = region.GetFlag(t);
            if (f == null)
            {
                UnturnedChat.Say(caller, "An unexpected error occurred: flag instance equals null but type was registered. Please report this", Color.red);
                return;
            }

            var hasFlagPermission = PermissionUtil.HasPermission(caller, "flag." + flagName);
            var usage = "Usage: /rflag " + name + " " + f.Name + " " + f.Usage + " [group]";
            if (command.Length == 2)
            {
                var description = f.Description;
                var value = f.GetValue<object>();

                UnturnedChat.Say(caller, "Flag: " + f.Name, Color.blue);
                UnturnedChat.Say(caller, "Description: " + description, Color.blue);
                if (hasFlagPermission)
                {
                    UnturnedChat.Say(caller, usage);
                    UnturnedChat.Say(caller, "Value: " + (value ?? "null"), Color.blue);
                }
                return;
            }

            if (!hasFlagPermission)
            {
                UnturnedChat.Say(caller, "You don't have access to this flag!", Color.red);
                return;
            }

            var group = Group.NONE;
            if (command.Length == 4)
            {
                group = GroupExtensions.GetGroup(command.GetStringParameter(3));
                if (group == Group.NONE)
                {
                    UnturnedChat.Say(caller, "Unknown group: " + command.GetStringParameter(3) + "!", Color.red);
                    return;
                }
            }

            f.Region = region;

            string shownValue;
            if (!f.ParseValue(caller, region, command.GetStringParameter(2), out shownValue, group))
            {
                UnturnedChat.Say(caller, usage, Color.red);
                return;
            }

            region.SetFlag(f.Name, f.Value, f.GroupValues);
            UnturnedChat.Say(caller, $"Flag has been set to: {shownValue}!", Color.green);
        }
        
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "regionflag";

        public string Help => "Set flags for regions";

        public string Syntax => "<region> <flag> [value] [-g <group>]";

        public List<string> Aliases => new List<string> { "rflag" };

        public List<string> Permissions => new List<string> { "regions.command.flag" };
    }
}