using HugsLib;
using VanillaBooksExpanded;
using Verse;

namespace VanillaBooksExpandedExpanded;

public class VanillaBooksExpandedExpanded : ModBase
{
    public static bool IsJoyBook(Thing item)
    {
        return item is SkillBook or Newspaper or IdeoligionBook;
    }
}