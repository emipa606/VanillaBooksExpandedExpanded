using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using VanillaBooksExpanded;
using Verse;
using Verse.AI;

namespace VanillaBooksExpandedExpanded;

public class JoyGiver_ReadBook : JoyGiver
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
        var candidates = new List<Thing>();

        GetSearchSet(pawn, candidates);

        var allowedSkillBooks = candidates.OfType<SkillBook>()
            .Where(b => !pawn.skills.GetSkill(b.SkillData.skillToTeach).TotallyDisabled)
            .Where(b => b.CanLearnFromBook(pawn))
            .ToList();

        var bestSkillBook = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, allowedSkillBooks,
            PathEndMode.OnCell, TraverseParms.For(pawn, pawn.NormalMaxDanger()),
            validator: book => (predicate == null || predicate(book)) && CanReadBook(pawn, book),
            priorityGetter: book => BookQualityPriority(book) + SkillBookPriority(pawn, book));
        Job job;
        if (bestSkillBook != null)
        {
            job = JobMaker.MakeJob(def.jobDef, null, bestSkillBook);
            job.count = 1;

            return job;
        }

        if (!(pawn.needs.joy.CurLevel >= 0.6f))
        {
            return null;
        }

        var allowedNewspapers = candidates.OfType<Newspaper>()
            .Where(n => n.IsRelevant)
            .ToList();

        var bestNewspaper = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, allowedNewspapers,
            PathEndMode.OnCell, TraverseParms.For(pawn, pawn.NormalMaxDanger()),
            validator: book => (predicate == null || predicate(book)) && CanReadBook(pawn, book));
        if (bestNewspaper == null)
        {
            return null;
        }

        job = JobMaker.MakeJob(def.jobDef, null, bestNewspaper);
        job.count = 1;

        return job;
    }

    public static bool CanReadBook(Pawn pawn, Thing book)
    {
        if (!book.Spawned)
        {
            return true;
        }

        if (!pawn.CanReserve(book))
        {
            return false;
        }

        if (book.IsForbidden(pawn))
        {
            return false;
        }

        return book.IsSociallyProper(pawn) && book.IsPoliticallyProper(pawn);
    }

    public static float BookQualityPriority(Thing book)
    {
        if (book.TryGetQuality(out var quality))
        {
            return -(int)quality;
        }

        return 0f;
    }

    public static float SkillBookPriority(Pawn pawn, Thing book)
    {
        if (book is not SkillBook skillBook)
        {
            return 0f;
        }

        // prioritize enforcing higher skills rather than learning new ones,
        // and greatly prioritize skills the pawn is passionate about.
        var skill = pawn.skills.GetSkill(skillBook.SkillData.skillToTeach);

        return (-skill.Level / 10f) - (int)skill.passion;
    }
}