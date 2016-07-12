using System;

namespace RocketRegions.Util
{
    public static class TypeUtil
    {
        // http://stackoverflow.com/a/2742288
        public static bool IsSameOrSubclass(this Type potentialDescendant, Type potentialBase)
        {
            return potentialDescendant.IsSubclassOf(potentialBase)
                   || potentialDescendant == potentialBase;
        } 
    }
}