using System;

namespace RocketRegions.Model
{
    [Flags]
    public enum Group
    {
        ALL = 0,
        MEMBERS = 1,
        NONMEMBERS = 2,
        OWNERS = 3,
    }

    public static class GroupExtensions
    {
        public static Group GetGroup(string name)
        {
            switch (name.Trim().ToUpperInvariant())
            {
                case "ALL":
                    return Group.ALL;
                case "OWNERS":
                case "OWNER":
                    return Group.OWNERS;
                case "MEMBERS":
                case "MEMBER":
                    return Group.MEMBERS;
                case "NONMEMBERS":
                case "NON-MEMBERS":
                case "NONMEMBER":
                case "NON-MEMBER":
                    return Group.NONMEMBERS;
            }

            return Group.ALL;
        }

        public static string GetSerializableName(this Group group) => @group.ToString().ToLower();
    }
}