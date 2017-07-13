using System.Collections.Generic;
using Rocket.API;
using Rocket.API.Extensions;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using RocketRegions.Model.Flag.Impl;
using RocketRegions.Util;
using UnityEngine;

namespace RocketRegions.Commands
{
    public class TeleportCommand : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 1)
            {
                this.SendUsage(caller);
                return;
            }

            var player = (UnturnedPlayer) caller;
            if (command.Length > 1)
            {
                string playerName = command.GetStringParameter(1);
                player = UnturnedPlayer.FromName(playerName);
                if (player == null)
                {
                    UnturnedChat.Say(caller, "Player \"" + playerName + "\" not found!", Color.red);
                    return;
                }
            }

            string regionName = command.GetStringParameter(0);
            var region = RegionsPlugin.Instance.GetRegion(regionName);
            if (region == null)
            {
                UnturnedChat.Say(caller, "Region \"" + regionName + "\" not found!", Color.red);
                return;
            }

            var flag = region.GetFlag<TeleportFlag>();
            if (flag == null)
            {
                UnturnedChat.Say(caller, "Region \"" + regionName + "\" does not have Teleport flag setup!", Color.red);
                return;
            }

            var pos = flag.Deserialize(region.GetGroup(caller));
            if (pos == default(Vector3))
            {
                UnturnedChat.Say(caller,
                    "Region \"" + regionName + "\" does not have teleport positions for your group!", Color.red);
                return;
            }

            UnturnedChat.Say(caller, "Teleporting...", Color.green);
            player.Teleport(pos, player.Rotation);
        }
        
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "regionteleport";

        public string Help => "Teleport to a region";

        public string Syntax => "<region> [player]";

        public List<string> Aliases => new List<string> {"rteleport"};

        public List<string> Permissions => new List<string> {"regions.command.teleport"};

        #endregion Properties
    }
}