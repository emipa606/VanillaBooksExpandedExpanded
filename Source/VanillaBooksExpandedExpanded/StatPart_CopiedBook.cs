using RimWorld;
using Verse;

namespace VanillaBooksExpandedExpanded;

public class StatPart_CopiedBook : StatPart
{
    private readonly float factorCopied = 1f;
    private readonly float factorOriginal = 1f;

    public override void TransformValue(StatRequest req, ref float val)
    {
        var copied = req.Thing?.TryGetComp<CompCopiedBook>();
        if (copied == null)
        {
            return;
        }

        if (copied.Copied)
        {
            val *= factorCopied;
        }
        else
        {
            val *= factorOriginal;
        }
    }

    public override string ExplanationPart(StatRequest req)
    {
        var copied = req.Thing?.TryGetComp<CompCopiedBook>();
        if (copied == null)
        {
            return null;
        }

        if (copied.Copied)
        {
            return "VanillaBooksExpandedExpanded_StatsReport_BookIsCopied".Translate() + ": x" +
                   factorCopied.ToStringPercent();
        }

        return "VanillaBooksExpandedExpanded_StatsReport_BookIsOriginal".Translate() + ": x" +
               factorOriginal.ToStringPercent();
    }
}