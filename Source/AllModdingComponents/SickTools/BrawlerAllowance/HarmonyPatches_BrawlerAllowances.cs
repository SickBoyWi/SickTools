using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace SickTools
{

    public static partial class HarmonyPatches
    {
        public static void HarmonyPatches_BrawlerAllowances(Harmony harmony, Type type)
        {
            harmony.Patch(AccessTools.Method(typeof(Alert_BrawlerHasRangedWeapon), nameof(Alert_BrawlerHasRangedWeapon.GetReport)),
                postfix: new HarmonyMethod(type, nameof(Alert_BrawlerHasRangedWeapon_GetReport_Post)));

            harmony.Patch(AccessTools.DeclaredPropertyGetter(typeof(ThingDef), "IsRangedWeapon"), 
                new HarmonyMethod(typeof(HarmonyPatches), "ThingDef_IsRangedWeapon_Getter", null),
                null, null);

            harmony.Patch(AccessTools.Method(typeof(ThoughtWorker_IsCarryingRangedWeapon), "CurrentStateInternal"),
                prefix: new HarmonyMethod(type, nameof(ThoughtWorker_IsCarryingRangedWeapon_Prefix)));
        }

        public static bool ThoughtWorker_IsCarryingRangedWeapon_Prefix(Pawn p, ref ThoughtState __result)
        {
            var weapon = p?.equipment?.Primary;
            if (weapon == null)
                return true;

            if (weapon.def.HasModExtension<DefModExtension_BrawlerAllowance>())
            {
                __result = ThoughtState.Inactive;
                return false;
            }

            return true;
        }

        public static bool ThingDef_IsRangedWeapon_Getter(ref bool __result, ThingDef __instance)
        {
            if (__instance.IsWeapon && __instance.HasModExtension<DefModExtension_BrawlerAllowance>())
            {
                __result = false;
                return false;
            }

            return true;
        }

        public static void Alert_BrawlerHasRangedWeapon_GetReport_Post(ref AlertReport __result)
        {
            if (!__result.active || __result.culpritsPawns is null)
                return;

            var originalList = __result.culpritsPawns;
            var filtered = new List<GlobalTargetInfo>();

            foreach (GlobalTargetInfo target in originalList)
            {
                if (target.Thing is not Pawn pawn)
                    continue;

                var primary = pawn.equipment?.Primary;
                if (primary == null || !BrawlerAllowanceUtility.IsBrawlerOKWithWeapon(primary))
                {
                    filtered.Add(pawn);
                }
            }

            if (filtered.Count == 0)
            {
                __result = AlertReport.Inactive;
            }
            else
            {
                __result = AlertReport.CulpritsAre(filtered);
            }
        }
    }
}
