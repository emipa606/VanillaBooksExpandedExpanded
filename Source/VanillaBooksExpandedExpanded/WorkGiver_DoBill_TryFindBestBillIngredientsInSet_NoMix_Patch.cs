using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using VanillaBooksExpanded;
using Verse;

namespace VanillaBooksExpandedExpanded;

[HarmonyPatch(typeof(WorkGiver_DoBill), "TryFindBestBillIngredientsInSet_NoMix")]
internal static class WorkGiver_DoBill_TryFindBestBillIngredientsInSet_NoMix_Patch
{
    private static Map LastBillMap;
    private static int LastBillTick;
    private static List<UniqueBookKey> LastBillKeys = new List<UniqueBookKey>();

    private static List<UniqueBookKey> KeysForBill(Bill bill)
    {
        if (bill.Map == LastBillMap && GenTicks.TicksAbs == LastBillTick)
        {
            return LastBillKeys;
        }

        LastBillMap = bill.Map;
        LastBillTick = GenTicks.TicksAbs;
        LastBillKeys = RecipeWorkerCounter_CopyBookFewest.LeastCopiedBooks((Bill_Production)bill, out _);

        return LastBillKeys;
    }

    public static void Prefix(List<Thing> availableThings, Bill bill, List<ThingCount> chosen, IntVec3 rootCell,
        bool alreadySorted)
    {
        if (bill.recipe.workerCounterClass != typeof(RecipeWorkerCounter_CopyBookFewest))
        {
            return;
        }

        var keys = KeysForBill(bill);
        availableThings.RemoveAll(thing => thing is Book && !keys.Contains(UniqueBookKey.For(thing)));
    }
}