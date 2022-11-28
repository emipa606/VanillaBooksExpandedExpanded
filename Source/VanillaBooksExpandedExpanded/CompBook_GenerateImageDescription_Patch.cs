using HarmonyLib;
using VanillaBooksExpanded;
using Verse;

namespace VanillaBooksExpandedExpanded;

[HarmonyPatch(typeof(CompBook), nameof(CompBook.GenerateImageDescription))]
internal static class CompBook_GenerateImageDescription_Patch
{
    public static bool Prefix(ref TaggedString __result, CompBook __instance)
    {
        if (__instance.parent is not IdeoligionBook ideoBook || ideoBook.ideoligion == null)
        {
            return true;
        }

        Rand.PushState();
        Rand.Seed = __instance.TaleRef.seed;
        try
        {
            __result = $"{ideoBook.ideoligion.GetNewDescription().text.StripTags()}\n\n" +
                       "VanillaBooksExpandedExpanded_BookRelatedToIdeo".Translate(
                           ideoBook.ideoligion.Named("IDEO"));
        }
        finally
        {
            Rand.PopState();
        }

        return false;
    }
}