﻿using System;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace SickAbilityUser
{
    public class AbilityDef : Def
    {
        public Type abilityClass = typeof(PawnAbility);
        public VerbProperties_Ability MainVerb;
        public PassiveEffectProperties PassiveProps;

        [Unsaved] public Texture2D uiIcon = BaseContent.BadTex;

        public string uiIconPath;

        public override void PostLoad()
        {
            base.PostLoad();
            LongEventHandler.ExecuteWhenFinished(() =>
            {
                if (!uiIconPath.NullOrEmpty())
                    uiIcon = ContentFinder<Texture2D>.Get(uiIconPath);
                //else if (DrawMatSingle != null && DrawMatSingle != BaseContent.BadMat)
                //    uiIcon = (Texture2D)DrawMatSingle.mainTexture;
            });
        }

        public Job GetJob(AbilityTargetCategory cat, LocalTargetInfo target)
        {
            return JobMaker.MakeJob(cat switch
            {
                AbilityTargetCategory.TargetSelf => AbilityDefOf.CastAbilitySelf,
                _ => AbilityDefOf.CastAbilityVerb,
            }, target);
        }

        public virtual string GetDescription()
        {
            var coolDesc = GetBasics();
            var AoEDesc = GetAoEDesc();
            //var postDesc = PostAbilityVerbDesc();
            var desc = new StringBuilder();
            desc.AppendLine(description);
            if (coolDesc.Length != 0)
                desc.AppendLine(coolDesc);
            if (AoEDesc.Length != 0)
                desc.AppendLine(AoEDesc);
            //if (postDesc.Length != 0) desc.AppendLine(postDesc);
            return desc.ToString();
        }

        public virtual string GetAoEDesc()
        {
            var def = MainVerb;
            if (def != null)
                if (def.TargetAoEProperties != null)
                {
                    var s = new StringBuilder();
                    s.AppendLine(StringsToTranslate.ST_AoEProperties);
                    if (def.TargetAoEProperties.targetClass == typeof(Pawn))
                        s.AppendLine("\t" + StringsToTranslate.ST_TargetClass + StringsToTranslate.ST_TargetClass);
                    else
                        s.AppendLine("\t" + StringsToTranslate.ST_TargetClass +
                                     def.TargetAoEProperties.targetClass.ToString().CapitalizeFirst());
                    s.AppendLine("\t" + "Range".Translate() + ": " + def.TargetAoEProperties.range);
                    s.AppendLine("\t" + StringsToTranslate.ST_TargetClass + def.TargetAoEProperties.friendlyFire);
                    s.AppendLine("\t" + StringsToTranslate.ST_AoEMaxTargets + def.TargetAoEProperties.maxTargets);
                    if (def.TargetAoEProperties.startsFromCaster)
                        s.AppendLine("\t" + StringsToTranslate.ST_AoEStartsFromCaster);
                    return s.ToString();
                }
            return "";
        }

        public string GetBasics()
        {
            var def = MainVerb;
            if (def != null)
            {
                var s = new StringBuilder();
                s.AppendLine(StringsToTranslate.ST_Cooldown + def.SecondsToRecharge.ToString("N0") + " " +
                             "SecondsLower".Translate());
                s.AppendLine(StringsToTranslate.ST_Type + def.AbilityTargetCategory switch
                {
                    AbilityTargetCategory.TargetAoE => StringsToTranslate.ST_TargetAoE,
                    AbilityTargetCategory.TargetSelf => StringsToTranslate.ST_TargetSelf,
                    AbilityTargetCategory.TargetThing => StringsToTranslate.ST_TargetThing,
                    AbilityTargetCategory.TargetLocation => StringsToTranslate.ST_TargetLocation,
                    _ => throw new NotImplementedException(),
                });
                if (def.tooltipShowProjectileDamage)
                    if (def.defaultProjectile != null)
                        if (def.defaultProjectile.projectile != null)
                            if (def.defaultProjectile.projectile.GetDamageAmount(1f, null) > 0)
                            {
                                s.AppendLine("Damage".Translate() + ": " +
                                             def.defaultProjectile.projectile.GetDamageAmount(1f, null));
                                s.AppendLine("Damage".Translate() + " " + StringsToTranslate.ST_Type +
                                             def.defaultProjectile.projectile.damageDef.LabelCap);
                            }
                if (def.tooltipShowExtraDamages)
                    if (def.extraDamages != null)
                        if (def.extraDamages.Count == 1)
                        {
                            s.AppendLine(StringsToTranslate.ST_Extra + " " + "Damage".Translate() + ": " +
                                         def.extraDamages[0].damage);
                            s.AppendLine(StringsToTranslate.ST_Extra + " " + "Damage".Translate() + " " +
                                         StringsToTranslate.ST_Type + def.extraDamages[0].damageDef.LabelCap);
                        }
                        else if (def.extraDamages.Count > 1)
                        {
                            s.AppendLine(StringsToTranslate.ST_Extra + " " + "Damage".Translate() + ": ");
                            foreach (var extraDam in def.extraDamages)
                            {
                                s.AppendLine("\t" + StringsToTranslate.ST_Extra + " " + "Damage".Translate() + " " +
                                             StringsToTranslate.ST_Type + extraDam.damageDef.LabelCap);
                                s.AppendLine("\t" + StringsToTranslate.ST_Extra + " " + "Damage".Translate() + ": " +
                                             extraDam.damage);
                            }
                        }
                if (def.tooltipShowMentalStatesToApply)
                    if (def.mentalStatesToApply != null)
                        if (def.mentalStatesToApply.Count == 1)
                        {
                            s.AppendLine(StringsToTranslate.ST_MentalStateChance + ": " +
                                         def.mentalStatesToApply[0].mentalStateDef.LabelCap + " " +
                                         def.mentalStatesToApply[0].applyChance.ToStringPercent());
                        }
                        else if (def.mentalStatesToApply.Count > 1)
                        {
                            s.AppendLine(StringsToTranslate.ST_MentalStateChance);
                            foreach (var mentalState in def.mentalStatesToApply)
                                s.AppendLine("\t" + mentalState.mentalStateDef.LabelCap + " " +
                                             mentalState.applyChance.ToStringPercent());
                        }
                if (def.tooltipShowHediffsToApply)
                {
                    if (def.hediffsToApply != null)
                        if (def.hediffsToApply.Count == 1)
                        {
                            s.AppendLine(StringsToTranslate.ST_EffectChance + def.hediffsToApply[0].hediffDef.LabelCap +
                                         " " + def.hediffsToApply[0].applyChance.ToStringPercent());
                        }
                        else if (def.hediffsToApply.Count > 1)
                        {
                            s.AppendLine(StringsToTranslate.ST_EffectChance);
                            foreach (var hediff in def.hediffsToApply)
                            {
                                float duration = 0;
                                if (hediff.hediffDef.comps != null)
                                    if (hediff.hediffDef.HasComp(typeof(HediffComp_Disappears)))
                                    {
                                        var intDuration =
                                        ((HediffCompProperties_Disappears)hediff.hediffDef.CompPropsFor(
                                            typeof(HediffComp_Disappears))).disappearsAfterTicks.max;
                                        duration = intDuration.TicksToSeconds();
                                    }
                                if (duration == 0)
                                    s.AppendLine("\t" + hediff.hediffDef.LabelCap + " " +
                                                 hediff.applyChance.ToStringPercent());
                                else
                                    s.AppendLine("\t" + hediff.hediffDef.LabelCap + " " +
                                                 hediff.applyChance.ToStringPercent() + " " + duration + " " +
                                                 "SecondsToLower".Translate());
                            }
                        }
                    if (def.burstShotCount > 1)
                        s.AppendLine(StringsToTranslate.ST_BurstShotCount + " " + def.burstShotCount);
                }

                return s.ToString();
            }
            return "";
        }
    }
}
