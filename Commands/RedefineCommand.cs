using System.Collections;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Safezone.Util;
using UnityEngine;
using Rocket.API.Extensions;

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

            var name = command.GetStringParameter(0);

            var zone = SafeZonePlugin.Instance.GetSafeZone(name, true);
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

            var args = new ArrayList(command);
            args.RemoveAt(0);

            if (zone.Type.OnRedefine(PlayerUtil.GetUnturnedPlayer(caller), (string[])args.ToArray(typeof(string))))
            {
                SafeZonePlugin.Instance.SafeZones.Remove(zone);
                SafeZonePlugin.Instance.Configuration.Save();
                SafeZonePlugin.Instance.OnSafeZoneRemoved(zone);
                SafeZonePlugin.Instance.SafeZones.Add(zone);
                SafeZonePlugin.Instance.Configuration.Save();
                SafeZonePlugin.Instance.OnSafeZoneCreated(zone);

                UnturnedChat.Say(caller, "Successfully redefined safezone: " + name, Color.green);
                return;
            }

            UnturnedChat.Say(caller, "Redefine of safezone: " + name + " failed.", Color.red);
        }


        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "safezoneredefine";

        public string Help => "Redefine a safezone";

        public string Syntax => "<name>";

        public List<string> Aliases => new List<string> { "sredefine" };

        public List<string> Permissions => new List<string> { "safezones.redefine" };
    }
}