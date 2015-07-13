namespace Safezone.Model.Flag
{
    public class NoZombieFlag : BoolFlag
    {
        public NoZombieFlag() : base("NoZombie")
        {
        }

        public override string Description
        {
            get { return "Prevents spawning of zombies"; }
        }

        public override object DefaultValue
        {
            get { return true; }
        }
    }
}