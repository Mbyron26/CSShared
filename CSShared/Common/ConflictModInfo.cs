namespace CSShared.Common;

public readonly struct ConflictModInfo {
    public readonly ulong ModID;
    public readonly string Name;
    public readonly bool Inclusive;
    public readonly string UseInstead;

    public ConflictModInfo(ulong modID, string modName, bool isInclusive) {
        ModID = modID;
        Name = modName;
        Inclusive = isInclusive;
        UseInstead = null;
    }
    public ConflictModInfo(ulong modID, string modName, bool isInclusive, string useInstead) {
        ModID = modID;
        Name = modName;
        Inclusive = isInclusive;
        UseInstead = useInstead;
    }
}