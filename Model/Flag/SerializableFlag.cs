using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RocketRegions.Model.Flag
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
        public string GroupName;
        public object Value;
    }
}