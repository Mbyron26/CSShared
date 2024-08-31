using CitiesHarmony.API;
using CSShared.Common;
using CSShared.Debug;

namespace CSShared.Patch;

public abstract class ModPatcherBase<TypeMod, TypeConfig> : ModBase<TypeMod, TypeConfig> where TypeMod : ModBase<TypeMod, TypeConfig> where TypeConfig : SingletonConfig<TypeConfig>, new() {
    public virtual string HarmonyID { get; set; } = string.Empty;
    public bool IsPatched { get; private set; }
    public HarmonyPatcher Patcher { get; private set; }

    protected override void Enable() {
        PatchAll();
        base.Enable();
    }
    protected override void Disable() => UnpatchAll();

    protected virtual void PatchAll() {
        if (IsPatched) return;
        if (HarmonyHelper.IsHarmonyInstalled) {
            LogManager.GetLogger().Info("Starting Harmony patches");
            Patcher = new HarmonyPatcher(HarmonyID);
            Patcher.Harmony.PatchAll();
            PatchAction(Patcher);
            IsPatched = true;
            LogManager.GetLogger().Info("Harmony patches completed");
        }
        else {
            LogManager.GetLogger().Error("Harmony is not installed correctly");
        }
    }
    protected virtual void UnpatchAll() {
        if (!IsPatched || !HarmonyHelper.IsHarmonyInstalled)
            return;
        LogManager.GetLogger().Info("Reverting Harmony patches");
        Patcher.Harmony.UnpatchAll(HarmonyID);
        Patcher = null;
        IsPatched = false;
    }
    protected virtual void PatchAction(HarmonyPatcher harmonyPatcher) { }

}