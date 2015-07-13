namespace Safezone.Model.Flag
{
    public class NoEnterFlag : BoolFlag
    {
        public NoEnterFlag() : base("NoEnter")
        {
        }

        public override string Description
        {
            get { return "Disallows entering the safezone"; }
        }

        public override object DefaultValue
        {
            get { return false; }
        }
    }
}