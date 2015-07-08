using System;
using System.Collections;
using Rocket.Unturned;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
using UnityEngine;

namespace Safezone.Model
{
    public class CircleType : SafeZoneType
    {
        public int? Radius;
        public Position Center;

        public override SafeZone Create(RocketPlayer player, string name, ArrayList args)
        {
            if(args.Count < 1)
            {
                RocketChat.Say(player.CSteamID, "Usage: /screate circle <radius>", Color.red);
                return null;
            }
            string[] command = (string[]) args.ToArray(typeof(string));
            
            Radius = command.GetInt32Parameter(0);
            if (Radius == null)
            {
                RocketChat.Say(player.CSteamID, "Usage: /screate circle <radius>", Color.red);
                return null;
            }

            Center = new Position {X = player.Position.x, Y = player.Position.y};

            SafeZone zone = new SafeZone
            {
                Name = name,
                Owner = SafeZonePlugin.GetId(player),
                Type = this
            };

            return zone;
        }

        public override bool IsInSafeZone(Position p)
        {
            double distance = Math.Sqrt(Math.Pow(p.X - Center.X, 2) + Math.Pow(p.Y - Center.Y, 2));
            return distance <= Radius;
        }

        public override bool Redefine(RocketPlayer player, ArrayList args)
        {
            if (args.Count < 1)
            {
                RocketChat.Say(player.CSteamID, "Usage: /sredefine circle <radius>", Color.red);
                return false;
            }

            string[] command = (string[])args.ToArray(typeof(string));

            Radius = command.GetInt32Parameter(0);
            if (Radius == null)
            {
                RocketChat.Say(player.CSteamID, "Usage: /screate circle <radius>", Color.red);
                return false;
            }

            Center = new Position { X = player.Position.x, Y = player.Position.y };
            return true;
        }
    }
}