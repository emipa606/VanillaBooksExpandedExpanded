using Verse;

namespace VanillaBooksExpandedExpanded;

public class CompCopiedBook : ThingComp
{
    private bool isCopied;

    public bool Copied
    {
        get => isCopied;
        internal set => isCopied = value;
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref isCopied, "isCopied");
    }

    public override bool AllowStackWith(Thing other)
    {
        return other.TryGetComp<CompCopiedBook>()?.isCopied == isCopied;
    }

    public override void PostSplitOff(Thing piece)
    {
        base.PostSplitOff(piece);
        piece.TryGetComp<CompCopiedBook>().isCopied = isCopied;
    }

    public override string CompInspectStringExtra()
    {
        return isCopied ? "VanillaBooksExpandedExpanded_BookIsCopy".Translate() : null;
    }
}