using Rocket.Unturned.Player;

namespace Safezone.Model.Flag
{
    public class BoolFlag : Flag
    {
        public BoolFlag(string name, bool defaultValue) : base(name, defaultValue)
        {
        }

        public override bool OnSetValue(RocketPlayer caller, SafeZone zone, params string[] values)
        {
            switch (values[0])
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

            return false;
        }

        public override string Usage
        {
            get { return "<on/off/true/false/1/0>"; }
        }
    }
}