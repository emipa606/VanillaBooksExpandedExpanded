using HarmonyLib;
using VanillaBooksExpanded;
using Verse;

namespace VanillaBooksExpandedExpanded;

[HarmonyPatch(typeof(RoomRoleWorker_Library), nameof(RoomRoleWorker_Library.GetScore))]
internal static class RoomRoleWorker_Library_GetScore_Patch
{
    public static float Postfix(float __result, Room room)
    {
        foreach (var thing in room.ContainedAndAdjacentThings)
        {
            if (thing.def.defName == "VanillaBooksExpandedExpanded_Bookshelf")
            {
                __result += 5f;
            }
        }

        return __result;
    }
}