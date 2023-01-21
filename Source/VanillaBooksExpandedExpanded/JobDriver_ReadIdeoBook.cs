using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using VanillaBooksExpanded;
using Verse;
using Verse.AI;

namespace VanillaBooksExpandedExpanded;

[StaticConstructorOnStartup]
public class JobDriver_ReadIdeoBook : JobDriver_ReadBook
{
    private static readonly FieldInfo CurReadingTicksField =
        AccessTools.Field(typeof(JobDriver_ReadBook), "curReadingTicks");

    private static readonly MethodInfo FindSeatsForReadingMethod =
        AccessTools.Method(typeof(JobDriver_ReadBook), "FindSeatsForReading");

    private static readonly PropertyInfo TotalReadingTicksProperty =
        AccessTools.Property(typeof(JobDriver_ReadBook), "totalReadingTicks");

    protected IdeoligionBook book => TargetThingB as IdeoligionBook;

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDestroyedOrNull(TargetIndex.B);
        yield return Toils_Reserve.Reserve(TargetIndex.B);
        yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch);
        pawn.CurJob.count = 1;
        yield return Toils_Haul.StartCarryThing(TargetIndex.B);
        yield return ((Toil)FindSeatsForReadingMethod.Invoke(this, new object[] { pawn })).FailOnForbidden(
            TargetIndex.C);
        yield return Toils_Reserve.Reserve(TargetIndex.C);
        yield return Toils_Goto.GotoThing(TargetIndex.C, PathEndMode.OnCell);

        var toil = new Toil();
        toil.AddPreInitAction(delegate
        {
            pawn.CurJob.targetA = pawn;
            if (pawn.carryTracker.CarriedThing is Book)
            {
                book.stopDraw = true;
            }

            pawn.GainComfortFromCellIfPossible();
            if (book.Props.saveReadingProgress)
            {
                CurReadingTicksField.SetValue(this, book.curReadingTicks);
            }
        });
        toil.tickAction = delegate
        {
            var jobPawn = pawn;
            if (TargetC.HasThing)
            {
                jobPawn.Rotation = TargetC.Thing.Rotation;
            }

            var ideoBook = book;
            if (ideoBook != null && jobPawn?.ideo != null)
            {
                // can't call OffsetCertainty or it'll spam text motes
                var modified = Mathf.Clamp01(jobPawn.ideo.Certainty + ideoBook.GetCertaintyAmountPerTick(jobPawn));
                jobPawn.ideo.Certainty = modified;
            }

            if (book.Props.joyAmountPerTick > 0f)
            {
                pawn.needs.joy.GainJoy(book.Props.joyAmountPerTick, VBE_DefOf.VBE_Reading);
            }

            var readingTicks = (int)CurReadingTicksField.GetValue(this);
            readingTicks++;
            CurReadingTicksField.SetValue(this, readingTicks);
            book.curReadingTicks = readingTicks;
            if (readingTicks <= (int)TotalReadingTicksProperty.GetValue(this))
            {
                return;
            }

            if (pawn.carryTracker.CarriedThing is Book)
            {
                book.stopDraw = false;
            }

            ReadyForNextToil();
        };
        toil.handlingFacing = true;
        toil.WithEffect(() => book.Props.readingEffecter, () => TargetA);
        toil.defaultCompleteMode = ToilCompleteMode.Never;
        toil.WithProgressBar(TargetIndex.B,
            () => (int)CurReadingTicksField.GetValue(this) / (float)(int)TotalReadingTicksProperty.GetValue(this));
        yield return toil;
        yield return new Toil
        {
            initAction = delegate
            {
                if (pawn.carryTracker.CarriedThing is Book)
                {
                    book.stopDraw = false;
                }

                if (pawn.GetRoom()?.Role == VBE_DefOf.VBE_Library)
                {
                    TryGainLibraryRoomThought(pawn);
                }
                else
                {
                    JoyUtility.TryGainRecRoomThought(pawn);
                }

                if (VBEE_DefOf.VME_ReadBook != null)
                {
                    Find.HistoryEventsManager.RecordEvent(new HistoryEvent(VBEE_DefOf.VME_ReadBook,
                        pawn.Named(HistoryEventArgsNames.Doer)));
                }

                if (book.Props.destroyAfterReading)
                {
                    book.Destroy();
                    return;
                }

                Thing ideoligionBook = book;
                if (book.ParentHolder is Pawn_CarryTracker pawn_CarryTracker)
                {
                    pawn_CarryTracker.TryDropCarriedThing(pawn.Position, ThingPlaceMode.Near, out ideoligionBook);
                }

                var currentPriority = StoreUtility.CurrentStoragePriorityOf(ideoligionBook);
                if (StoreUtility.TryFindBestBetterStoreCellFor(book, pawn, Map, currentPriority, pawn.Faction,
                        out var storeCell))
                {
                    var j = HaulAIUtility.HaulToCellStorageJob(pawn, ideoligionBook, storeCell, false);
                    pawn.jobs.jobQueue.EnqueueFirst(j);
                }

                EndJobWith(JobCondition.Succeeded);
            },
            defaultCompleteMode = ToilCompleteMode.Instant
        };
    }
}