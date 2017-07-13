using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using RocketRegions.Model;
using RocketRegions.Model.RegionType;
using RocketRegions.Util;

namespace RocketRegions.Commands
{
    public class PosCommand : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 1 || (command[0] != "1" && command[0] != "2"))
            {
                this.SendUsage(caller);
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
        
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "regionpos";

        public string Help => "Set position bounds for rectangular region";

        public string Syntax => "<1/2>";

        public List<string> Aliases => new List<string> { "rpos" };

        public List<string> Permissions => new List<string> { "regions.command.pos" };
        
        #endregion
    }
}