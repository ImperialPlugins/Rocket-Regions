using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;
using RocketRegions.Model;
using RocketRegions.Model.Flag;
using RocketRegions.Util;

namespace RocketRegions.Commands
{
    public class FlagCommand : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 2)
            {
                this.SendUsage(caller);
                return;
            }
            var name = command[0];
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

            var flagName = command[1];

            var t = RegionFlag.GetFlagType(flagName);
            if (t == null)
            {
                UnturnedChat.Say(caller, "Unknown flag: \"" + flagName + "\"", Color.red);
                return;
            }

            var f = region.GetFlag(t);
            if (f == null)
            {
                f = (RegionFlag)Activator.CreateInstance(t);
                f.Name = RegionFlag.GetFlagName(t);
            }

            var hasFlagPermission = PermissionUtil.HasPermission(caller, "flag." + flagName);
            var usage = "Usage: /rflag " + name + " " + f.Name + " " + f.Usage + " [-g <group>]";
            if (command.Length == 3 && command[2].Equals("--help"))
            {
                var description = f.Description;
                var value = f.GetValue<object>();

                UnturnedChat.Say(caller, "Flag: " + f.Name, Color.blue);
                UnturnedChat.Say(caller, "Description: " + description, Color.blue);
                if (!hasFlagPermission) return;
                UnturnedChat.Say(caller, usage);
                UnturnedChat.Say(caller, "Value: " + (value ?? "null"), Color.blue);
                return;
            }

            if (!hasFlagPermission)
            {
                UnturnedChat.Say(caller, "You don't have access to this flag!", Color.red);
                return;
            }


            f.Region = region;

            Group group = Group.ALL;
            List<string> args = new List<string>();
            bool isValue = false;
            bool groupSet = false;
            foreach (string arg in command.Skip(2))
            {
                if (!groupSet)
                {
                    if (isValue)
                    {
                        if (f.SupportsGroups)
                            group = GroupExtensions.GetGroup(arg);
                        groupSet = true;
                        isValue = false;
                        continue;
                    }
                    if (arg.ToLower().Equals("-g") || arg.ToLower().Equals("--group"))
                    {
                        if (!f.SupportsGroups)
                            UnturnedChat.Say(caller, "Warning: Flag does not support groups", Color.red);
                        isValue = true;
                        continue;
                    }
                }

                args.Add(arg);
            }

            string shownValue;
            if (isValue || !f.ParseValue(caller, region, args.ToArray(), out shownValue, group))
            {
                UnturnedChat.Say(caller, usage, Color.red);
                return;
            }

            region.SetFlag(f.Name, f.Value, f.GroupValues);

            if (shownValue != null)
            {
                string msg = $"Flag has been set to: {shownValue}";
                if (f.SupportsGroups)
                    msg += $" for group {group}!";
                UnturnedChat.Say(caller,  msg, Color.green);
            }

            RegionsPlugin.Instance.Configuration.Save();
        }

        public class FlagGroup
        {
            public Group Group { get; set; }
        }
        
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "regionflag";

        public string Help => "Set flags for regions";

        public string Syntax => "<region> <flag> [value] [-g <group>]";

        public List<string> Aliases => new List<string> { "rflag" };

        public List<string> Permissions => new List<string> { "regions.command.flag" };
        
        #endregion
    }
}