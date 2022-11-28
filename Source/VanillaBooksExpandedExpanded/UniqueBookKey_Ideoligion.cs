using RimWorld;
using VanillaBooksExpanded;
using Verse;

namespace VanillaBooksExpandedExpanded;

public class UniqueBookKey_Ideoligion : UniqueBookKey
{
    public UniqueBookKey_Ideoligion(ThingDef def, TaleBookReference taleRef, Ideo ideo) : base(def, taleRef)
    {
        Ideo = ideo;
    }

    public Ideo Ideo { get; }

    public override int GetHashCode()
    {
        return Gen.HashCombine(base.GetHashCode(), Ideo);
    }

    public override bool Equals(UniqueBookKey key)
    {
        return base.Equals(key) && key is UniqueBookKey_Ideoligion ideo && ideo.Ideo == Ideo;
    }
}