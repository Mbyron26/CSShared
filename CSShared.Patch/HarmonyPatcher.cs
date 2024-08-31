using ColossalFramework.UI;
using CSShared.Debug;
using CSShared.Tools;
using HarmonyLib;
using System;
using System.Reflection;
using System.Text;

namespace CSShared.Patch;

public sealed class HarmonyPatcher {
    public Harmony Harmony { get; private set; }

    public HarmonyPatcher(string harmonyID = null) {
        if (string.IsNullOrEmpty(harmonyID))
            harmonyID = AssemblyTools.CurrentAssemblyName;
        Harmony = new Harmony(harmonyID);
    }

    public void Reverse() {
        Harmony.UnpatchAll();
        Harmony = null;
    }

    public void PrefixPatching(Type originalType, string originalMethod, Type patchType, string patchMethod, Type[] targetParm = null) => PatchMethod(PatcherType.Prefix, originalType, originalMethod, patchType, patchMethod, targetParm);
    public void PrefixPatching(MethodBase originalMethodInfo, MethodInfo patchMethodInfo) => PatchMethod(PatcherType.Prefix, originalMethodInfo, patchMethodInfo);

    public void PostfixPatching(Type originalType, string originalMethod, Type patchType, string patchMethod, Type[] targetParm = null) => PatchMethod(PatcherType.Postfix, originalType, originalMethod, patchType, patchMethod, targetParm);
    public void PostfixPatching(MethodBase originalMethodInfo, MethodInfo patchMethodInfo) => PatchMethod(PatcherType.Postfix, originalMethodInfo, patchMethodInfo);

    public void TranspilerPatching(Type originalType, string originalMethod, Type patchType, string patchMethod, Type[] targetParm = null) => PatchMethod(PatcherType.Transpiler, originalType, originalMethod, patchType, patchMethod, targetParm);
    public void TranspilerPatching(MethodBase originalMethodInfo, MethodInfo patchMethodInfo) => PatchMethod(PatcherType.Transpiler, originalMethodInfo, patchMethodInfo);

    private void PatchMethod(PatcherType patcherType, MethodBase originalMethodInfo, MethodInfo patchMethodInfo) {
        if (originalMethodInfo is null) {
            LogManager.GetLogger().Error($"Original method not found");
            return;
        }
        if (patchMethodInfo is null) {
            LogManager.GetLogger().Error($"Patch method not found");
            return;
        }
        switch (patcherType) {
            case PatcherType.Prefix: Harmony.Patch(originalMethodInfo, prefix: new HarmonyMethod(patchMethodInfo)); break;
            case PatcherType.Postfix: Harmony.Patch(originalMethodInfo, postfix: new HarmonyMethod(patchMethodInfo)); break;
            case PatcherType.Transpiler: Harmony.Patch(originalMethodInfo, transpiler: new HarmonyMethod(patchMethodInfo)); break;
        };
        LogManager.GetLogger().Info(PatchInfo(patcherType, originalMethodInfo, originalMethodInfo.Name, patchMethodInfo, patchMethodInfo.Name));
    }

    private void PatchMethod(PatcherType patcherType, Type originalType, string originalMethod, Type patchType, string patchMethod, Type[] targetParm = null) {
        var original = AccessTools.Method(originalType, originalMethod, targetParm);
        var patch = AccessTools.Method(patchType, patchMethod);
        if (original is null) {
            LogManager.GetLogger().Error($"Original method [{originalMethod}] not found");
            return;
        }
        if (patch is null) {
            LogManager.GetLogger().Error($"Patch method [{patchMethod}] not found");
            return;
        }
        switch (patcherType) {
            case PatcherType.Prefix: Harmony.Patch(original, prefix: new HarmonyMethod(patch)); break;
            case PatcherType.Postfix: Harmony.Patch(original, postfix: new HarmonyMethod(patch)); break;
            case PatcherType.Transpiler: Harmony.Patch(original, transpiler: new HarmonyMethod(patch)); break;
        };
        LogManager.GetLogger().Info(PatchInfo(patcherType, original, originalMethod, patch, patchMethod));
    }

    public void LogPatchedMethods() => LogManager.GetLogger().Info(GetPatchedMethods());

    public string GetPatchedMethods() {
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine("Patched Methods:");
        Harmony.GetPatchedMethods().ForEach(_ => stringBuilder.AppendLine($"{_.DeclaringType.FullName}.{_.Name}"));
        return stringBuilder.ToString();
    }

    private string PatchInfo(PatcherType patchType, MethodBase original, string originalMethod, MethodInfo patch, string patchMethod) => $"[{patchType}] [{original.DeclaringType.FullName}.{originalMethod}] patched by [{patch.DeclaringType.FullName}.{patchMethod}]";
}