using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Safezone.Util;
using UnityEngine;

namespace Safezone.Model.Safezone.Type
{
    [Serializable]
    public class RectangleType : SafeZoneType
    {
        public SerializablePosition Position1;
        public SerializablePosition Position2;
        private static readonly Dictionary<uint, SerializablePosition> FirstPositions = new Dictionary<uint, SerializablePosition>();
        private static readonly Dictionary<uint, SerializablePosition> SecondsPositions = new Dictionary<uint, SerializablePosition>();

        public override SafeZone OnCreate(IRocketPlayer player, string name, string[] args)
        {
            if (!HasPositionSet(player))
            {
                UnturnedChat.Say(player, "Please set pos1 (/spos1) and pos2 (/spos2) before using this command", Color.red);
                return null;
            }

            Position1 = GetPosition1(player);
            Position2 = GetPosition2(player);
            var zone = new SafeZone
            {
                Name = name,
                Owners = new List<uint> { PlayerUtil.GetId(player) },
                Type = this
            };

            return zone;
        }

        public override double GetDistance(SerializablePosition pos)
        {
            // http://wiki.unity3d.com/index.php/Distance_from_a_point_to_a_rectangle

            var xMin = Math.Min(Position1.X, Position2.X);
            var xMax = Math.Max(Position1.X, Position2.X); 
            var yMin = Math.Min(Position1.Y, Position2.Y);
            var yMax = Math.Max(Position1.Y, Position2.Y);
            var point = new Vector2(pos.X, pos.Y);

            if (pos.X < xMin)
            {
                if (pos.Y < yMin)
                { 
                    var diff = point - new Vector2(xMin, yMin);
                    return diff.magnitude;
                }
                if (pos.Y > yMax)
                { 
                    var diff = point - new Vector2(xMin, yMax);
                    return diff.magnitude;
                }
                return xMin - pos.X;
            }
            if (pos.X > xMax)
            {
                if (pos.Y < yMin)
                { 
                    var diff = point - new Vector2(xMax, yMin);
                    return diff.magnitude;
                }
                if (pos.Y > yMax)
                { 
                    var diff = point - new Vector2(xMax, yMax);
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
            /*
            SerializablePosition p1 = Position1;
            SerializablePosition p2 = Position2;

            bool b1 = p.X >= Math.Min(p1.X, p2.X);
            bool b2 = p.X <= Math.Max(p1.X, p2.X);
            bool b3 = p.Y >= Math.Min(p1.Y, p2.Y);
            bool b4 = p.Y <= Math.Max(p1.Y, p2.Y);

            return b1 && b2 && b3 && b4;
              
             
             */
            return GetDistance(p) <= 0;
        }

        public override bool OnRedefine(IRocketPlayer player, string[] args)
        {
            if (!HasPositionSet(player))
            {
                UnturnedChat.Say(player, "Please set pos1 (/spos1) and pos2 (/spos2) before using this command", Color.red);
                return false;
            }

            Position1 = GetPosition1(player);
            Position2 = GetPosition2(player);
            return true;
        }

        public static void SetPosition1(IRocketPlayer player, SerializablePosition pos)
        {
            if (FirstPositions.ContainsKey(PlayerUtil.GetId(player)))
            {
                FirstPositions[PlayerUtil.GetId(player)] = pos;
                return;
            }
            FirstPositions.Add(PlayerUtil.GetId(player), pos);
        }

        public static void SetPosition2(IRocketPlayer player, SerializablePosition pos)
        {
            if (SecondsPositions.ContainsKey(PlayerUtil.GetId(player)))
            {
                SecondsPositions[PlayerUtil.GetId(player)] = pos;
                return;
            }
            SecondsPositions.Add(PlayerUtil.GetId(player), pos);
        }

        public static bool HasPositionSet(IRocketPlayer player)
        {
            return FirstPositions.ContainsKey(PlayerUtil.GetId(player)) && SecondsPositions.ContainsKey(PlayerUtil.GetId(player));
        }

        public static SerializablePosition GetPosition1(IRocketPlayer caller)
        {
            return FirstPositions[PlayerUtil.GetId(caller)];
        }

        public static SerializablePosition GetPosition2(IRocketPlayer caller)
        {
            return SecondsPositions[PlayerUtil.GetId(caller)];
        }
    }
}