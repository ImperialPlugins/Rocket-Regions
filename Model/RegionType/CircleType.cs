using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;
using Rocket.API.Extensions;
using RocketRegions.Util;

namespace RocketRegions.Model.RegionType
{
    [Serializable]
    public class CircleType : RegionType
    {
        public double Radius;
        public SerializablePosition Center;

        public override Region OnCreate(IRocketPlayer player, string name, string[] args)
        {
            var pos = PlayerUtil.GetUnturnedPlayer(player).Position;
            var r= args.GetFloatParameter(0);
            if (r == null)
            {
                UnturnedChat.Say(player, "Usage: /screate circle <radius>", Color.red);
                return null;
            }

            Radius = r.Value;

            Center = new SerializablePosition(pos);

            var region = new Region
            {
                Name = name,
                Owners = new List<ulong> {PlayerUtil.GetId(player)}, 
                Type = this
            };

            return region;
        }

        public override double GetDistance(SerializablePosition p) => GetDistanceToCenter(p) - Radius;

        public double GetDistanceToCenter(SerializablePosition p) => Math.Sqrt(Math.Pow(p.X - Center.X, 2) + Math.Pow(p.Y - Center.Y, 2));

        public override bool IsInRegion(SerializablePosition p) => GetDistanceToCenter(p) <= Radius;

        public override bool OnRedefine(IRocketPlayer player, string[] args)
        {
            var pos = PlayerUtil.GetUnturnedPlayer(player).Position;
            if (args.Length < 1)
            {
                UnturnedChat.Say(player, "Usage: /sredefine <name> circle <radius>", Color.red);
                return false;
            }

            var r = args.GetInt32Parameter(0);

            if (r == null)
            {
                UnturnedChat.Say(player, "Usage: /sredefine <name> circle <radius>", Color.red);
                return false;
            }

            Radius = r.Value;

            Center = new SerializablePosition(pos);
            return true;
        }
    }
}
