using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Safezone.Model;
using Safezone.Model.Safezone.Type;
using Safezone.Util;
using UnityEngine;

namespace Safezone.Commands
{
    public class PosCommand : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 1 || (command[0] != "1" && command[0] != "2"))
            {
                UnturnedChat.Say(caller, "Use /spos 1 or /spos 2!", Color.red);
                return;
            }
            
            var rawpos = PlayerUtil.GetUnturnedPlayer(caller).Position;
            var pos = new SerializablePosition(rawpos);

            if (command[0] == "1")
            {
                UnturnedChat.Say(caller, "First position set to: X:" + pos.X + ", Z: " + pos.Y);
                RectangleType.SetPosition1(caller, pos);
            }
            else
            {
                UnturnedChat.Say(caller, "Second position set to: X:" + pos.X + ", Z: " + pos.Y);
                RectangleType.SetPosition2(caller, pos);
            }
        }
        
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        
        public string Name => "safezonepos";

        public string Help => "Set first positon for safezones";

        public string Syntax => "<1/2>";

        public List<string> Aliases => new List<string> {"spos"};

        public List<string> Permissions => new List<string> { "safezones.pos" };
    }
}