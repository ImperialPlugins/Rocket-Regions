using System;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;

namespace Safezone.Model.Safezone.Type
{
    public class RectangleType : SafeZoneType
    {
        public SerializablePosition Position1;
        public SerializablePosition Position2;

        public override SafeZone OnCreate(IRocketPlayer player, String name, string[] args)
        {
            if (!SafeZonePlugin.Instance.HasPositionSet(player))
            {
                UnturnedChat.Say(player, "Please set pos1 (/spos1) and pos2 (/spos2) before using this command", Color.red);
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

        public override double GetDistance(SerializablePosition pos)
        {
            // http://wiki.unity3d.com/index.php/Distance_from_a_point_to_a_rectangle

            float xMin = Math.Min(Position1.X, Position2.X);
            float xMax = Math.Max(Position1.X, Position2.X); 
            float yMin = Math.Min(Position1.Y, Position2.Y);
            float yMax = Math.Max(Position1.Y, Position2.Y);
            Vector2 point = new Vector2(pos.X, pos.Y);

            if (pos.X < xMin)
            {
                if (pos.Y < yMin)
                { 
                    Vector2 diff = point - new Vector2(xMin, yMin);
                    return diff.magnitude;
                }
                if (pos.Y > yMax)
                { 
                    Vector2 diff = point - new Vector2(xMin, yMax);
                    return diff.magnitude;
                }
                return xMin - pos.X;
            }
            if (pos.X > xMax)
            {
                if (pos.Y < yMin)
                { 
                    Vector2 diff = point - new Vector2(xMax, yMin);
                    return diff.magnitude;
                }
                if (pos.Y > yMax)
                { 
                    Vector2 diff = point - new Vector2(xMax, yMax);
                    return diff.magnitude;
                }
                return pos.X - xMax;
            }
            if (pos.Y < yMin)
            { 
                return yMin - pos.Y;
            }
            if (pos.Y > yMax)
            { 
                return pos.Y - yMax;
            }
            return 0f;
        }

        public override bool IsInSafeZone(SerializablePosition p)
        {
            SerializablePosition p1 = Position1;
            SerializablePosition p2 = Position2;

            bool b1 = p.X >= Math.Min(p1.X, p2.X);
            bool b2 = p.X <= Math.Max(p1.X, p2.X);
            bool b3 = p.Y >= Math.Min(p1.Y, p2.Y);
            bool b4 = p.Y <= Math.Max(p1.Y, p2.Y);

            return b1 && b2 && b3 && b4;
        }

        public override bool OnRedefine(IRocketPlayer player, string[] args)
        {
            if (!SafeZonePlugin.Instance.HasPositionSet(player))
            {
                UnturnedChat.Say(player, "Please set pos1 (/spos1) and pos2 (/spos2) before using this command", Color.red);
                return false;
            }

            Position1 = SafeZonePlugin.Instance.GetPosition1(player);
            Position2 = SafeZonePlugin.Instance.GetPosition2(player);
            return true;
        }
    }
}