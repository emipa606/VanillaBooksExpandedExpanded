using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using VanillaBooksExpanded;
using Verse;

namespace VanillaBooksExpandedExpanded;

[HarmonyPatch(typeof(JoyUtility), nameof(JoyUtility.JoyKindsOnMapTempList))]
internal static class JoyUtility_JoyKindsOnMapTempList_Patch
{
    public static void Postfix(Map map, List<JoyKindDef> ___tempKindList)
    {
        if (___tempKindList.Contains(VBE_DefOf.VBE_Reading))
        {
            return;
        }

        foreach (var def in VBEE_JoyGiverDefOf.VBE_ReadBook.thingDefs)
        {
            if (map.listerThings.ThingsOfDef(def).Count == 0)
            {
                continue;
            }

            ___tempKindList.Add(VBE_DefOf.VBE_Reading);

            return;
        }

        if (VBEE_JoyGiverDefOf.VanillaBooksExpandedExpanded_ReadIdeoBook == null)
        {
            return;
        }

        foreach (var def in VBEE_JoyGiverDefOf.VanillaBooksExpandedExpanded_ReadIdeoBook.thingDefs)
        {
            if (map.listerThings.ThingsOfDef(def).Count == 0)
            {
                continue;
            }

            ___tempKindList.Add(VBE_DefOf.VBE_Reading);

            return;
        }
    }
}