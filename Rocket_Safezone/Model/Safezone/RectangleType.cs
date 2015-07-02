using System;
using System.Collections;
using Rocket.Unturned;
using Rocket.Unturned.Player;

namespace Safezone.Model
{
    public class RectangleType : SafeZoneType
    {
        public Position Position1;
        public Position Position2;

        public override string GetName()
        {
            return "Rectangle";
        }

        public override SafeZone Create(RocketPlayer player, String name, ArrayList args)
        {
            if (!SafeZonePlugin.Instance.HasPositionSet(player))
            {
                RocketChat.Say(player.CSteamID, "Please set pos1 (/spos1) and pos2 (/spos2) before using this command", UnityEngine.Color.red);
                return null;
            }

            Position1 = SafeZonePlugin.Instance.GetPosition1(player);
            Position2 = SafeZonePlugin.Instance.GetPosition2(player);
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
            Position p1 = Position1;
            Position p2 = Position2;

            //float x2 = p1.X;
            //float y2 = p4.X;
            //float x3 = p4.X;
            //float y3 = p1.Y;

            //Position p2 = new Position() {X = x2, Y = y2};
            //Position p3 = new Position() {X = x3, Y = y3 };

            bool b1 = p.X >= Math.Min(p1.X, p2.X);
            bool b2 = p.X <= Math.Max(p1.X, p2.X);
            bool b3 = p.Y >= Math.Min(p1.Y, p2.Y);
            bool b4 = p.Y <= Math.Max(p1.Y, p2.Y);

            return b1 && b2 && b3 && b4;
        }

        public override bool Redefine(RocketPlayer player, ArrayList args)
        {
            if (!SafeZonePlugin.Instance.HasPositionSet(player))
            {
                RocketChat.Say(player.CSteamID, "Please set pos1 (/spos1) and pos2 (/spos2) before using this command", UnityEngine.Color.red);
                return false;
            }

            Position1 = SafeZonePlugin.Instance.GetPosition1(player);
            Position2 = SafeZonePlugin.Instance.GetPosition2(player);
            return true;
        }
    }
}