using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Safezone.Model;
using Safezone.Model.Flag;
using Safezone.Util;
using UnityEngine;
using Rocket.API.Extensions;

namespace Safezone.Commands
{
    public class FlagCommand : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            var name = command.GetStringParameter(0);
            var zone = SafeZonePlugin.Instance.GetSafeZone(name, true);
            if (zone == null)
            {
                UnturnedChat.Say(caller, "Safezone \"" + name + "\" not found", Color.red);
                return;
            }

            if (!zone.IsOwner(caller) && !PermissionUtil.HasPermission(caller, "flag.override"))
            {
                UnturnedChat.Say(caller, "You're not the owner of this region!", Color.red);
                return;
            }
            
            var flagName = command.GetStringParameter(1);

            var t = Flag.GetFlagType(flagName);
            if (t == null)
            {
                UnturnedChat.Say(caller, "Unknown flag: \"" + flagName + "\"", Color.red);
                return;
            }

            var f = zone.GetFlag(t);
            if (f == null)
            {
                UnturnedChat.Say(caller, "An unexpected error occurred: flag instance equals null but type was registered. Please report this", Color.red);
                return;
            }

            var hasFlagPermission = PermissionUtil.HasPermission(caller, "flag." + flagName);
            var usage = "Usage: /sflag " + name + " " + f.Name + " " + f.Usage + " [group]";
            if (command.Length == 2)
            {
                var description = f.Description;
                var defaultValue = f.DefaultValue;
                var value = f.Value;

                UnturnedChat.Say(caller, "Flag: " + f.Name, Color.blue);
                UnturnedChat.Say(caller, "Description: " + description, Color.blue);
                if (hasFlagPermission)
                {
                    UnturnedChat.Say(caller, usage);
                    UnturnedChat.Say(caller, "Value: " + value, Color.blue);
                }
                UnturnedChat.Say(caller, "Default Value: " + defaultValue, Color.blue);
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
                group = GroupUtil.GetGroup(command.GetStringParameter(3));
                if (group == Group.NONE)
                {
                    UnturnedChat.Say(caller, "Unknown group: " + command.GetStringParameter(3) + "!", Color.red);
                    return;
                }
            }
            if (!f.OnSetValue(caller, zone, command.GetStringParameter(2), group))
            {
                UnturnedChat.Say(caller, usage, Color.red);
                return;
            }
            zone.SetFlag(f.Name, f.Value, f.GroupValues);
            UnturnedChat.Say(caller, $"Flag has been set to: {f.Value}!", Color.green);
        }
        
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "safezoneflag";

        public string Help => "Modify flags for safezones";

        public string Syntax => "<region> <flag> <value>";

        public List<string> Aliases => new List<string> { "sflag" };

        public List<string> Permissions => new List<string> { "safezones.flag" };
    }
}