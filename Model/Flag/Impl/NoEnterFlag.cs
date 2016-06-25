namespace Safezone.Model.Flag.Impl
{
    public class NoEnterFlag : BoolFlag
    {
        public NoEnterFlag() : base("NoEnter")
        {
        }

        public override string Description => "Disallows entering the safezone";

        public override object DefaultValue => false;
    }
}