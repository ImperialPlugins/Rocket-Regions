using Rocket.Unturned;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
using UnityEngine;

namespace Safezone.Model.Flag
{
    public class IntFlag : Flag
    {
        public IntFlag(string name, object defaultValue) : base(name, defaultValue)
        {
        }

        public override bool OnSetValue(RocketPlayer caller, string[] args)
        {
            if (args.Length < 1)
            {
                RocketChat.Say(caller.CSteamID, "Usage: /sflag <region> " + Name + " <on/off/true/false/1/0>", Color.red);
                return false;
            }

            int? value = args.GetInt32Parameter(0);
            if (value == null)
            {
                RocketChat.Say(caller.CSteamID, "Usage: /sflag <region> " + Name + " <on/off/true/false/1/0>", Color.red);
                return false;
            }

            Value = value;
            return true;
        }
    }
}