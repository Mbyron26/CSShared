using CSShared.Common;
using CSShared.Debug;
using ICities;

namespace CSShared.Extension;

public abstract class ModLoadingExtension<T> : LoadingExtensionBase where T : IModLoading {
    public virtual void Created(ILoading loading) { }
    public virtual void LevelLoaded(LoadMode mode) { }
    public virtual void LevelUnloading() { }
    public virtual void Released() { }

    public sealed override void OnCreated(ILoading loading) {
        base.OnCreated(loading);
        LogManager.GetLogger().Info("Call loading OnCreated");
        Created(loading);
    }

    public sealed override void OnLevelLoaded(LoadMode mode) {
        SingletonMod<T>.Instance.IsLevelLoaded = true;
        LogManager.GetLogger().Info("Call loading OnLevelLoaded");
        LevelLoaded(mode);
        SingletonMod<T>.Instance.ShowLogMessageBox();
    }

    public sealed override void OnLevelUnloading() {
        SingletonMod<T>.Instance.IsLevelLoaded = false;
        LogManager.GetLogger().Info("Call loading OnLevelUnloading");
        LevelUnloading();
    }

    public sealed override void OnReleased() {
        LogManager.GetLogger().Info("Call loading OnReleased");
        Released();
    }
}
