using System;
using System.Collections;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Commands;
using Safezone.Model.Safezone;
using Safezone.Util;
using UnityEngine;

namespace Safezone.Commands
{
    public class RedefineCommand : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                UnturnedChat.Say(caller, "Usage: /sredefine <name> [...]", Color.red);
                return;
            }

            String name = command.GetStringParameter(0);

            SafeZone zone = SafeZonePlugin.Instance.GetSafeZone(name, true);
            if (zone == null)
            {
                UnturnedChat.Say(caller, "Safezone \"" + name + "\" not found", Color.red);
                return;
            }

            if (!zone.IsOwner(caller) && !PermissionUtil.HasPermission(caller, "redefine.override"))
            {
                UnturnedChat.Say(caller, "You're not the owner of this region!", Color.red);
                return;
            }

            ArrayList args = new ArrayList(command);
            args.RemoveAt(0);

            if (zone.Type.OnRedefine(PlayerUtil.GetUnturnedPlayer(caller), (string[])args.ToArray(typeof(string))))
            {
                SafeZonePlugin.Instance.Configuration.Instance.SafeZones.Remove(zone);
                SafeZonePlugin.Instance.Configuration.Save(SafeZonePlugin.Instance.Configuration.Instance);
                SafeZonePlugin.Instance.OnSafeZoneRemoved(zone);
                SafeZonePlugin.Instance.Configuration.Instance.SafeZones.Add(zone);
                SafeZonePlugin.Instance.Configuration.Save(SafeZonePlugin.Instance.Configuration.Instance);
                SafeZonePlugin.Instance.OnSafeZoneCreated(zone);

                UnturnedChat.Say(caller, "Successfully redefined safezone: " + name, Color.green);
                return;
            }

            UnturnedChat.Say(caller, "Redefine of safezone: " + name + " failed.", Color.red);
        }

        public bool AllowFromConsole
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

        public List<string> Permissions
        {
            get { return new List<string> { "safezones.redefine" }; }
        }
    }
}