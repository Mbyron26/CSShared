using ColossalFramework;
using ICities;

namespace CSShared.Manager;

public class GameInfoManager : IManager {
    public ItemClass.Availability GetGameMode() => Singleton<ToolManager>.instance.m_properties.m_mode;

    public AppMode GetAppMode() {
        ItemClass.Availability mode = GetGameMode();
        switch (mode) {
            case ItemClass.Availability.Game:
                return AppMode.Game;
            case ItemClass.Availability.MapEditor:
                return AppMode.MapEditor;
            default:
                if (mode != ItemClass.Availability.ScenarioEditor) {
                    return AppMode.Game;
                }
                return AppMode.ScenarioEditor;
            case ItemClass.Availability.AssetEditor:
                return AppMode.AssetEditor;
            case ItemClass.Availability.ThemeEditor:
                return AppMode.ThemeEditor;
        }
    }


    public void Reset() { }
    public void Update() { }
    public void OnCreated() { }
    public void OnReleased() { }
}