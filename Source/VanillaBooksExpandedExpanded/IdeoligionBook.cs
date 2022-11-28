using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using VanillaBooksExpanded;
using Verse;
using Verse.AI;

namespace VanillaBooksExpandedExpanded;

public class IdeoligionBook : Book
{
    private Material iconCached;
    public Ideo ideoligion;
    public bool initialized;

    private Material IdeoIcon
    {
        get
        {
            if (iconCached == null)
            {
                iconCached = MaterialPool.MatFrom(ideoligion.Icon, ShaderDatabase.Cutout,
                    Color.Lerp(ideoligion.Color, new Color32(133, 89, 113, 255), 0.5f));
            }

            return iconCached;
        }
    }

    private void InitOnce()
    {
        if (ideoligion == null)
        {
            ideoligion = Find.IdeoManager.IdeosListForReading.RandomElementWithFallback();
        }

        if (initialized)
        {
            return;
        }

        var compBook = this.TryGetComp<CompBook>();
        if (!compBook.Active)
        {
            compBook.InitializeBook();
        }

        initialized = true;
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);

        InitOnce();
    }

    public override void PostGeneratedForTrader(TraderKindDef trader, int forTile, Faction forFaction)
    {
        base.PostGeneratedForTrader(trader, forTile, forFaction);

        ideoligion = forFaction?.ideos?.GetRandomIdeoForNewPawn();

        InitOnce();
    }

    public float GetBaseCertaintyAmountPerTick(bool reassure)
    {
        var amount = reassure ? 0.000015f : -0.00001f;

        switch (GetComp<CompQuality>().Quality)
        {
            case QualityCategory.Awful:
                amount *= -0.25f;
                break;
            case QualityCategory.Poor:
                amount *= 0.5f;
                break;
            case QualityCategory.Normal:
                amount *= 1f;
                break;
            case QualityCategory.Good:
                amount *= 1.5f;
                break;
            case QualityCategory.Excellent:
                amount *= 2f;
                break;
            case QualityCategory.Masterwork:
                amount *= 3.5f;
                break;
            case QualityCategory.Legendary:
                amount *= 5.5f;
                break;
        }

        return amount;
    }

    public float GetCertaintyAmountPerTick(Pawn pawn)
    {
        var reassure = pawn.Ideo == ideoligion;
        var certainty = GetBaseCertaintyAmountPerTick(reassure);

        if (!reassure)
        {
            certainty *= pawn.GetStatValue(StatDefOf.CertaintyLossFactor);
        }

        return certainty;
    }

    public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
    {
        if (!myPawn.CanReach(this, PathEndMode.Touch, Danger.Deadly))
        {
            yield return new FloatMenuOption("CannotUseNoPath".Translate(), null);
        }
        else if (myPawn.Ideo != null)
        {
            yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(
                "VanillaBooksExpandedExpanded_ReadBook".Translate(GetComp<CompBook>().Title.Named("TITLE"),
                    this.Named("BOOK")), delegate
                {
                    var job = JobMaker.MakeJob(VBEE_DefOf.VanillaBooksExpandedExpanded_ReadIdeoBook, null, this);
                    job.count = 1;
                    myPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                }), myPawn, this);
        }
        else
        {
            yield return new FloatMenuOption("VanillaBooksExpandedExpanded_CantReadBookNoIdeo".Translate(), null);
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref initialized, "initialized");
        Scribe_References.Look(ref ideoligion, "ideoligion");
    }

    public override void DrawAt(Vector3 drawLoc, bool flip = false)
    {
        base.DrawAt(drawLoc, flip);

        if (!stopDraw && ideoligion != null)
        {
            Graphics.DrawMesh(MeshPool.plane05, drawLoc + new Vector3(0.0125f, 0.01f, 0.08f), Quaternion.identity,
                IdeoIcon, 0);
        }
    }

    public override void Print(SectionLayer layer)
    {
        base.Print(layer);

        if (ideoligion != null)
        {
            Printer_Plane.PrintPlane(layer, this.TrueCenter() + new Vector3(0.0125f, 0.01f, 0.08f),
                new Vector2(0.5f, 0.5f), IdeoIcon);
        }
    }
}