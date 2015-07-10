using Rocket.Unturned.Player;

namespace Safezone.Model.Flag
{
    public class StringFlag : Flag
    {
        public StringFlag(string name, string defaultValue) : base(name, defaultValue)
        {
        }

        public override bool OnSetValue(RocketPlayer caller, SafeZone zone, params string[] values)
        {
            Value = values[0];
            return true;
        }

        public override string Usage
        {
            get { return "<String>"; }
        }
    }
}