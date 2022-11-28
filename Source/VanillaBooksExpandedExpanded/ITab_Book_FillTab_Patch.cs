using System.Reflection;
using HarmonyLib;
using UnityEngine;
using VanillaBooksExpanded;
using Verse;

namespace VanillaBooksExpandedExpanded;

[HarmonyPatch(typeof(ITab_Book), "FillTab")]
internal static class ITab_Book_FillTab_Patch
{
    private static readonly PropertyInfo SelectedCompBook = AccessTools.Property(typeof(ITab_Book), "SelectedCompBook");
    private static Vector2 ScrollbarPosition = Vector2.zero;

    public static bool Prefix(ITab_Book __instance, ref string ___cachedImageDescription,
        ref CompBook ___cachedImageSource, ref TaleBookReference ___cachedTaleRef, Vector2 ___WinSize)
    {
        var selected = (CompBook)SelectedCompBook.GetValue(__instance);
        var winRect = new Rect(0f, 0f, ___WinSize.x, ___WinSize.y).ContractedBy(10f);

        var titleRect = winRect;
        Text.Font = GameFont.Medium;
        Widgets.LabelCacheHeight(ref titleRect, selected.Title);
        if (___cachedImageSource != selected || ___cachedTaleRef != selected.TaleRef)
        {
            ___cachedImageDescription = selected.GenerateImageDescription();
            ___cachedImageSource = selected;
            ___cachedTaleRef = selected.TaleRef;
        }

        var descRect = winRect;
        descRect.yMin += titleRect.height + 5f;
        Text.Font = GameFont.Small;
        Widgets.LabelScrollable(descRect, ___cachedImageDescription, ref ScrollbarPosition);

        return false;
    }
}