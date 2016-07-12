using System;
using System.Xml.Serialization;
using UnityEngine;

namespace RocketRegions.Model
{
    [Serializable]
    public class SerializablePosition
    {
        public SerializablePosition()
        {
            X = 0;
            Y = 0;
        }

        public SerializablePosition(Vector2 vec)
        {
            X = vec.x;
            Y = vec.y;
        }
        
        public SerializablePosition(Vector3 vec)
        {
            X = vec.x;
            Y = vec.z;
        }

        [XmlAttribute("x")]
        public float X;
        [XmlAttribute("y")]
        public float Y;
    }
}