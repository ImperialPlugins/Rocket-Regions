using System.Collections;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;
using Rocket.API.Extensions;
using RocketRegions.Model.Flag;
using RocketRegions.Model.RegionType;
using RocketRegions.Util;

namespace RocketRegions.Commands
{
    public class CreateCommand : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 2)
            {
                this.SendUsage(caller);
                return;
            }

            var name = command.GetStringParameter(0);
            var typeName = command.GetStringParameter(1).ToLower();

            if (RegionsPlugin.Instance.GetRegion(name, true) != null)
            {
                UnturnedChat.Say(caller, "A region with this name already exists!", Color.red);
                return;
            }

            var type = RegionType.RegisterRegionType(typeName);
            if (type == null)
            {
                var types = "";
                foreach (var t in RegionType.GetTypes())
                {
                    if (types == "")
                    {
                        types = t;
                        continue;
                    }

                    types += ", " + t;
                }
                UnturnedChat.Say(caller, "Unknown type: " + typeName + "! Available types: " + types, Color.red);
                return;
            }
            var args = new ArrayList(command);
            args.RemoveAt(0); // remove name...
            args.RemoveAt(0); // remove type...
            var region = type.OnCreate(caller, name, (string[]) args.ToArray(typeof(string)));

            if (region == null)
            {
                UnturnedChat.Say(caller, "Could't create region!", Color.red);
                return;
            }

            RegionsPlugin.Instance.Regions.Add(region);
            RegionsPlugin.Instance.OnRegionCreated(region);

            region.SetFlag("EnterMessage", "Entered region: {0}", new List<GroupValue>());
            region.SetFlag("LeaveMessage", "Left region: {0}", new List<GroupValue>());

            RegionsPlugin.Instance.Configuration.Save();
            UnturnedChat.Say(caller, "Successfully created region: " + name, Color.green);
        }
        
        #region Properties
        
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "regioncreate";

        public string Help => "Create a region";

        public string Syntax => "<name> <type> [args]";

        public List<string> Aliases => new List<string> { "rcreate" };

        public List<string> Permissions => new List<string> { "regions.command.create" };
        
        #endregion
    }
}