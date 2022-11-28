using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using VanillaBooksExpanded;
using Verse;

namespace VanillaBooksExpandedExpanded;

[HarmonyPatch(typeof(SkillBook), nameof(SkillBook.GetFloatMenuOptions))]
internal static class SkillBook_GetFloatMenuOptions_Patch
{
    public static IEnumerable<FloatMenuOption> Postfix(IEnumerable<FloatMenuOption> __result, SkillBook __instance,
        Pawn myPawn)
    {
        // VBE always returns exactly one menu item.
        var option = __result.Single();

        if (option.action != null)
        {
            option.Label =
                "VanillaBooksExpandedExpanded_ReadBook".Translate(__instance.GetComp<CompBook>().Title.Named("TITLE"),
                    __instance.Named("BOOK"));
            option = FloatMenuUtility.DecoratePrioritizedTask(option, myPawn, __instance);
        }

        yield return option;
    }
}