using System.Collections.Generic;
using Rocket.Unturned;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
using Safezone.Model;

namespace Safezone.Commands
{
    public class Pos1Command : IRocketCommand
    {
        public void Execute(RocketPlayer caller, string[] command)
        {
            SerializablePosition pos = new SerializablePosition(caller.Position);
            RocketChat.Say(caller.CSteamID, "First position set to: X:" + pos.X + ", Z: " + pos.Y);
            SafeZonePlugin.Instance.SetPosition1(caller, pos);
        }

        public bool RunFromConsole
        {
            get { return false; }
        }

        public string Name
        {
            get { return "safezonepos1";  }
        }

        public string Help
        {
            get { return "Set first positon for safezones"; }
        }

        public string Syntax
        {
            get { return ""; }
        }

        public List<string> Aliases
        {
            get
            {
                return new List<string> {"spos1"};
            }
        }
    }
}