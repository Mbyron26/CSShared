using System.Reflection;
using System;
using CSShared.Patch;

namespace CSShared.Debug;

public partial interface ILog {
    void Patch(PatcherType patchType, MethodBase original, MethodBase patch);
}