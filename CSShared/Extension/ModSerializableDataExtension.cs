using CSShared.Debug;
using ICities;

namespace CSShared.Extension;

public abstract class ModSerializableDataExtension : SerializableDataExtensionBase {
    public virtual void Created(ISerializableData serializableData) { }
    public virtual void LoadData() { }
    public virtual void SaveData() { }
    public virtual void Released() { }

    public sealed override void OnCreated(ISerializableData serializableData) {
        base.OnCreated(serializableData);
        LogManager.GetLogger().Info("Call serializable data OnCreated");
        Created(serializableData);
    }

    public sealed override void OnLoadData() {
        LogManager.GetLogger().Info("Call serializable data OnLoadData");
        LoadData();
    }

    public sealed override void OnSaveData() {
        LogManager.GetLogger().Info("Call serializable data OnSaveData");
        SaveData();
    }

    public sealed override void OnReleased() {
        LogManager.GetLogger().Info("Call serializable data OnReleased");
        Released();
    }
}