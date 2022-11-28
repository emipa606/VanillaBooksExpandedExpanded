using HarmonyLib;
using VanillaBooksExpanded;
using Verse;

namespace VanillaBooksExpandedExpanded;

[HarmonyPatch(typeof(CompBook), "GenerateTitle")]
internal static class CompBook_GenerateTitle_Patch
{
    public static string Postfix(string __result, CompBook __instance)
    {
        if (__instance.parent is IdeoligionBook { ideoligion: { } } ideoBook)
        {
            return "VanillaBooksExpandedExpanded_BookIdeoTitle".Translate(__result.Named("TITLE"),
                ideoBook.ideoligion.Named("IDEO"));
        }

        return __result;
    }
}