using Rocket.Unturned;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
using UnityEngine;

namespace Safezone.Model.Flag
{
    public class StringFlag : Flag
    {
        public StringFlag(string name, object defaultValue) : base(name, defaultValue)
        {
        }

        public override bool OnSetValue(RocketPlayer caller, string[] args)
        {
            if (args.Length < 1)
            {
                RocketChat.Say(caller.CSteamID, "Usage: /sflag <region> " + Name + " <String>", Color.red);
                return false;
            }

            Value = args.GetStringParameter(0);
            return true;
        }
    }
}