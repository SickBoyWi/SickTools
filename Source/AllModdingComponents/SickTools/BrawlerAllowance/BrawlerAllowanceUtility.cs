using Verse;

namespace SickTools
{
    public static class BrawlerAllowanceUtility
    {
        public static bool IsBrawlerOKWithWeapon(ThingWithComps weapon)
        {
            return weapon?.def?.GetModExtension<DefModExtension_BrawlerAllowance>() != null;
        }
    }
}
