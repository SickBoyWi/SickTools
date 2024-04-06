using RimWorld;
using Verse;

namespace SickAbilityUser
{
    [DefOf]
    public static class AbilityDefOf
    {
        public static JobDef CastAbilitySelf;
        public static JobDef CastAbilityVerb;

        static AbilityDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(AbilityDefOf));
    }
}
