using ColossalFramework.UI;
using CSShared.Common;
using CSShared.Debug;
using CSShared.Tools;
using System;
using UnifiedUI.Helpers;
using UnityEngine;

namespace CSShared.ToolButton;

public abstract class UUIToolManagerBase<TypeInGameToolButton, TypeMod, TypeConfig> : ITool where TypeInGameToolButton : ToolButtonBase<TypeConfig> where TypeMod : IMod where TypeConfig : SingletonConfig<TypeConfig>, new() {
    public Action EnableCallback;
    public Action DisableCallback;

    public TypeInGameToolButton InGameToolButton { get; private set; }
    public bool UUISupport => PluginTools.IsPluginEnabled("UnifiedUIMod");
    protected UUICustomButton UUIButton { get; set; }
    protected abstract Texture2D UUIIcon { get; }
    public bool UUIRegistered { get; protected set; }
    protected abstract string Tooltip { get; }
    public bool UUIButtonIsPressed {
        set {
            if (UUIButton is not null) {
                UUIButton.IsPressed = value;
            }
        }
    }

    public void OnCreated() { }

    public void OnReleased() => Disable();

    public void Reset() {
        Disable();
        Enable();
    }

    public void Enable() {
        if (UUISupport) {
            if (SingletonConfig<TypeConfig>.Instance.ToolButtonPresent == ToolButtonPresent.InGame) {
                AddInGameButton();
            }
            else if (SingletonConfig<TypeConfig>.Instance.ToolButtonPresent == ToolButtonPresent.UUI) {
                RegisterUUI();
            }
            else if (SingletonConfig<TypeConfig>.Instance.ToolButtonPresent == ToolButtonPresent.Both) {
                AddInGameButton();
                RegisterUUI();
            }
        }
        else {
            EnsurePresent();
            if (SingletonConfig<TypeConfig>.Instance.ToolButtonPresent == ToolButtonPresent.InGame)
                AddInGameButton();
        }
        EnableCallback?.Invoke();
    }

    public void Disable() {
        if (UUISupport) {
            LogoutUUI();
        }
        RemoveInGameButton();
        DisableCallback?.Invoke();
    }

    protected virtual void EnsurePresent() {
        if (!UUISupport && (SingletonConfig<TypeConfig>.Instance.ToolButtonPresent == ToolButtonPresent.UUI || SingletonConfig<TypeConfig>.Instance.ToolButtonPresent == ToolButtonPresent.Both)) {
            SingletonConfig<TypeConfig>.Instance.ToolButtonPresent = ToolButtonPresent.InGame;
            SingletonMod<TypeMod>.Instance.SaveConfig();
        }
    }

    protected void AddInGameButton() {
        LogManager.GetLogger().Info("Adding InGame button");
        InGameToolButton = UIView.GetAView().AddUIComponent(typeof(TypeInGameToolButton)) as TypeInGameToolButton;
        InGameToolButton.tooltipBox = UIView.GetAView()?.defaultTooltipBox;
        InGameToolButton.tooltip = Tooltip;
        InGameToolButton.EventToggleChanged += InGameToolButtonToggle;
    }

    protected void RemoveInGameButton() {
        if (InGameToolButton is null) {
            return;
        }
        LogManager.GetLogger().Info("Removing InGame button");
        InGameToolButton.EventToggleChanged -= InGameToolButtonToggle;
        InGameToolButton.tooltip = Tooltip;
        InGameToolButton.Destroy();
        InGameToolButton = null;
    }

    protected void RegisterUUI() {
        if (UUIRegistered || !UUISupport) {
            return;
        }
        LogManager.GetLogger().Info("UnifiedUI detected, registering UUI");
        UUIButton = UUIHelpers.RegisterCustomButton(AssemblyTools.CurrentAssemblyName, null, Tooltip, UUIIcon, UUIButtonToggle);
        UUIButton.Button.eventTooltipEnter += (c, e) => c.tooltip = Tooltip;
        UUIButton.IsPressed = false;
        UUIRegistered = true;
    }

    protected void LogoutUUI() {
        if (!UUIRegistered || !UUISupport || UUIButton is null) {
            return;
        }
        LogManager.GetLogger(AssemblyTools.CurrentAssemblyName).Info("UnifiedUI detected, logout UUI");
        UUIButton.Button.Destroy();
        UUIButton = null;
        UUIRegistered = false;
    }

    protected abstract void UUIButtonToggle(bool isOn);
    protected abstract void InGameToolButtonToggle(bool isOn);

    public void Update() { }
}