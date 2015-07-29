using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Safezone.Model;

namespace Safezone.Commands
{
    public class Pos2Command : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            var rawpos = SafeZonePlugin.GetUnturnedPlayer(caller).Position;
            SerializablePosition pos = new SerializablePosition(rawpos);
            UnturnedChat.Say(caller, "Second position set to: X:" + pos.X + ", Z: " + pos.Y);
            SafeZonePlugin.Instance.SetPosition2(caller, pos);
        }

        public bool AllowFromConsole
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

        public List<string> Permissions
        {
            get { return new List<string> { "safezones.pos" }; }
        }
    }
}