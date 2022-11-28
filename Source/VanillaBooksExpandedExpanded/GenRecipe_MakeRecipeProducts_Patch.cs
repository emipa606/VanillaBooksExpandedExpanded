using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using VanillaBooksExpanded;
using Verse;

namespace VanillaBooksExpandedExpanded;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(GenRecipe), nameof(GenRecipe.MakeRecipeProducts))]
internal static class GenRecipe_MakeRecipeProducts_Patch
{
    public static bool Prefix(ref IEnumerable<Thing> __result, RecipeDef recipeDef, Pawn worker,
        List<Thing> ingredients, Thing dominantIngredient)
    {
        if (recipeDef.defName == "VanillaBooksExpandedExpanded_BurnBooks")
        {
            var eventDef = VBEE_DefOf.VanillaBooksExpandedExpanded_BookBurned;

            if (eventDef != null)
            {
                Find.HistoryEventsManager.RecordEvent(new HistoryEvent(
                    eventDef,
                    worker.Named(HistoryEventArgsNames.Doer),
                    ingredients.Single().Named(HistoryEventArgsNames.Victim)
                ));
            }

            return true;
        }

        if (recipeDef.workerClass != typeof(RecipeWorker_CopyBook))
        {
            return true;
        }

        var sourceBook = ingredients.OfType<Book>().Single();
        var product = recipeDef.products.Single();

        var copies = new List<Thing>(product.count);

        for (var i = 0; i < product.count; i++)
        {
            copies.Add(RecipeWorker_CopyBook.CopyBook(sourceBook, recipeDef, worker, ingredients, dominantIngredient));
        }

        __result = copies;

        return false;
    }
}