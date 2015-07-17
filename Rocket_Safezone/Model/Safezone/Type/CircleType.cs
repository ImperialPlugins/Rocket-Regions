using System;
using Rocket.Unturned;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
using UnityEngine;

namespace Safezone.Model.Safezone.Type
{
    public class CircleType : SafeZoneType
    {
        public int? Radius;
        public SerializablePosition Center;

        public override SafeZone OnCreate(RocketPlayer player, string name, string[] args)
        {
           
            Radius = args.GetInt32Parameter(0);
            if (Radius == null)
            {
                RocketChat.Say(player.CSteamID, "Usage: /screate circle <radius>", Color.red);
                return null;
            }

            Center = new SerializablePosition(player.Position);

            SafeZone zone = new SafeZone
            {
                Name = name,
                Owner = SafeZonePlugin.GetId(player),
                Type = this
            };

            return zone;
        }

        public override double GetDistance(SerializablePosition p)
        {
            return (double) (GetDistanceToCenter(p) - Radius);
        }

        public double GetDistanceToCenter(SerializablePosition p)
        {
            return Math.Sqrt(Math.Pow(p.X - Center.X, 2) + Math.Pow(p.Y - Center.Y, 2));
        }

        public override bool IsInSafeZone(SerializablePosition p)
        {
            return GetDistance(p) <= Radius;
        }

        public override bool OnRedefine(RocketPlayer player, string[] args)
        {
            if (args.Length < 1)
            {
                RocketChat.Say(player.CSteamID, "Usage: /sredefine <name> circle <radius>", Color.red);
                return false;
            }

            Radius = args.GetInt32Parameter(0);

            if (Radius == null)
            {
                RocketChat.Say(player.CSteamID, "Usage: /sredefine <name> circle <radius>", Color.red);
                return false;
            }

            Center = new SerializablePosition(player.Position);
            return true;
        }
    }
}