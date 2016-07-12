using System.Collections;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;
using Rocket.API.Extensions;
using RocketRegions.Model.Flag;
using RocketRegions.Model.Safezone.Type;
using RocketRegions.Model.Flag.Impl;

namespace RocketRegions.Commands
{
    public class CreateCommand : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 2)
            {
                UnturnedChat.Say(caller, "Usage: /screate <name> <type>", Color.red);
                return;
            }

            var name = command.GetStringParameter(0);
            var typeName = command.GetStringParameter(1).ToLower();

            if (RegionsPlugin.Instance.GetSafeZone(name, true) != null)
            {
                UnturnedChat.Say(caller, "A region with this name already exists!", Color.red);
                return;
            }

            var type = RegionType.RegisterSafeZoneType(typeName);
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
            var safeZone = type.OnCreate(caller, name, (string[]) args.ToArray(typeof(string)));

            if (safeZone == null)
            {
                UnturnedChat.Say(caller, "Could't create region!", Color.red);
                return;
            }

            RegionsPlugin.Instance.Regions.Add(safeZone);
            RegionsPlugin.Instance.OnRegionCreated(safeZone);

            safeZone.SetFlag("EnterMessage", "Entered Safezone: {0}", new List<GroupValue>());
            safeZone.SetFlag("LeaveMessage", "Left Safezone: {0}", new List<GroupValue>());

            RegionsPlugin.Instance.Configuration.Save();
            UnturnedChat.Say(caller, "Successfully created region: " + name, Color.green);
        }
        
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "regioncreate";

        public string Help => "Create a region";

        public string Syntax => "<name>";

        public List<string> Aliases => new List<string> { "rcreate" };

        public List<string> Permissions => new List<string> { "regions.create" };
    }
}