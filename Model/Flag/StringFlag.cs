using System;
using Rocket.API;
using Safezone.Model.Safezone;

namespace Safezone.Model.Flag
{
    public abstract class StringFlag : Flag
    {
        public override bool ParseValue(IRocketPlayer caller, SafeZone zone, string rawValue, Group group = Group.NONE)
        {
            if(rawValue.Equals("null", StringComparison.CurrentCultureIgnoreCase))
                rawValue = null;
            SetValue(rawValue, group);
            return true;
        }
    }
}