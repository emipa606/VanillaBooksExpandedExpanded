using RimWorld;

namespace VanillaBooksExpandedExpanded;

[DefOf]
public static class VBEE_JoyGiverDefOf
{
    public static JoyGiverDef VBE_ReadBook;

    [MayRequireIdeology] public static JoyGiverDef VanillaBooksExpandedExpanded_ReadIdeoBook;

    static VBEE_JoyGiverDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(VBEE_JoyGiverDefOf));
    }
}