using ColossalFramework;
using CSShared.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSShared.UI.MessageBoxes;

public class CompatibilityMessageBox : MessageBoxBase {
    private string ModName { get; set; } = string.Empty;

    public void Init(string modName, List<ConflictModInfo> conflictModsInfo, Action<MessageBoxBase> disableAction = null, bool first = false) {
        ModName = modName;
        TitleText = $"{modName} {Localize("OptionPanel_CompatibilityCheck")}";
        if (conflictModsInfo is null || !conflictModsInfo.Any()) {
            AddLabelInMainPanel(first ? Localize("CompatibilityMessageBox_RestartGame") : Localize("MessageBox_NormalPrompt"));
            AddButton(Localize("MessageBox_OK"), Close);
        }
        else {
            AddLabelInMainPanel(Localize("MessageBox_WarningPrompt"));
            conflictModsInfo.ForEach(a => AddItem(a));
            AddButton(Localize("CompatibilityMessageBox_Unsubscribe"), () => disableAction(this));
            AddButton(Localize("Cancel"), Close);
        }
    }

    protected AlphaSinglePropertyPanel AddItem(ConflictModInfo mod) {
        var panel = MainPanel.AddUIComponent<AlphaSinglePropertyPanel>();
        panel.Atlas = CustomUIAtlas.CSSharedAtlas;
        panel.BgSprite = CustomUIAtlas.RoundedRectangle3;
        panel.BgNormalColor = CustomUIColor.CPPrimaryBg;
        panel.Padding = new UnityEngine.RectOffset(10, 10, 10, 10);
        panel.width = MessageBoxParm.ComponentWidth;
        panel.MajorLabelText = mod.Name;
        panel.MinorLabelText = mod.Inclusive ? (ModName + " " + Localize("CompatibilityCheck_SameFunctionality")) : Localize("CompatibilityCheck_Incompatible") + (mod.UseInstead.IsNullOrWhiteSpace() ? string.Empty : string.Format(Localize("CompatibilityCheck_UseInstead"), mod.UseInstead));
        panel.MinorLabelTextScale = 0.9f;
        panel.StartLayout();
        return panel;
    }
}