using HarmonyLib;
using Verse;

namespace VanillaBooksExpandedExpanded;

[HarmonyPatch(typeof(GenRecipe), "PostProcessProduct")]
internal static class GenRecipe_PostProcessProduct_Patch
{
    public static void Postfix(Thing __result, Pawn worker)
    {
        if (__result is IdeoligionBook ideoBook)
        {
            ideoBook.ideoligion = worker.Ideo ?? worker.Faction?.ideos?.GetRandomIdeoForNewPawn() ??
                Find.IdeoManager?.IdeosListForReading?.RandomElementWithFallback();
        }
    }
}