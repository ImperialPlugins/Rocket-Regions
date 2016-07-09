using Rocket.API;
using Safezone.Model.Safezone;

namespace Safezone.Model.Flag
{
    public abstract class BoolFlag : Flag
    {
        public override bool ParseValue(IRocketPlayer caller, SafeZone zone, string rawValue, Group group = Group.NONE)
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