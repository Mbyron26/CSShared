using ColossalFramework.IO;
using CSShared.Debug;
using CSShared.Localization;
using CSShared.Tools;
using System;
using System.IO;

namespace CSShared.Common;

public partial class SingletonConfig<T> : SingletonItem<T>, ILocalizationSetting where T : SingletonConfig<T>, new() {
    public static string ConfigFilePath => Path.Combine(DataLocation.localApplicationData, $"{AssemblyTools.CurrentAssemblyName}Config.xml");
    public string ModVersion { get; set; } = "0.0.0";
    public string LocaleId { get; set; } = ModLocalizationManager.USE_GAME_LANGUAGE;

    public static void Save() {
        try {
            if (SingletonConfig<T>.Instance is not null) {
                XmlTool.SerializeObjectToFile(SingletonConfig<T>.Instance, ConfigFilePath);
            }
        }
        catch (Exception ex) {
            LogManager.GetLogger().Error(ex, $"Saving {ConfigFilePath} failed");
        }
    }

    public static bool Load() {
        LogManager.GetLogger().Info("Loading setting");
        bool isSucceed = true;
        try {
            if (File.Exists(ConfigFilePath)) {
                var instance = XmlTool.DeserializeObjectFromFile<T>(ConfigFilePath);
                if (instance is not null && instance is T) {
                    Instance = instance as T;
                    LogManager.GetLogger().Info("Local settings detected, deserialized");
                }
                else {
                    Instance = new();
                    LogManager.GetLogger().Info("Unable to load the setting file, generate default setting");
                    isSucceed = false;
                }
            }
            else {
                Instance = new();
                LogManager.GetLogger().Info("No settings file found, generate default setting");
            }
        }
        catch (Exception ex) {
            Instance = new();
            LogManager.GetLogger().Error(ex, "Unable to load the setting file, generate default setting");
            isSucceed = false;
        }
        Save();
        return isSucceed;
    }
}