using System.Collections.Generic;
using Rocket.Unturned;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
using UnityEngine;

namespace Safezone.Commands
{
    public class CoordsCommand : IRocketCommand
    {
        public void Execute(RocketPlayer caller, string[] command)
        {
            Vector3 pos = caller.Position;
            RocketChat.Say(caller.CSteamID, "Position: X: " + pos.x + ", Y: " + pos.y + ", Z: " + pos.z + " - Rotation: " + caller.Rotation, Color.green);
        }

        public bool RunFromConsole
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
    }
}