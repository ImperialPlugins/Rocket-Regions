using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Safezone.Model;

namespace Safezone.Commands
{
    public class Pos1Command : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            var rawpos = SafeZonePlugin.GetUnturnedPlayer(caller).Position;
            SerializablePosition pos = new SerializablePosition(rawpos);
            UnturnedChat.Say(caller, "First position set to: X:" + pos.X + ", Z: " + pos.Y);
            SafeZonePlugin.Instance.SetPosition1(caller, pos);
        }

        public bool AllowFromConsole
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

        public List<string> Permissions
        {
            get { return new List<string> { "safezones.pos" }; }
        }
    }
}