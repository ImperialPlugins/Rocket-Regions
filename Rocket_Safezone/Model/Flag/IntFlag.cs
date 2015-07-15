using System;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
using Safezone.Model.Safezone;

namespace Safezone.Model.Flag
{
    public abstract class IntFlag : Flag
    {
        protected IntFlag(string name) : base(name)
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