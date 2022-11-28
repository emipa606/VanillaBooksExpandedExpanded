using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace VanillaBooksExpandedExpanded;

public class RecipeWorkerCounter_CopyBook : RecipeWorkerCounter
{
    public static List<ThingDef> PossibleOutputsForBill(Bill_Production bill)
    {
        return (from def in bill.ingredientFilter.AllowedThingDefs
            where def.HasComp(typeof(CompCopiedBook))
            select def).ToList();
    }

    public virtual bool ValidForBill(Thing book, Bill_Production bill)
    {
        var copied = book.TryGetComp<CompCopiedBook>();
        if (copied is not { Copied: true })
        {
            return false;
        }

        if (book.TryGetQuality(out var quality) && !bill.ingredientFilter.AllowedQualityLevels.Includes(quality))
        {
            return false;
        }

        return !bill.limitToAllowedStuff || bill.ingredientFilter.Allows(book.Stuff);
    }

    public virtual int Count(IEnumerable<Thing> books, Bill_Production bill)
    {
        return books.Count(book => ValidForBill(book, bill));
    }

    public override int CountProducts(Bill_Production bill)
    {
        var bookDefs = PossibleOutputsForBill(bill);

        if (bill.includeFromZone == null)
        {
            return Count(bookDefs.SelectMany(def => bill.Map.listerThings.ThingsOfDef(def))
                .Concat(bill.Map.mapPawns.FreeColonistsSpawned.Select(pawn => pawn.carryTracker.CarriedThing)
                    .Where(carried => carried != null && bookDefs.Contains(carried.def))), bill);
        }

        return Count(bill.includeFromZone.AllContainedThings.Where(thing => bookDefs.Contains(thing.def)), bill);
    }

    public override string ProductsDescription(Bill_Production bill)
    {
        var possible = PossibleOutputsForBill(bill);
        return possible.Count == 1 ? possible[0].label : null;
    }

    public override bool CanPossiblyStoreInStockpile(Bill_Production bill, Zone_Stockpile stockpile)
    {
        if (!stockpile.GetStoreSettings().filter.Allows(VBEE_DefOf.VanillaBooksExpandedExpanded_AllowCopies))
        {
            return false;
        }

        var allowedQuality = stockpile.GetStoreSettings().filter.AllowedQualityLevels;
        if (allowedQuality.min > bill.ingredientFilter.AllowedQualityLevels.max ||
            allowedQuality.max < bill.ingredientFilter.AllowedQualityLevels.min)
        {
            return false;
        }

        return PossibleOutputsForBill(bill).Any(stockpile.GetStoreSettings().AllowedToAccept);
    }
}