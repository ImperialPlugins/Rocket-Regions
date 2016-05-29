namespace Safezone.Model.Flag
{
    public class EnterVehiclesFlag : BoolFlag
    {
        public EnterVehiclesFlag() : base("EnterVehicles")
        {
        }

        public override string Description
        {
            get { return "Allows entering vehicles in a safezone"; }
        }

        public override object DefaultValue
        {
            get { return true; }
        }
    }
}