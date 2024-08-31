using ICities;

namespace CSShared.UI.MessageBoxes;

public class TwoButtonMessageBox : MessageBoxBase {
    public virtual void Init(string title, string text, OnButtonClicked yesCallback, OnButtonClicked noCallBack = null, bool callCloseAfterAction = true) {
        TitleText = title;
        AddLabelInMainPanel(text);
        AddButtons(1, 2, Localize("MessageBox_OK"), () => {
            if (callCloseAfterAction) {
                yesCallback?.Invoke();
                Close();
            } else {
                yesCallback?.Invoke();
            }
        });
        AddButtons(2, 2, Localize("Cancel"), noCallBack is null ? Close : null);
    }
}

public class OneButtonMessageBox : MessageBoxBase {
    public override void Start() {
        base.Start();
        AddButtons(1, 1, Localize("MessageBox_OK"), Close);
    }

    public virtual void Init(string title, string text) {
        TitleText = title;
        AddLabelInMainPanel(text);
    }
}