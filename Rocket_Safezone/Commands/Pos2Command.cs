using System.Collections.Generic;
using Rocket.Unturned;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
using Safezone.Model;

namespace Safezone.Commands
{
    public class Pos2Command : IRocketCommand
    {
        public void Execute(RocketPlayer caller, string[] command)
        {
            SerializablePosition pos = new SerializablePosition(caller.Position);
            RocketChat.Say(caller.CSteamID, "Second position set to: X:" + pos.X + ", Z: " + pos.Y);
            SafeZonePlugin.Instance.SetPosition2(caller, pos);
        }

        public bool RunFromConsole
        {
            get { return false; }
        }

        public string Name
        {
            get { return "safezonepos2"; }
        }

        public string Help
        {
            get { return "Set second positon for safezones"; }
        }

        public string Syntax
        {
            get { return ""; }
        }

        public List<string> Aliases
        {
            get { return new List<string> {"spos2"};  }
        } 
    }
}