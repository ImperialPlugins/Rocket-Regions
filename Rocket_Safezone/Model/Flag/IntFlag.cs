using System;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;

namespace Safezone.Model.Flag
{
    public class IntFlag : Flag
    {
        public IntFlag(string name, int defaultValue) : base(name, defaultValue)
        {
        }

        public override bool OnSetValue(RocketPlayer caller, SafeZone zone, params string[] values)
        {
            try
            {
                Value = Convert.ToInt32(values[0]);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override string Usage
        {
            get { return "<Number>"; }
        }
    }
}