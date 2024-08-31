using CSShared.Common;
using CSShared.Manager;
using CSShared.ToolButton;
using UnityEngine;

namespace CSShared.UI.OptionPanel;

public partial class OptionPanelBase<TypeMod, TypeConfig, TypeOptionPanel> {
    protected virtual void AddToolButtonOptions<T>() where T : class, ITool, new() {
        OptionPanelHelper.AddGroup(GeneralContainer, Localize("ToolButton"));
        var uuiSupport = ManagerPool.GetOrCreateManager<T>().UUISupport;
        var array = uuiSupport ? new string[] { Localize("None"), Localize("OnlyInGame"), Localize("OnlyInUUI"), Localize("Both") } : new string[] { Localize("None"), Localize("OnlyInGame"), };
        if (!uuiSupport) {
            SingletonItem<TypeConfig>.Instance.ToolButtonPresent = (ToolButtonPresent)Mathf.Clamp((int)SingletonItem<TypeConfig>.Instance.ToolButtonPresent, 0, 1);
        }
        OptionPanelHelper.AddDropDown(Localize("ToolButtonDisplay"), null, array, (int)SingletonItem<TypeConfig>.Instance.ToolButtonPresent, 300, 30, ToolButtonDropDownCallBack);
        OptionPanelHelper.Reset();
    }

    protected virtual void ToolButtonDropDownCallBack(int value) => SingletonItem<TypeConfig>.Instance.ToolButtonPresent = (ToolButtonPresent)value;

}
