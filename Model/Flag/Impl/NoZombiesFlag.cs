namespace Safezone.Model.Flag.Impl
{
    public class NoZombiesFlag : BoolFlag
    {
        public NoZombiesFlag() : base("NoZombies")
        {
        }

        public override string Description => "Prevents spawning of zombies";

        public override object DefaultValue => true;

        public override bool SupportsGroups => false;
    }
}