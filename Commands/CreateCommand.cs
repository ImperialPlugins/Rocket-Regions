using System.Collections;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Safezone.Model.Safezone.Type;
using UnityEngine;
using Rocket.API.Extensions;
using Safezone.Model.Flag;
using Safezone.Model.Flag.Impl;

namespace Safezone.Commands
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

            if (SafeZonePlugin.Instance.GetSafeZone(name, true) != null)
            {
                UnturnedChat.Say(caller, "A safezone with this name already exists!", Color.red);
                return;
            }

            var type = SafeZoneType.RegisterSafeZoneType(typeName);
            if (type == null)
            {
                var types = "";
                foreach (var t in SafeZoneType.GetTypes())
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
                UnturnedChat.Say(caller, "Could't create safezone!", Color.red);
                return;
            }

            SafeZonePlugin.Instance.SafeZones.Add(safeZone);
            SafeZonePlugin.Instance.OnSafeZoneCreated(safeZone);

            safeZone.SetFlag("EnterMessage", "Entered Safezone: {0}", new List<GroupValue>());
            safeZone.SetFlag("LeaveMessage", "Left Safezone: {0}", new List<GroupValue>());

            SafeZonePlugin.Instance.Configuration.Save();
            UnturnedChat.Say(caller, "Successfully created safezone: " + name, Color.green);
        }
        
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "safezonecreate";

        public string Help => "Create a safezone";

        public string Syntax => "<name>";

        public List<string> Aliases => new List<string> { "screate" };

        public List<string> Permissions => new List<string> { "safezones.create" };
    }
}