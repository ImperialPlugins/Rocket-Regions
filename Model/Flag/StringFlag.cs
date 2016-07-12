using System;
using Rocket.API;
using RocketRegions.Model.Safezone;

namespace RocketRegions.Model.Flag
{
    public abstract class StringFlag : RegionFlag
    {
        public override bool ParseValue(IRocketPlayer caller, Region zone, string rawValue, Group group = Group.NONE)
        {
            if(rawValue.Equals("null", StringComparison.CurrentCultureIgnoreCase))
                rawValue = null;
            SetValue(rawValue, group);
            return true;
        }
    }
}