using System;
using VanillaBooksExpanded;
using Verse;

namespace VanillaBooksExpandedExpanded;

public class UniqueBookKey : IEquatable<UniqueBookKey>
{
    public UniqueBookKey(ThingDef def, TaleBookReference taleRef)
    {
        Def = def;
        TaleRef = taleRef;
    }

    public ThingDef Def { get; }
    public TaleBookReference TaleRef { get; }

    public virtual bool Equals(UniqueBookKey key)
    {
        return !(key is null) &&
               key.GetType() == GetType() &&
               key.Def == Def &&
               key.TaleRef.tale == TaleRef.tale &&
               key.TaleRef.seed == TaleRef.seed;
    }

    public static UniqueBookKey For(Thing book)
    {
        var comp = book?.TryGetComp<CompBook>();
        if (comp is not { Active: true })
        {
            return null;
        }

        if (book is IdeoligionBook ideo)
        {
            return new UniqueBookKey_Ideoligion(book.def, comp.TaleRef, ideo.ideoligion);
        }

        return new UniqueBookKey(book.def, comp.TaleRef);
    }

    public static bool operator ==(UniqueBookKey a, UniqueBookKey b)
    {
        if (a is null)
        {
            return b is null;
        }

        return a.Equals(b);
    }

    public static bool operator !=(UniqueBookKey a, UniqueBookKey b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        if (obj is UniqueBookKey key)
        {
            return Equals(key);
        }

        return false;
    }

    public override int GetHashCode()
    {
        var hash = GetType().GetHashCode();
        hash = Gen.HashCombine(hash, Def);
        hash = Gen.HashCombine(hash, TaleRef.tale);
        hash = Gen.HashCombine(hash, TaleRef.seed);
        return hash;
    }
}