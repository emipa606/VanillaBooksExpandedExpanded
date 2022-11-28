using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace VanillaBooksExpandedExpanded;

public class JoyGiver_ReadIdeoBook : JoyGiver
{
    public override Job TryGiveJob(Pawn pawn)
    {
        return TryGiveJobInternal(pawn, null);
    }

    public override Job TryGiveJobInGatheringArea(Pawn pawn, IntVec3 gatherSpot, float maxRadius = -1)
    {
        return TryGiveJobInternal(pawn,
            book => !book.Spawned || GatheringsUtility.InGatheringArea(book.Position, gatherSpot, pawn.Map) &&
                (maxRadius < 0f || book.Position.InHorDistOf(gatherSpot, maxRadius)));
    }

    private Job TryGiveJobInternal(Pawn pawn, Predicate<Thing> predicate)
    {
        if (pawn.Ideo == null)
        {
            return null;
        }

        var candidates = new List<Thing>();

        GetSearchSet(pawn, candidates);

        var chosenBook = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, candidates,
            PathEndMode.OnCell, TraverseParms.For(pawn, pawn.NormalMaxDanger()),
            validator: book => (predicate == null || predicate(book)) && JoyGiver_ReadBook.CanReadBook(pawn, book),
            priorityGetter: JoyGiver_ReadBook.BookQualityPriority);
        if (chosenBook == null)
        {
            return null;
        }

        var job = JobMaker.MakeJob(def.jobDef, null, chosenBook);
        job.count = 1;

        return job;
    }
}