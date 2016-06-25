namespace Safezone.Model.Flag.Impl
{
    public class EnterVehiclesFlag : BoolFlag
    {
        public EnterVehiclesFlag() : base("EnterVehicles")
        {
        }

        public override string Description => "Allows entering vehicles in a safezone";

        public override object DefaultValue => true;
    }
}