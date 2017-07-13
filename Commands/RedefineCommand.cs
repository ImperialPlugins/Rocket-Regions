using System.Collections;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;
using Rocket.API.Extensions;
using RocketRegions.Util;

namespace RocketRegions.Commands
{
    public class RedefineCommand : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                this.SendUsage(caller);
                return;
            }

            var name = command.GetStringParameter(0);

            var region = RegionsPlugin.Instance.GetRegion(name, true);
            if (region == null)
            {
                UnturnedChat.Say(caller, "Region \"" + name + "\" not found", Color.red);
                return;
            }

            if (!region.IsOwner(caller) && !PermissionUtil.HasPermission(caller, "redefine.override"))
            {
                UnturnedChat.Say(caller, "You're not the owner of this region!", Color.red);
                return;
            }

            var args = new ArrayList(command);
            args.RemoveAt(0);

            if (region.Type.OnRedefine(PlayerUtil.GetUnturnedPlayer(caller), (string[])args.ToArray(typeof(string))))
            {
                RegionsPlugin.Instance.Regions.Remove(region);
                RegionsPlugin.Instance.Configuration.Save();
                RegionsPlugin.Instance.OnRegionRemoved(region);
                RegionsPlugin.Instance.Regions.Add(region);
                RegionsPlugin.Instance.Configuration.Save();
                RegionsPlugin.Instance.OnRegionCreated(region);

                UnturnedChat.Say(caller, "Successfully redefined region: " + name, Color.green);
                return;
            }

            UnturnedChat.Say(caller, "Redefine of region: " + name + " failed.", Color.red);
        }

        #region Properties
        
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "regionredefine";

        public string Help => "Redefine a region";

        public string Syntax => "<name> [type args]";

        public List<string> Aliases => new List<string> { "rredefine" };

        public List<string> Permissions => new List<string> { "regions.command.redefine" };
        
        #endregion
    }
}