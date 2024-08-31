using ColossalFramework.UI;
using CSShared.Debug;
using CSShared.Extension;
using CSShared.Localization;
using CSShared.Manager;
using CSShared.Tools;
using CSShared.UI.MessageBoxes;
using ICities;
using System;
using System.Collections.Generic;

namespace CSShared.Common;

public abstract class ModBase<TypeMod, TypeConfig> : IMod where TypeMod : ModBase<TypeMod, TypeConfig> where TypeConfig : SingletonConfig<TypeConfig>, new() {
    private ILog Logger { get; } = LogManager.GetLogger();
    public abstract string ModName { get; }
    public virtual Version ModVersion => AssemblyTools.CurrentAssemblyVersion;
    public abstract ulong StableID { get; }
    public virtual ulong? BetaID { get; }
    public abstract BuildVersion VersionType { get; }
    public string Name => VersionType switch {
        BuildVersion.BetaDebug or BuildVersion.BetaRelease => ModName + " [BETA] " + ModVersion.GetString(),
        _ => ModName + ' ' + ModVersion.GetString(),
    };
    public virtual string Description => string.Empty;
    public bool IsEnabled { get; private set; }
    public abstract List<ChangelogInfo> Changelog { get; }
    public virtual List<ConflictModInfo> ConflictMods { get; set; }
    public bool IsLevelLoaded { get; set; }
    public bool LoadedMode { get; set; }

    public ModBase() {
        Logger.Info("Initializing mod");
        SingletonMod<TypeMod>.Instance = (TypeMod)this;
        var loadResult = LoadConfig();
        ManagerPool.GetOrCreateManager<ModLocalizationManager, string, ILocalizationSetting>(AssemblyTools.CurrentAssemblyDirectory, SingletonConfig<TypeConfig>.Instance);
        if (!loadResult) {
            LoadingManager.instance.m_introLoaded += () => MessageBox.Show<OneButtonMessageBox>().Init(Name, Localize("XMLWariningMessageBox_Warning"));
        }
        ManagerPool.GetOrCreateManager<GameInfoManager>();
    }

    public void OnSettingsUI(UIHelperBase helper) {
        Logger.Info("Setting UI");
        SettingsUI(helper);
    }
    protected virtual void SettingsUI(UIHelperBase helper) { }

    public string Localize(string value) => ModLocalizationManager.Localize(value);

    public bool LoadConfig() => SingletonConfig<TypeConfig>.Load();
    public void SaveConfig() => SingletonConfig<TypeConfig>.Save();

    public void OnEnabled() {
        Logger.Info("Enabled");
        IsEnabled = true;
        if (UIView.GetAView() != null)
            CallIntroActions();
        else
            LoadingManager.instance.m_introLoaded += CallIntroActions;
        Enable();
    }

    public void OnDisabled() {
        Logger.Info("Disabled");
        IsEnabled = false;
        Disable();
    }

    private void CallIntroActions() {
        
        ConflictMods ??= new List<ConflictModInfo>();
        ManagerPool.GetOrCreateManager<CompatibilityManager, string, List<ConflictModInfo>>(ModName, ConflictMods);
        Logger.Info("Call intro actions");
        IntroActions();
    }

    protected virtual void Enable() { }
    protected virtual void Disable() { }
    public virtual void IntroActions() { }

    public void ShowLogMessageBox() {
        var lastVersion = new Version(SingletonConfig<TypeConfig>.Instance.ModVersion);
        if ((VersionType != BuildVersion.StableRelease) && (VersionType != BuildVersion.BetaRelease)) {
            SingletonConfig<TypeConfig>.Instance.ModVersion = ModVersion.ToString();
            SaveConfig();
            return;
        }
        if ((lastVersion.Major == ModVersion.Major) && (lastVersion.Minor == ModVersion.Minor) && (lastVersion.Build == ModVersion.Build)) {
            SingletonConfig<TypeConfig>.Instance.ModVersion = ModVersion.ToString();
            SaveConfig();
            return;
        }
        if (lastVersion < ModVersion) {
            MessageBox.Show<LogMessageBox>().Initialize<TypeMod>(true);
        }
        SingletonConfig<TypeConfig>.Instance.ModVersion = ModVersion.ToString();
        SaveConfig();
    }
}