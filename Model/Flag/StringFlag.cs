using Rocket.API;
using Safezone.Model.Safezone;

namespace Safezone.Model.Flag
{
    public abstract class StringFlag : Flag
    {
        public override bool OnSetValue(IRocketPlayer caller, SafeZone zone, string rawValue, Group group = Group.NONE)
        {
            SetValue(rawValue, group);
            return true;
        }

        public override string Usage => "<String>";
    }
}