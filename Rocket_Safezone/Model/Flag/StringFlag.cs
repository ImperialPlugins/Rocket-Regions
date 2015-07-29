using Rocket.API;
using Safezone.Model.Safezone;

namespace Safezone.Model.Flag
{
    public abstract class StringFlag : Flag
    {
        protected StringFlag(string name) : base(name)
        {

        }

        public override bool OnSetValue(IRocketPlayer caller, SafeZone zone, params string[] values)
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