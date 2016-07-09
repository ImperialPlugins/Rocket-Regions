namespace Safezone.Model
{
    public enum Group
    {
        ALL = 0,
        MEMBERS = 1,
        NONMEMBERS = 2,
        NONE = 3
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