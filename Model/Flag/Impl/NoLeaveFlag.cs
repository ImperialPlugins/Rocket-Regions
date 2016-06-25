namespace Safezone.Model.Flag.Impl
{
    public class NoLeaveFlag : BoolFlag
    {
        public NoLeaveFlag() : base("NoLeave")
        {
        }

        public override string Description => "Disallows leaving a safezone after entering it";

        public override object DefaultValue => false;
    }
}