using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;

namespace Safezone.Commands
{
    public class CoordsCommand : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            var player = SafeZonePlugin.GetUnturnedPlayer(caller);
            Vector3 pos = player.Position;
            UnturnedChat.Say(caller, "Position: X: " + pos.x + ", Y: " + pos.y + ", Z: " + pos.z + " - Rotation: " + player.Rotation, Color.green);
        }

        public bool AllowFromConsole
        {
            get { return false; }
        }

        public string Name
        {
            get { return "coords"; }
        }

        public string Help
        {
            get { return "Shows your current position";  }
        }

        public string Syntax
        {
            get { return "";  }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public List<string> Permissions
        {
            get { return new List<string> { "safezones.coords" }; }
        }
    }
}