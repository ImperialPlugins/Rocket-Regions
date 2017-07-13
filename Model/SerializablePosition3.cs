using System.Xml.Serialization;
using UnityEngine;

namespace RocketRegions.Model
{
    public class SerializablePosition3 : SerializablePosition
    {
        [XmlAttribute("z")]
        public float Z;

        public SerializablePosition3() : base()
        {
            Z = 0.0f;
        }

        public SerializablePosition3(Vector2 vec)
        {
            X = vec.x;
            Y = 0;
            Z = vec.y;
        }

        public SerializablePosition3(Vector3 vec)
        {
            X = vec.x;
            Y = vec.y;
            Z = vec.z;
        }

        public Vector3 ToVector3() => new Vector3(X, Y, Z);
    }
}