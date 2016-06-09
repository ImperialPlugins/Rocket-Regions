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
        public String Name;
        [XmlAttribute("value")]
        public Object Value; 
        [XmlAttribute("groupvalues")]
        public List<GroupValue> GroupValues; 
    }

    public class GroupValue
    {
        [XmlAttribute]
        public string Key;
        [XmlAttribute]
        public object Value;
    }

}