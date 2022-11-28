using HarmonyLib;
using RimWorld;
using VanillaBooksExpanded;
using Verse;

namespace VanillaBooksExpandedExpanded;

[HarmonyPatch(typeof(Thing), nameof(Thing.Kill))]
internal static class Thing_Kill_Patch
{
    public static void Prefix(Thing __instance, DamageInfo? dinfo)
    {
        if (__instance is not Book or Newspaper)
        {
            return;
        }

        var instigator = dinfo?.Instigator;

        var eventDef = VBEE_DefOf.VanillaBooksExpandedExpanded_BookKilled;
        if (eventDef != null)
        {
            Find.HistoryEventsManager.RecordEvent(new HistoryEvent(
                eventDef,
                instigator.Named(HistoryEventArgsNames.Doer),
                __instance.Named(HistoryEventArgsNames.Victim)
            ));
        }
    }
}