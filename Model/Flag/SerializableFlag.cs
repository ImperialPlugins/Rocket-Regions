using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Safezone.Model.Flag
{
    [Serializable]
    [XmlType(TypeName = "Flag")]
    public class SerializableFlag
    {
        [XmlAttribute("name")]
        public string Name;
        public object Value; 

        public List<GroupValue> GroupValues; 
    }

    public class GroupValue
    {
        [XmlAttribute]
        public string GroupName;
        [XmlAttribute]
        public object Value;
    }
}