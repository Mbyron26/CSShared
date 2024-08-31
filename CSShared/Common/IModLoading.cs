namespace CSShared.Common;

public interface IModLoading {
    bool IsLevelLoaded { get; set; }
    bool LoadedMode { get; set; }
    void ShowLogMessageBox();
}