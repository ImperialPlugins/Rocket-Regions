using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;
using UnityEngine;

namespace RocketRegions.Model.Flag.Impl
{
    public class TeleportFlag : RegionFlag
    {
        public override string Description => "Set teleport position of a region";
        public override bool ParseValue(IRocketPlayer caller, Region region, string[] command, out string valueShown, Group @group = Group.ALL)
        {
            string value = Serialize(((UnturnedPlayer)caller).Position);
            valueShown = value;
            SetValue(value, group);
            return true;
        }

        public Vector3 Deserialize(Group group)
        {
            string raw = GetValue<object>(group) as string;
            if (raw == null)
                return default(Vector3);
            string value = raw;
            if (string.IsNullOrEmpty(value))
                return default(Vector3);
            var args = value.Split(',');
            if (args.Length != 3)
                return default(Vector3);

            var x = Convert.ToSingle(args[0].Trim());
            var y = Convert.ToSingle(args[1].Trim());
            var z = Convert.ToSingle(args[2].Trim());

            return new Vector3(x, y, z);
        }

        public string Serialize(Vector3 raw) => raw.x + "," + raw.y + "," + raw.z;

        public override string Usage => "";

        public override void UpdateState(List<UnturnedPlayer> players)
        {

        }

        public override void OnRegionEnter(UnturnedPlayer player)
        {
 
        }

        public override void OnRegionLeave(UnturnedPlayer player)
        {
       
        }
    }
}