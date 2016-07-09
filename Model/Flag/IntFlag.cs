using System;
using Rocket.API;
using Safezone.Model.Safezone;

namespace Safezone.Model.Flag
{
    public abstract class IntFlag : Flag
    {
        public override bool ParseValue(IRocketPlayer caller, SafeZone zone, string rawValue, Group group = Group.NONE)
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
    }
}