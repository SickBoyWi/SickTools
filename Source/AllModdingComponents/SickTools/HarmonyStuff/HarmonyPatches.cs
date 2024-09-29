using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SickTools
{
    [StaticConstructorOnStartup]
    public static partial class HarmonyPatches
    {
        //For alternating fire on some weapons
        public static Dictionary<Thing, int> AlternatingFireTracker = new Dictionary<Thing, int>();

        static HarmonyPatches()
        {
            var harmony = new Harmony("sicktools.sickboy.main");
            var type = typeof(HarmonyPatches);

            HarmonyPatches_StartWithHediff(harmony, type);
        }

        [Conditional("DEBUGLOG")]
        private static void DebugMessage(string s)
        {
            Log.Message(s);
        }

        private static string ToString(ExtraDamage ed)
        {
            return $"(def={ed.def}, amount={ed.amount}, armorPenetration={ed.armorPenetration}, chance={ed.chance})";
        }



    }
}
