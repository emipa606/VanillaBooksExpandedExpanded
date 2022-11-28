using Verse;

namespace VanillaBooksExpandedExpanded;

public class SpecialThingFilterWorker_OriginalBooks : SpecialThingFilterWorker
{
    public override bool Matches(Thing t)
    {
        return t.TryGetComp<CompCopiedBook>()?.Copied == false;
    }

    public override bool CanEverMatch(ThingDef def)
    {
        return def.HasComp(typeof(CompCopiedBook));
    }
}