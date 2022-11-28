using Verse;

namespace VanillaBooksExpandedExpanded;

public class SpecialThingFilterWorker_CopiedBooks : SpecialThingFilterWorker
{
    public override bool Matches(Thing t)
    {
        return t.TryGetComp<CompCopiedBook>()?.Copied == true;
    }

    public override bool CanEverMatch(ThingDef def)
    {
        return def.HasComp(typeof(CompCopiedBook));
    }
}