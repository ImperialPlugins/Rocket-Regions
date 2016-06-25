using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Safezone.Model;
using Safezone.Model.Safezone.Type;
using Safezone.Util;

namespace Safezone.Commands
{
    public class Pos2Command : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            var rawpos = PlayerUtil.GetUnturnedPlayer(caller).Position;
            var pos = new SerializablePosition(rawpos);
            UnturnedChat.Say(caller, "Second position set to: X:" + pos.X + ", Z: " + pos.Y);
            RectangleType.SetPosition2(caller, pos);
        }


        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "safezonepos2";

        public string Help => "Set second positon for safezones";

        public string Syntax => "";

        public List<string> Aliases => new List<string> {"spos2"};

        public List<string> Permissions => new List<string> { "safezones.pos" };
    }
}