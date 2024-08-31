using System;

namespace CSShared.Extension;

public static class VersionExtension {
    public static string GetString(this Version version) {
        if (version.Revision > 0)
            return version.ToString(4);
        else if (version.Build > 0)
            return version.ToString(3);
        else
            return version.ToString(2);
    }
}