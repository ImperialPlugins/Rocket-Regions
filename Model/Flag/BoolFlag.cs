using Rocket.API;
using RocketRegions.Model;

namespace RocketRegions.Model.Flag
{
    public abstract class BoolFlag : RegionFlag
    {
        public override bool ParseValue(IRocketPlayer caller, Region region, string rawValue, Group group = Group.NONE)
        {
            switch (rawValue.ToLower().Trim())
            {
                case "on":
                case "true":
                case "1":
                    SetValue(true, group);
                    return true;

                case "off":
                case "false":
                case "0":
                    SetValue(false, group);
                    return true;
            }

            return false;
        }

        public override string Usage => "<on/off/true/false/1/0>";
    }
}