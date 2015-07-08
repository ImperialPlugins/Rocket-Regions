using System;
using System.Xml.Serialization;

namespace Safezone.Model.Flag
{
    [Serializable]
    [XmlType(AnonymousType = false)]
    [XmlInclude(typeof(PickupAllowedFlag))]
    [XmlInclude(typeof(GodmodeFlag))]
    [XmlInclude(typeof(NoZombieFlag))]
    [XmlInclude(typeof(EnterVehiclesFlag))]
    [XmlInclude(typeof(NoEnterFlag))]
    [XmlInclude(typeof(NoLeaveFlag))]
    //for now this does not support 3rd party flags
    public abstract class Flag
    {
        public String Name;
        public Object Value;
        public T GetValue<T>()
        {
            if (!(Value is T))
            {
                throw new InvalidOperationException("Can't cast " + Value.GetType().Name + " to " + typeof(T).Name);
            }
            return (T) Value;
        }

        public T GetDefaultValue<T>()
        {
            if (!(_defaultValue is T))
            {
                throw new InvalidOperationException("Can't cast " + Value.GetType().Name + " to " + typeof(T).Name);
            }
            return (T)_defaultValue;
        }

        private readonly Object _defaultValue;
        protected Flag(String name, Object defaultValue)
        {
            Name = name;
            _defaultValue = defaultValue;
        }
    }
}