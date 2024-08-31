using ICities;
using System.Collections.Generic;
using System.Globalization;
using System;

namespace CSShared.Common;

public interface IMod : IUserMod, IModLoading {
    BuildVersion VersionType { get; }
    string ModName { get; }
    List<ChangelogInfo> Changelog { get; }
    Version ModVersion { get; }
    ulong StableID { get; }
    void SaveConfig();
    bool LoadConfig();
}