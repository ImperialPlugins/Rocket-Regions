using System;

namespace RocketRegions.Model
{
    [Flags]
    public enum Group
    {
        NONE = 0,
        ALL = 1,
        MEMBERS = 2,
        NONMEMBERS = 3,
        OWNERS = 4,
    }

    public static class GroupExtensions
    {
        public static Group GetGroup(string name)
        {
            name = name.Trim().ToLower();
            switch (name)
            {
                case "all":
                    return Group.ALL;
                case "owners":
                case "owner":
                    return Group.OWNERS;
                case "members":
                case "member":
                    return Group.MEMBERS;
                case "nonmembers":
                case "non-members":
                case "nonmember":
                case "non-member":
                    return Group.NONMEMBERS;
            }

            return Group.NONE;
        }

        public static string GetSerializableName(this Group group)
        {
            return group.ToString().ToLower();
        }
    }
}