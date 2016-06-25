namespace Safezone.Model.Flag.Impl
{
    public class GodmodeFlag : BoolFlag
    {
        public GodmodeFlag() : base("Godmode")
        {
        }

        public override string Description => "Gives players in safezone godmode";

        public override object DefaultValue => true;
    }
}