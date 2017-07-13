using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;
using Rocket.API.Extensions;
using RocketRegions.Util;

namespace RocketRegions.Commands
{
    public class RemoveCommand : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                this.SendUsage(caller);
                return;
            }

            var name = command.GetStringParameter(0);
            var region = RegionsPlugin.Instance.GetRegion(name);
            if (region == null)
            {
                UnturnedChat.Say(caller, "Region \"" + name + "\" not found", Color.red);
                return;
            }

            if (!region.IsOwner(caller) && !PermissionUtil.HasPermission(caller, "remove.override"))
            {
                UnturnedChat.Say(caller, "You're not the owner of this region!", Color.red);
                return;
            }

            RegionsPlugin.Instance.Regions.Remove(region);
            RegionsPlugin.Instance.Configuration.Save();
            RegionsPlugin.Instance.OnRegionRemoved(region);

            UnturnedChat.Say(caller, "Successfully removed region: " + name, Color.green);
        }
        
        #region Properties

        public string Name => "regionremove";

        public string Help => "Remove a region";

        public string Syntax => "<name>";

        public List<string> Aliases => new List<string> { "rremove" };

        public List<string> Permissions => new List<string> { "regions.command.remove" };

        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        
        #endregion
    }
}