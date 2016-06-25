using System;
using Rocket.API;
using Safezone.Model.Safezone;

namespace Safezone.Model.Flag
{
    public abstract class IntFlag : Flag
    {
        protected IntFlag(string name) : base(name)
        {
        }

        public override bool OnSetValue(IRocketPlayer caller, SafeZone zone, string rawValue, Group group)
        {
            try
            {
                SetValue(Convert.ToInt32(rawValue), group);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override string Usage => "<Number>";
    }
}