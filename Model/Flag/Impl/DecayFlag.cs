using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace RocketRegions.Model.Flag.Impl
{
    public class DecayFlag : IntFlag
    {
        public override string Description => "Decreases health of barricades and structures by 10 every x milliseconds";
        public override string Usage => "<interval in seconds>";
        public override bool SupportsGroups => false;

        private DateTime _lastRun = DateTime.Now;

        public override void UpdateState(List<UnturnedPlayer> players)
        {
            if (GetValue<int>() == 0)
                return;

            if ((DateTime.Now - _lastRun).TotalSeconds < GetValue<int>())
            {
                return;
            }

            _lastRun = DateTime.Now;

            if (StructureManager.regions != null)
            {
                foreach (var region in StructureManager.regions)
                {
                    if (region == null) continue;

                    byte index = 0;

                    var dropsList = region.drops.ToList();
                    foreach (var drop in dropsList)
                    {
                        if (dropsList.Count != region.drops.Count)
                            break;

                        if (dropsList.Count != region.structures.Count)
                            break;

                        var structure = region.structures.ToList().ElementAt(index);
                        index++;

                        if (structure?.structure == null) continue;
                        if (!Region.Type.IsInRegion(new SerializablePosition(structure.point))) continue;

                        var asset = (ItemStructureAsset)Assets.find(EAssetType.ITEM, structure.structure.id);
                        if (asset == null) continue;
                        
                        StructureManager.damage(drop.model, Vector3.zero, 10f, 1f, true);
                    }
                }
            }

            if (BarricadeManager.regions == null) return;
            {
                foreach (var region in BarricadeManager.regions)
                {
                    if (region == null) continue;

                    byte index = 0;
                    var dropsList = region.drops.ToList();
                    foreach (var drop in dropsList)
                    {
                        if (dropsList.Count != region.drops.Count)
                            break;

                        if (dropsList.Count != region.barricades.Count)
                            break;


                        var barricade = region.barricades.ToList().ElementAt(index);
                        index++;

                        if (barricade?.barricade == null) continue;
                        if (!Region.Type.IsInRegion(new SerializablePosition(barricade.point))) continue;

                        var asset = (ItemBarricadeAsset)Assets.find(EAssetType.ITEM, barricade.barricade.id);
                        if (asset == null) continue;

                        BarricadeManager.damage(drop.model, 10f, 1f, true);
                    }
                }
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