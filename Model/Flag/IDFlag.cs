using System;
using Rocket.API;
using Rocket.API.Extensions;

namespace RocketRegions.Model.Flag
{
    public abstract class IDFlag : RegionFlag
    {
        public override string Usage => "<ID>";

        public override bool ParseValue(IRocketPlayer caller, Region region, string[] command, out string shownValue, Group group = Group.ALL)
        {
            shownValue = null;
            int? value = command.GetUInt16Parameter(0);
            try
            {
                shownValue = value.Value.ToString();
                SetValue(Convert.ToInt32(value.Value), group);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}