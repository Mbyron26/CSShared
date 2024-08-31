using System.Collections.Generic;
using System;

namespace CSShared.Common;

public struct ChangelogInfo {
    public readonly Version ModVersion;
    public readonly DateTime Date;
    public readonly List<ChangelogContent> Contents;

    public ChangelogInfo(Version version, DateTime date, List<ChangelogContent> contents) {
        ModVersion = version;
        Date = date;
        Contents = contents;
    }
}