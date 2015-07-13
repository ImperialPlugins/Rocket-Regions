namespace Safezone.Model.Flag
{
    public class GodmodeFlag : BoolFlag
    {
        public GodmodeFlag() : base("Godmode")
        {
        }

        public override string Description
        {
            get { return "Gives players in safezone godmode";  }
        }

        public override object DefaultValue
        {
            get { return true; }
        }
    }
}