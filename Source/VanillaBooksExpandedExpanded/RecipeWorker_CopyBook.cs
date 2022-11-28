using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using VanillaBooksExpanded;
using Verse;

namespace VanillaBooksExpandedExpanded;

[StaticConstructorOnStartup]
public class RecipeWorker_CopyBook : RecipeWorker
{
    private static readonly FieldInfo AuthorNameInt = AccessTools.Field(typeof(CompBook), "authorNameInt");
    private static readonly FieldInfo TitleInt = AccessTools.Field(typeof(CompBook), "titleInt");
    private static readonly FieldInfo TaleRef = AccessTools.Field(typeof(CompBook), "taleRef");

    public override void ConsumeIngredient(Thing ingredient, RecipeDef recipe, Map map)
    {
        if (ingredient is Book)
        {
            return;
        }

        base.ConsumeIngredient(ingredient, recipe, map);
    }

    public static Book CopyBook(Book source, RecipeDef recipeDef, Pawn worker, List<Thing> ingredients,
        Thing dominantIngredient)
    {
        var bookData = source.GetComp<CompBook>();
        var author = AuthorNameInt.GetValue(bookData);
        var title = TitleInt.GetValue(bookData);
        var tale = bookData.TaleRef;

        var copy = (Book)ThingMaker.MakeThing(stuff: source.def.MadeFromStuff ? dominantIngredient.def : null,
            def: source.def);

        if (source.TryGetQuality(out var quality))
        {
            copy.TryGetComp<CompQuality>()?.SetQuality(quality, ArtGenerationContext.Colony);
        }

        copy.GetComp<CompCopiedBook>().Copied = true;

        var outBookData = copy.GetComp<CompBook>();

        outBookData.Clear();
        AuthorNameInt.SetValue(outBookData, author);
        TitleInt.SetValue(outBookData, title);

        if (tale != null)
        {
            tale.tale?.Notify_NewlyUsed();

            TaleRef.SetValue(outBookData, new TaleBookReference(tale.tale)
            {
                seed = tale.seed
            });
        }

        if (source is IdeoligionBook sourceIdeo)
        {
            ((IdeoligionBook)copy).ideoligion = sourceIdeo.ideoligion;
        }

        return copy;
    }
}