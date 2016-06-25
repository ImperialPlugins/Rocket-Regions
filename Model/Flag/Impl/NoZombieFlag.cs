namespace Safezone.Model.Flag.Impl
{
    public class NoZombieFlag : BoolFlag
    {
        public NoZombieFlag() : base("NoZombie")
        {
        }

        public override string Description => "Prevents spawning of zombies";

        public override object DefaultValue => true;

        public override bool SupportsGroups => false;
    }
}