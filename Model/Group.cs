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

            return Group.ALL;
        }

        public static string GetSerializableName(this Group group)
        {
            return group.ToString().ToLower();
        }
    }
}