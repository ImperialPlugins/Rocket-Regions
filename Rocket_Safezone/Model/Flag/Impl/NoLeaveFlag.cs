namespace Safezone.Model.Flag
{
    public class NoLeaveFlag : BoolFlag
    {
        public NoLeaveFlag() : base("NoLeave")
        {
        }

        public override string Description
        {
            get { return "Disallows leaving a safezone after entering it"; }
        }

        public override object DefaultValue
        {
            get { return false; }
        }
    }
}