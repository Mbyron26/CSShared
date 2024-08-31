using ColossalFramework.PlatformServices;
using ColossalFramework.Plugins;
using CSShared.Common;
using CSShared.Debug;
using CSShared.Localization;
using CSShared.Tools;
using CSShared.UI.MessageBoxes;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSShared.Manager;

public class CompatibilityManager : IManager<string, List<ConflictModInfo>> {
    private string ModName { get; set; } = string.Empty;
    public List<ConflictModInfo> ConflictMods { get; set; }
    public List<ConflictModInfo> DetectedConflictMods { get; private set; }

    private bool RemoveConflictMods() {
        List<bool> result = new();
        foreach (var item in DetectedConflictMods) {
            result.Add(RemoveConflictMod(item));
        }
        return result.TrueForAll(x => true);
    }

    private bool RemoveConflictMod(ConflictModInfo mod) {
        var flag = PlatformService.workshop.Unsubscribe(new PublishedFileId(mod.ModID));
        if (flag) {
            LogManager.GetLogger().Info($"Unsubscribed conflict mod succeeded: {mod.Name}");
        }
        else {
            LogManager.GetLogger().Info($"Unsubscribed conflict mod failed: {mod.Name}");
        }
        return flag;
    }

    private void CheckConflictMods() {
        if (ConflictMods is null || !ConflictMods.Any() || DetectedConflictMods is null)
            return;
        DetectedConflictMods.Clear();
        foreach (PluginManager.PluginInfo info in PluginTools.GetPluginsInfoSortByName()) {
            if (info is not null /*&& info.userModInstance is IUserMod*/) {
                for (int i = 0; i < ConflictMods.Count; i++) {
                    if (info.publishedFileID.AsUInt64 == ConflictMods[i].ModID) {
                        DetectedConflictMods.Add(ConflictMods[i]);
                    }
                }
            }
        }
        if (DetectedConflictMods.Count > 0) {
            StringBuilder stringBuilder = new();
            stringBuilder.Append($"Detected conflict mods: ");
            foreach (var item in DetectedConflictMods) {
                stringBuilder.Append($"[{item.Name}] ");
            }
            LogManager.GetLogger().Warn(stringBuilder.ToString());
        }
    }


    public void ShowMessageBox() {
        CheckConflictMods();
        MessageBox.Show<CompatibilityMessageBox>().Init(ModName, DetectedConflictMods, DisableAction);
    }

    private void DisableAction(MessageBoxBase messageBoxBase) {
        MessageBox.Hide(messageBoxBase);
        var result = RemoveConflictMods();
        CheckConflictMods();
        if (result) {
            DetectedConflictMods.Clear();
            MessageBox.Show<CompatibilityMessageBox>().Init(ModName, DetectedConflictMods, first: true);
        }
        else {
            MessageBox.Show<OneButtonMessageBox>().Init($"{ModName} {ModLocalizationManager.Localize("OptionPanel_CompatibilityCheck")}", ModLocalizationManager.Localize("CompatibilityCheckRequestRestart"));
        }
    }


    public void OnCreated(string t1, List<ConflictModInfo> t2) {
        ModName = t1;
        ConflictMods = t2;
        DetectedConflictMods = new();
        CheckConflictMods();
        if (DetectedConflictMods.Any()) {
            MessageBox.Show<CompatibilityMessageBox>().Init(ModName, DetectedConflictMods, DisableAction);
        }
    }

    public void Reset() {
        DetectedConflictMods.Clear();
    }

    public void Update() { }
    public void OnCreated() {  }
    public void OnReleased() { }
}