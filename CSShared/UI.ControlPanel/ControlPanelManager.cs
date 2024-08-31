using ColossalFramework.UI;
using CSShared.Common;
using CSShared.Debug;
using CSShared.Tools;
using System;
using UnityEngine;

namespace CSShared.UI.ControlPanel;

public class ControlPanelManager<TypeMod, TypePanel> where TypeMod : IMod where TypePanel : UIComponent {
    private static GameObject panelGameObject;
    private static bool isVisible;
    private static TypePanel panel;

    public static event Action<bool> EventOnVisibleChanged;
    public static event Action<TypePanel> EventPanelCreated;
    public static event Action<TypePanel> EventPanelClosing;

    public static bool IsVisible {
        get => isVisible;
        private set {
            if (value != isVisible) {
                isVisible = value;
                EventOnVisibleChanged?.Invoke(isVisible);
            }
        }
    }

    public static void CallPanel() {
        if (IsVisible) {
            Close();
        }
        else {
#if BETA_DEBUG
            using var performanceCounter = PerformanceCounter.Start(_ => LogManager.GetLogger().Debug($"Control panel create time: {_.ReportMilliseconds}"));
            Create();
#else
            Create();
#endif
        }
    }
    public static void OnLocaleChanged() {
        if (IsVisible) {
            Close();
            Create();
        }
    }
    public static void Create() {
        if (panelGameObject is not null)
            return;
        panelGameObject = new GameObject(AssemblyTools.CurrentAssemblyName + "ControlPanel");
        panelGameObject.transform.parent = UIView.GetAView().transform;
        panel = panelGameObject.AddComponent<TypePanel>();
        //panel.Show();
        EventPanelCreated?.Invoke(panel);
        IsVisible = true;
    }
    public static void Close() {
        if (panelGameObject is null)
            return;
        SingletonMod<TypeMod>.Instance.SaveConfig();
        EventPanelClosing?.Invoke(panel);
        UnityEngine.Object.Destroy(panel);
        UnityEngine.Object.Destroy(panelGameObject);
        panel = null;
        panelGameObject = null;
        IsVisible = false;
    }
}
