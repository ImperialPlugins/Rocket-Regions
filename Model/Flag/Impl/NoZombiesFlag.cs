using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rocket.Unturned.Player;
using Safezone.Model.Safezone;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace Safezone.Model.Flag.Impl
{
    public class NoZombiesFlag : BoolFlag
    {
        public override string Description => "Prevents spawning of zombies";

        public override bool SupportsGroups => false;
        public override void UpdateState(List<UnturnedPlayer> players)
        {
            if (ZombieManager.ZombieRegions == null) return;

            foreach (ZombieRegion t in ZombieManager.ZombieRegions)
            {
                if (t?.Zombies == null) continue;
                foreach (var zombie in t.Zombies)
                {
                    // ReSharper disable once MergeSequentialChecks
                    if(zombie == null || zombie.transform?.position == null) continue;

                    SafeZone safeZone = SafeZonePlugin.Instance?.GetSafeZoneAt(zombie.transform.position);
                    if (safeZone == null || !GetValue<bool>()) continue;
                    Vector3 ragdoll = (Vector3)typeof(Zombie).GetField("ragdoll", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(zombie);

                    ZombieManager.sendZombieDead(zombie, ragdoll);
                    Object.Destroy(zombie);
                }
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