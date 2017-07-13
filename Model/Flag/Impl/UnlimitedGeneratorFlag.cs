using System.Collections.Generic;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace RocketRegions.Model.Flag.Impl
{
    public class UnlimitedGeneratorFlag : BoolFlag
    {
        public override string Description => "Infinite fuel for generators in this region";
        public override bool SupportsGroups => false;

        public override void UpdateState(List<UnturnedPlayer> players)
        {
            var generators = Object.FindObjectsOfType<InteractableGenerator>();
            foreach (var generator in generators)
            {
                var pos = generator.gameObject.transform.position;
                if (RegionsPlugin.Instance.GetRegionAt(pos) != Region)
                    continue;

                ushort oldFuel = generator.fuel;
                ushort newFuel = oldFuel;
                if (GetValueSafe())
                {
                    newFuel = (ushort) (generator.capacity + 1);
                }
                else if (generator.fuel > generator.capacity)
                {
                    newFuel = generator.capacity;
                }

                if (newFuel == oldFuel) //Prevent spamming packets to clients
                    continue;

                BarricadeManager.sendFuel(generator.transform, newFuel);
            }
        }

        public override void OnRegionEnter(UnturnedPlayer player)
        {

        }

        public override void OnRegionLeave(UnturnedPlayer player)
        {
     
        }
    }
}