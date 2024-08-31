using System.IO;
using System.Reflection;
using System.Threading;
using System;
using UnityEngine;
using CSShared.Tools;
using CSShared.Patch;

namespace CSShared.Debug;

public partial class Logger : ILog {
    public void Patch(PatcherType patchType, MethodBase original, MethodBase patch) => Log(LogType.Patch, $"[{patchType}] [{original.DeclaringType.FullName}.{original.Name}] patched by [{patch.DeclaringType.FullName}.{patch.Name}]");
}