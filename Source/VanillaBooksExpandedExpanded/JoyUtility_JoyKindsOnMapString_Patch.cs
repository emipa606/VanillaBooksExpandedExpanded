using System.Reflection;
using System.Text;
using HarmonyLib;
using RimWorld;
using VanillaBooksExpanded;
using Verse;

namespace VanillaBooksExpandedExpanded;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(JoyUtility), nameof(JoyUtility.JoyKindsOnMapString))]
internal static class JoyUtility_JoyKindsOnMapString_Patch
{
    private static readonly MethodInfo
        CheckAppendJoyKind = AccessTools.Method(typeof(JoyUtility), "CheckAppendJoyKind");

    public static string Postfix(string __result, Map map)
    {
        var sb = new StringBuilder(__result);
        if (sb.Length != 0)
        {
            sb.AppendLine();
        }

        foreach (var item in map.listerThings.AllThings)
        {
            if (VanillaBooksExpandedExpanded.IsJoyBook(item))
            {
                CheckAppendJoyKind.Invoke(null, new object[] { sb, item, VBE_DefOf.VBE_Reading, map });
            }
        }

        return sb.ToString().TrimEndNewlines();
    }
}