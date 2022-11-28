using RimWorld;
using Verse;

namespace VanillaBooksExpandedExpanded;

[DefOf]
public static class VBEE_DefOf
{
    [MayRequire("vanillaexpanded.vmemese")]
    public static HistoryEventDef VanillaBooksExpandedExpanded_BookBurned;

    [MayRequire("vanillaexpanded.vmemese")]
    public static HistoryEventDef VanillaBooksExpandedExpanded_BookKilled;

    [MayRequire("vanillaexpanded.vmemese")]
    public static HistoryEventDef VME_ReadBook;

    [MayRequireIdeology] public static JobDef VanillaBooksExpandedExpanded_ReadIdeoBook;

    public static SpecialThingFilterDef VanillaBooksExpandedExpanded_AllowCopies;

    static VBEE_DefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(VBEE_DefOf));
    }
}