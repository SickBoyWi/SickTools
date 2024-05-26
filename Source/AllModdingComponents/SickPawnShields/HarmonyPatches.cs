using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace SickPawnShields
{
    /// <summary>
    /// Harmony patches, these aren't pretty.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var harmony = new Harmony("sicktools.shields");
            var type = typeof(HarmonyPatches);

            harmony.Patch(AccessTools.Method(typeof(PawnGenerator), "GenerateGearFor"),
                postfix: new HarmonyMethod(type, nameof(Patch_PawnGenerator_GenerateGearFor)));

            harmony.Patch(AccessTools.Method(typeof(PawnRenderer), nameof(PawnRenderer.RenderPawnAt)),
                postfix: new HarmonyMethod(type, nameof(Patch_PawnRenderer_RenderPawnAt)));

            harmony.Patch(AccessTools.Method(typeof(Pawn), nameof(Pawn.DropAndForbidEverything)),
                postfix: new HarmonyMethod(type, nameof(Patch_Pawn_DropAndForbidEverything)));

            harmony.Patch(AccessTools.Method(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.CheckForStateChange)),
                postfix: new HarmonyMethod(type, nameof(Patch_Pawn_HealthTracker_CheckForStateChance)));
            harmony.Patch(AccessTools.Method(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.PreApplyDamage)),
                prefix: new HarmonyMethod(type, nameof(Patch_Pawn_HealthTracker_PreApplyDamage)));

            harmony.Patch(AccessTools.Method(typeof(Pawn_EquipmentTracker), nameof(Pawn_EquipmentTracker.MakeRoomFor)),
                postfix: new HarmonyMethod(type, nameof(Patch_Pawn_EquipmentTracker_MakeRoomFor)));

            harmony.Patch(AccessTools.Method(typeof(StatWorker), nameof(StatWorker.GetValueUnfinalized)),
                transpiler: new HarmonyMethod(type, nameof(Transpiler_StatWorker_GetValueUnfinalized)));
            harmony.Patch(AccessTools.Method(typeof(StatWorker), nameof(StatWorker.GetExplanationUnfinalized)),
                transpiler: new HarmonyMethod(type, nameof(Transpiler_StatWorker_GetExplanationUnfinalized)));
        }

        public static void Patch_PawnGenerator_GenerateGearFor(Pawn pawn, ref PawnGenerationRequest request)
        {
            PawnShieldGenerator.TryGenerateShieldFor(pawn, request);
        }


        /*





        Error in static constructor of SickPawnShields.HarmonyPatches: System.TypeInitializationException: The type initializer for 'SickPawnShields.HarmonyPatches' threw an exception. ---> System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation. ---> System.InvalidCastException: Specified cast is not valid.
          at SickPawnShields.HarmonyPatches.Transpiler_StatWorker_GetValueUnfinalized (System.Collections.Generic.IEnumerable`1[T] instructions, System.Reflection.MethodBase method, System.Reflection.Emit.ILGenerator ilGen) [0x00084] in <e82eae6ccc8e4a4ab1830170db359d79>:0 
          at (wrapper managed-to-native) System.Reflection.MonoMethod.InternalInvoke(System.Reflection.MonoMethod,object,object[],System.Exception&)
          at System.Reflection.MonoMethod.Invoke (System.Object obj, System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, System.Object[] parameters, System.Globalization.CultureInfo culture) [0x00032] in <eae584ce26bc40229c1b1aa476bfa589>:0 
           --- End of inner exception stack trace ---
        [Ref 32FE3C1D]
         at System.Reflection.MonoMethod.Invoke (System.Object obj, System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, System.Object[] parameters, System.Globalization.CultureInfo culture) [0x00048] in <eae584ce26bc40229c1b1aa476bfa589>:0 
         at System.Reflection.MethodBase.Invoke (System.Object obj, System.Object[] parameters) [0x00000] in <eae584ce26bc40229c1b1aa476bfa589>:0 
         at HarmonyLib.CodeTranspiler+<>c__DisplayClass11_0.<GetResult>b__0 (System.Reflection.MethodInfo transpiler) [0x0004b] in <abec11463bc04855a5322a0a868aeb22>:0 
         at System.Collections.Generic.List`1[T].ForEach (System.Action`1[T] action) [0x00024] in <eae584ce26bc40229c1b1aa476bfa589>:0 
         at HarmonyLib.CodeTranspiler.GetResult (System.Reflection.Emit.ILGenerator generator, System.Reflection.MethodBase method) [0x00020] in <abec11463bc04855a5322a0a868aeb22>:0 
         at HarmonyLib.MethodBodyReader.FinalizeILCodes (HarmonyLib.Emitter emitter, System.Collections.Generic.List`1[T] transpilers, System.Collections.Generic.List`1[T] endLabels, System.Boolean& hasReturnCode, System.Boolean& methodEndsInDeadCode) [0x0014d] in <abec11463bc04855a5322a0a868aeb22>:0 
         at HarmonyLib.MethodCopier.Finalize (HarmonyLib.Emitter emitter, System.Collections.Generic.List`1[T] endLabels, System.Boolean& hasReturnCode, System.Boolean& methodEndsInDeadCode) [0x00000] in <abec11463bc04855a5322a0a868aeb22>:0 
         at HarmonyLib.MethodPatcher.CreateReplacement (System.Collections.Generic.Dictionary`2[System.Int32,HarmonyLib.CodeInstruction]& finalInstructions) [0x00352] in <abec11463bc04855a5322a0a868aeb22>:0 
         at HarmonyLib.PatchFunctions.UpdateWrapper (System.Reflection.MethodBase original, HarmonyLib.PatchInfo patchInfo) [0x00059] in <abec11463bc04855a5322a0a868aeb22>:0 
         at HarmonyLib.PatchProcessor.Patch () [0x000fc] in <abec11463bc04855a5322a0a868aeb22>:0 
         at HarmonyLib.Harmony.Patch (System.Reflection.MethodBase original, HarmonyLib.HarmonyMethod prefix, HarmonyLib.HarmonyMethod postfix, HarmonyLib.HarmonyMethod transpiler, HarmonyLib.HarmonyMethod finalizer) [0x0002a] in <abec11463bc04855a5322a0a868aeb22>:0 
         at <0x1e73d2b00d0 + 0x007c2> <unknown method>
           --- End of inner exception stack trace ---
        [Ref CF6A66]
         at (wrapper managed-to-native) System.RuntimeMethodHandle.GetFunctionPointer(intptr)
         at System.RuntimeMethodHandle.GetFunctionPointer () [0x00000] in <eae584ce26bc40229c1b1aa476bfa589>:0 
         at HarmonyLib.HarmonySharedState.GetRealMethod (System.Reflection.MethodInfo method, System.Boolean useReplacement) [0x00046] in <abec11463bc04855a5322a0a868aeb22>:0 
         at HarmonyLib.HarmonySharedState.GetStackFrameMethod (System.Diagnostics.StackFrame frame, System.Boolean useReplacement) [0x00015] in <abec11463bc04855a5322a0a868aeb22>:0 
         at HarmonyLib.Harmony.GetOriginalMethodFromStackframe (System.Diagnostics.StackFrame frame) [0x0000e] in <abec11463bc04855a5322a0a868aeb22>:0 
         at HarmonyMod.ExceptionTools.AddHarmonyFrames (System.Text.StringBuilder sb, System.Diagnostics.StackTrace trace) [0x00032] in <4a9a522fe62f478db3085dd636c372de>:0 
         at HarmonyMod.ExceptionTools.ExtractHarmonyEnhancedStackTrace (System.Diagnostics.StackTrace trace, System.Boolean forceRefresh, System.Int32& hash) [0x00040] in <4a9a522fe62f478db3085dd636c372de>:0 
         at HarmonyMod.Environment_GetStackTrace_Patch.Prefix (System.Exception e, System.Boolean needFileInfo, System.String& __result) [0x0001d] in <4a9a522fe62f478db3085dd636c372de>:0 
        UnityEngine.StackTraceUtility:ExtractStackTrace ()
        Verse.Log:Error (string)
        Verse.StaticConstructorOnStartupUtility:CallAll ()
        Verse.PlayDataLoader/<>c:<DoPlayLoad>b__4_4 ()
        Verse.LongEventHandler:ExecuteToExecuteWhenFinished ()
        Verse.LongEventHandler:UpdateCurrentAsynchronousEvent ()
        Verse.LongEventHandler:LongEventsUpdate (bool&)
        (wrapper dynamic-method) MonoMod.Utils.DynamicMethodDefinition:Verse.Root.Update_Patch1 (Verse.Root)
        Verse.Root_Entry:Update ()





        */

        public static IEnumerable<CodeInstruction> Transpiler_StatWorker_GetValueUnfinalized(IEnumerable<CodeInstruction> instructions,
            MethodBase method, ILGenerator ilGen)
        {
            // Transforms following:
            //  if (pawn.equipment != null && pawn.equipment.Primary != null)
            //  {
            //      result += StatOffsetFromGear(pawn.equipment.Primary, stat);
            //  }
            // into:
            //  if (pawn.equipment != null)
            //  {
            //      if (pawn.equipment.Primary != null)
            //          result += StatOffsetFromGear(pawn.equipment.Primary, stat);
            //      StatWorkerInjection_AddShieldValue(ref result, pawn.equipment, stat);
            //  }

            var instructionList = instructions.AsList();
            var locals = new Locals(method, ilGen);

            var pawnEquipmentIndex = instructionList.FindSequenceIndex(
                instruction => locals.IsLdloc(instruction),
                instruction => instruction.LoadsField(pawnEquipmentField),
                instruction => !instruction.IsBrfalse());
            //Log.Message((Label)instructionList[pawnEquipmentIndex + 3].operand);
            var targetLabel = (Label)instructionList[pawnEquipmentIndex + 3].operand;

            var resultStoreIndex = instructionList.FindIndex(pawnEquipmentIndex + 3,
                instruction => locals.IsStloc(instruction, out var local) && local.LocalType == typeof(float));

            var insertionIndex = instructionList.FindIndex(pawnEquipmentIndex + 3,
                instruction => instruction.labels.Contains(targetLabel));

            var labelsToTransfer = instructionList.GetRange(pawnEquipmentIndex + 3, insertionIndex - (pawnEquipmentIndex + 3))
                .Where(instruction => instruction.operand is Label)
                .Select(instruction => (Label)instruction.operand);
            instructionList.SafeInsertRange(insertionIndex, new[]
            {
                locals.FromStloc(instructionList[resultStoreIndex]).ToLdloca(), // &result
                instructionList[pawnEquipmentIndex].Clone(), // pawn...
                instructionList[pawnEquipmentIndex + 1].Clone(), // ...equipment
                new CodeInstruction(OpCodes.Ldarg_0), // this...
                new CodeInstruction(OpCodes.Ldfld, statWorkerStatField), // ...stat
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HarmonyPatches), nameof(StatWorkerInjection_AddShieldValue))),
            }, labelsToTransfer);

            return instructionList;


            /* PRE 1.5

            var instructionList = instructions.AsList();
            var locals = new Locals(method, ilGen);

            var pawnEquipmentIndex = instructionList.FindSequenceIndex(
                instruction => locals.IsLdloc(instruction),
                instruction => instruction.LoadsField(pawnEquipmentField),
                instruction => instruction.IsBrfalse());
            var targetLabel = (Label)instructionList[pawnEquipmentIndex + 2].operand;

            var resultStoreIndex = instructionList.FindIndex(pawnEquipmentIndex + 3,
                instruction => locals.IsStloc(instruction, out var local) && local.LocalType == typeof(float));

            var insertionIndex = instructionList.FindIndex(pawnEquipmentIndex + 3,
                instruction => instruction.labels.Contains(targetLabel));

            var labelsToTransfer = instructionList.GetRange(pawnEquipmentIndex + 3, insertionIndex - (pawnEquipmentIndex + 3))
                .Where(instruction => instruction.operand is Label)
                .Select(instruction => (Label)instruction.operand);
            instructionList.SafeInsertRange(insertionIndex, new[]
            {
                locals.FromStloc(instructionList[resultStoreIndex]).ToLdloca(), // &result
                instructionList[pawnEquipmentIndex].Clone(), // pawn...
                instructionList[pawnEquipmentIndex + 1].Clone(), // ...equipment
                new CodeInstruction(OpCodes.Ldarg_0), // this...
                new CodeInstruction(OpCodes.Ldfld, statWorkerStatField), // ...stat
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HarmonyPatches), nameof(StatWorkerInjection_AddShieldValue))),
            }, labelsToTransfer);

            return instructionList;
            */
        }

        private static void StatWorkerInjection_AddShieldValue(ref float result, Pawn_EquipmentTracker equipment, StatDef stat)
        {
            var shield = equipment.GetShield();
            if (shield != null)
            {
                result += shield.def.equippedStatOffsets.GetStatOffsetFromList(stat);
            }
        }

        public static IEnumerable<CodeInstruction> Transpiler_StatWorker_GetExplanationUnfinalized(IEnumerable<CodeInstruction> instructions,
            MethodBase method, ILGenerator ilGen)
        {
            // Transforms following:
            //  if (pawn.equipment != null && pawn.equipment.Primary != null)
            //  {
            //      result += StatOffsetFromGear(pawn.equipment.Primary, stat);
            //  }
            // into:
            //  if (pawn.equipment != null)
            //  {
            //      if (pawn.equipment.Primary != null)
            //          result += StatOffsetFromGear(pawn.equipment.Primary, stat);
            //      StatWorkerInjection_AddShieldValue(ref result, pawn.equipment, stat);
            //  }

            var instructionList = instructions.AsList();
            var locals = new Locals(method, ilGen);

            var pawnEquipmentIndex = instructionList.FindSequenceIndex(
                instruction => locals.IsLdloc(instruction),
                instruction => instruction.LoadsField(pawnEquipmentField),
                instruction => !instruction.IsBrfalse());
            //Log.Message((Label)instructionList[pawnEquipmentIndex + 3].operand);
            var targetLabel = (Label)instructionList[pawnEquipmentIndex + 3].operand;

            var resultStoreIndex = instructionList.FindIndex(pawnEquipmentIndex + 3,
                instruction => locals.IsStloc(instruction, out var local) && local.LocalType == typeof(float));

            var insertionIndex = instructionList.FindIndex(pawnEquipmentIndex + 3,
                instruction => instruction.labels.Contains(targetLabel));

            var labelsToTransfer = instructionList.GetRange(pawnEquipmentIndex + 3, insertionIndex - (pawnEquipmentIndex + 3))
                .Where(instruction => instruction.operand is Label)
                .Select(instruction => (Label)instruction.operand);
            instructionList.SafeInsertRange(insertionIndex, new[]
            {
                locals.FromStloc(instructionList[resultStoreIndex]).ToLdloca(), // &result
                instructionList[pawnEquipmentIndex].Clone(), // pawn...
                instructionList[pawnEquipmentIndex + 1].Clone(), // ...equipment
                new CodeInstruction(OpCodes.Ldarg_0), // this...
                new CodeInstruction(OpCodes.Ldfld, statWorkerStatField), // ...stat
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HarmonyPatches), nameof(StatWorkerInjection_AddShieldValue))),
            }, labelsToTransfer);

            return instructionList;
        }

        private static void StatWorkerInjection_BuildShieldString(StringBuilder stringBuilder, Pawn_EquipmentTracker equipment, StatDef stat)
        {
            var shield = equipment.GetShield();
            if (shield != null && GearAffectsStats(shield.def, stat))
            {
                stringBuilder.AppendLine(InfoTextLineFromGear(shield, stat));
            }
        }

        private static readonly FieldInfo pawnEquipmentField = AccessTools.Field(typeof(Pawn), nameof(Pawn.equipment));
        private static readonly FieldInfo statWorkerStatField = AccessTools.Field(typeof(StatWorker), "stat");
        private static readonly Func<ThingDef, StatDef, bool> GearAffectsStats =
            (Func<ThingDef, StatDef, bool>)AccessTools.Method(typeof(StatWorker), "GearAffectsStat")
                .CreateDelegate(typeof(Func<ThingDef, StatDef, bool>));
        private static readonly Func<Thing, StatDef, string> InfoTextLineFromGear =
            (Func<Thing, StatDef, string>)AccessTools.Method(typeof(StatWorker), "InfoTextLineFromGear")
                .CreateDelegate(typeof(Func<Thing, StatDef, string>));

        public static void Patch_PawnRenderer_RenderPawnAt(Pawn ___pawn, ref Vector3 drawLoc)
        {
            //Render shield.
            if (___pawn.GetShield() is ThingWithComps shield)
            {
                var bodyVector = drawLoc;

                var shieldComp = shield.GetCompShield();
                bodyVector += shieldComp.ShieldProps.renderProperties.OffsetFromRotation(___pawn.Rotation);

                shieldComp.RenderShield(bodyVector, ___pawn.Rotation, ___pawn, shield);
            }
        }

        public static void Patch_Pawn_EquipmentTracker_MakeRoomFor(Pawn_EquipmentTracker __instance, Pawn ___pawn, ThingWithComps eq)
        {
            var shieldComp = eq.GetCompShield();
            if (shieldComp != null)
            {
                //Unequip any existing shield.
                var shield = __instance.GetShield();
                if (shield != null)
                {
                    if (__instance.TryDropEquipment(shield, out var thingWithComps, ___pawn.Position, true))
                    {
                        thingWithComps?.SetForbidden(false, true);
                    }
                    else
                    {
                        Log.Error(___pawn + " couldn't make room for shield " + eq);
                    }
                }
            }
        }

        public static void Patch_Pawn_DropAndForbidEverything(Pawn __instance)
        {
            if (__instance.InContainerEnclosed && __instance.GetShield() is ThingWithComps shield)
            {
                __instance.equipment.TryTransferEquipmentToContainer(shield, __instance.holdingOwner);
            }
        }

        public static void Patch_Pawn_HealthTracker_CheckForStateChance(Pawn_HealthTracker __instance, Pawn ___pawn)
        {
            if (!__instance.Downed && !__instance.capacities.CapableOf(PawnCapacityDefOf.Manipulation) && ___pawn.GetShield() is ThingWithComps shield)
            {
                if (___pawn.kindDef.destroyGearOnDrop)
                {
                    ___pawn.equipment.DestroyEquipment(shield);
                }
                else if (___pawn.InContainerEnclosed)
                {
                    ___pawn.equipment.TryTransferEquipmentToContainer(shield, ___pawn.holdingOwner);
                }
                else if (___pawn.SpawnedOrAnyParentSpawned)
                {
                    ___pawn.equipment.TryDropEquipment(shield, out var _, ___pawn.PositionHeld);
                }
                else if (___pawn.IsCaravanMember())
                {
                    ___pawn.equipment.Remove(shield);
                    if (!___pawn.inventory.innerContainer.TryAdd(shield))
                    {
                        shield.Destroy();
                    }
                }
                else
                {
                    ___pawn.equipment.DestroyEquipment(shield);
                }
            }
        }

        public static bool Patch_Pawn_HealthTracker_PreApplyDamage(Pawn ___pawn, ref DamageInfo dinfo, out bool absorbed)
        {
            absorbed = false;

            if (___pawn == null)
                return true;

            //Notify of agressor
            var violence = new DamageInfo(dinfo);
            violence.SetAmount(0);
            ___pawn.mindState.Notify_DamageTaken(violence);

            //Try getting equipped shield.
            var shield = ___pawn.GetShield();
            if (shield == null)
                return true;

            var shieldComp = shield.GetCompShield();

            var shieldSound = shieldComp.BlockSound ?? shieldComp.ShieldProps.defaultSound;
            var discardShield = false;

            //Determine if it is a melee or ranged attack.
            if (shieldComp.ShieldProps.canBlockRanged &&
                dinfo.Instigator != null &&
                !dinfo.Instigator.Position.AdjacentTo8WayOrInside(___pawn.Position) ||
                dinfo.Def.isExplosive)
            {
                //Ranged
                absorbed = shieldComp.AbsorbDamage(___pawn, dinfo, true);
                if (absorbed)
                    shieldSound?.PlayOneShot(___pawn);
                //MoteMaker.ThrowText(dinfo.Instigator.DrawPos, dinfo.Instigator.Map, "Ranged absorbed=" + absorbed);

                if (shieldComp.IsBroken)
                {
                    discardShield = true;
                }
            }
            else if (shieldComp.ShieldProps.canBlockMelee &&
                dinfo.Instigator != null &&
                dinfo.Instigator.Position.AdjacentTo8WayOrInside(___pawn.Position))
            {
                //Melee
                absorbed = shieldComp.AbsorbDamage(___pawn, dinfo, false);
                if (absorbed)
                    shieldSound?.PlayOneShot(___pawn);
                //MoteMaker.ThrowText(dinfo.Instigator.DrawPos, dinfo.Instigator.Map, "Melee absorbed=" + absorbed);

                if (shieldComp.IsBroken)
                {
                    discardShield = true;
                }
            }

            if (shieldComp.ShieldProps.useFatigue && ___pawn.health.hediffSet.GetFirstHediffOfDef(ShieldHediffDefOf.ShieldFatigue) is Hediff hediff &&
                hediff.Severity >= hediff.def.maxSeverity)
            {
                discardShield = true;
            }

            //Discard shield either from damage or fatigue.
            if (shieldComp.ShieldProps.canBeAutoDiscarded && discardShield)
            {
                if (___pawn.equipment.TryDropEquipment(shield, out var thingWithComps, ___pawn.Position, true))
                {
                    thingWithComps?.SetForbidden(false, true);
                }
                else
                {
                    Log.Error(___pawn + " couldn't discard shield " + shield);
                }
            }

            return !absorbed;
        }
    }
}
