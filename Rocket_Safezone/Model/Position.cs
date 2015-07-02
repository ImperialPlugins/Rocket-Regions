using System;
using System.Xml.Serialization;

namespace Safezone.Model
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    public class Position
    {
        [XmlAttribute("x")]
        public float X;
        [XmlAttribute("y")]
        public float Y;
    }
}