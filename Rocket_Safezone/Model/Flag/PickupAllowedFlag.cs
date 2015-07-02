using System;
using System.Xml.Serialization;

namespace Safezone.Model.Flag
{
    public class PickupAllowedFlag : Flag
    {
        public PickupAllowedFlag() : base("PickupAllowed", true)
        {
            
        }
    }
}