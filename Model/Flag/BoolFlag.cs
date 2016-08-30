using System;
using System.Linq;
using Rocket.API;
using RocketRegions.Model;

namespace RocketRegions.Model.Flag
{
    public abstract class BoolFlag : RegionFlag
    {
        public bool GetValueSafe(Group group)
        {
            var val = GetValue<bool?>(group);
            if (val == null)
                SetValue(false, group);
            return val != null && val.Value;
        }

        public bool GetValueSafe()
        {
            var val = GetValue<bool?>();
            if (val == null)
                Value = false;
            return val != null && val.Value;
        }

        public override bool ParseValue(IRocketPlayer caller, Region region, string[] command, out string shownValue, Group group = Group.ALL)
        {
            shownValue = null;
            switch (command.FirstOrDefault()?.ToLower().Trim())
            {
                case "on":
                case "true":
                case "1":
                    shownValue = "true";
                    SetValue(true, group);
                    return true;

                case "off":
                case "false":
                case "0":
                    shownValue = "false";
                    SetValue(false, group);
                    return true;
            }
            

            return false;
        }

        public override string Usage => "<on/off/true/false/1/0>";
    }
}