using Rocket.Unturned;
using Rocket.Unturned.Player;
using UnityEngine;

namespace Safezone.Model.Flag
{
    public class BoolFlag : Flag
    {
        public BoolFlag(string name, object defaultValue) : base(name, defaultValue)
        {
        }

        public override bool OnSetValue(RocketPlayer caller, string[] args)
        {
            if (args.Length < 1)
            {
                RocketChat.Say(caller.CSteamID, "Usage: /sflag <region> " + Name + " <on/off/true/false/1/0>", Color.red);
                return false;
            }
            switch (args[0])
            {
                case "on":
                case "true":
                case "1":
                    Value = true;
                    return true;

                case "off":
                case "false":
                case "0":
                    Value = false;
                    return true;
            }

            RocketChat.Say(caller.CSteamID, "Usage: /sflag <region> " + Name + " <on/off/true/false/1/0>", Color.red);
            return false;
        }
    }
}