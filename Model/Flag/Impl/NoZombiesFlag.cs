using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace Safezone.Model.Flag.Impl
{
    public class NoZombiesFlag : BoolFlag
    {
        public override string Description => "Prevents spawning of zombies";

        public override object DefaultValue => true;

        public override bool SupportsGroups => false;
        public override void UpdateState(List<UnturnedPlayer> players)
        {
            foreach (var zombie in ZombieManager.ZombieRegions.SelectMany(t => (from zombie in t.Zombies
                                                                                let safeZone = SafeZonePlugin.Instance.GetSafeZoneAt(zombie.transform.position)
                                                                                where safeZone != null && GetValue<bool>() select zombie)))
            {
                Vector3 ragdoll =
                    (Vector3)typeof(Zombie).GetField("ragdoll", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(zombie);
                ZombieManager.sendZombieDead(zombie, ragdoll);
                Object.Destroy(zombie);
            }
        }

        public override void OnSafeZoneEnter(UnturnedPlayer player)
        {
            //do nothing
        }

        public override void OnSafeZoneLeave(UnturnedPlayer player)
        {
            //do nothing
        }
    }
}