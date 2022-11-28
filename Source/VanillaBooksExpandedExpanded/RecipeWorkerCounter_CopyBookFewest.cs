using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace VanillaBooksExpandedExpanded;

public class RecipeWorkerCounter_CopyBookFewest : RecipeWorkerCounter_CopyBook
{
    public static List<UniqueBookKey> LeastCopiedBooks(Bill_Production bill, out int count)
    {
        var defs = PossibleOutputsForBill(bill);
        var keys = bill.Map.mapPawns.FreeColonistsSpawned.Select(pawn => pawn.carryTracker.CarriedThing)
            .Where(carried => carried != null && defs.Contains(carried.def))
            .Concat(defs.SelectMany(def => bill.Map.listerThings.ThingsOfDef(def)))
            .Where(book => !book.TryGetQuality(out var quality) ||
                           bill.ingredientFilter.AllowedQualityLevels.Includes(quality))
            .GroupBy(UniqueBookKey.For)
            .Select(group => new
                { key = group.Key, count = group.Count(b => b.TryGetComp<CompCopiedBook>()?.Copied == true) })
            .ToList();

        if (!keys.Any())
        {
            count = 0;

            return new List<UniqueBookKey>();
        }

        var minCount = keys.Select(k => k.count).Min();

        count = minCount;

        return keys.Where(k => k.count == minCount).Select(k => k.key).ToList();
    }

    public override int CountProducts(Bill_Production bill)
    {
        _ = LeastCopiedBooks(bill, out var count);

        return count;
    }
}