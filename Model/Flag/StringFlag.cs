using System;
using Rocket.API;

namespace RocketRegions.Model.Flag
{
    public abstract class StringFlag : RegionFlag
    {
        public override bool ParseValue(IRocketPlayer caller, Region region, string[] command, out string shownValue, Group group = Group.ALL)
        {
            shownValue = null;
            if (command.Length == 0)
                return false;

            string label = "";
            foreach (string s in command)
            {
                if (label != "")
                    label += " ";
                label += s;
            }

            if(label.Equals("null", StringComparison.CurrentCultureIgnoreCase))
                label = null;
            shownValue = label;
            SetValue(label, group);
            return true;
        }
    }
}