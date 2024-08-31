using CSShared.Common;
using System;
using UnityEngine;

namespace CSShared.UI.MessageBoxes;

public class ResetModWarningMessageBox : MessageBoxBase {
    public void Init<T>(Action callback) where T : IMod {
        TitleText = $"{SingletonMod<T>.Instance.ModName} {Localize("Reset")}";
        AddLabelInMainPanel(Localize("ResetModWarning"));
        AddButtons(1, 2, Localize("MessageBox_OK"), () => callback()).TextNormalColor = Color.red;
        AddButtons(2, 2, Localize("Cancel"), Close);
    }
}

public class ResetModMessageBox : OneButtonMessageBox {
    public void Init<T>(bool isSucceeded = true) where T : IMod {
        TitleText = $"{SingletonMod<T>.Instance.ModName} {Localize("Reset")}";
        if (isSucceeded) {
            AddLabelInMainPanel(Localize("ResetModSucceeded"));
        }
        else {
            AddLabelInMainPanel(Localize("ResetModFailed"));
        }
    }
}