using ColossalFramework;
using ColossalFramework.Globalization;
using CSShared.Debug;
using CSShared.Manager;
using CSShared.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace CSShared.Localization;

public class ModLocalizationManager : IManager<string, ILocalizationSetting> {
    public const string USE_GAME_LANGUAGE = "UGL";

    public static event Action OnModActiveLocaleIdChanged;
    public static event Func<string, ModLocalizationManager> OnModActiveLocaleIdChanged2;

    private ILog Logger { get; set; }
    public string GameActiveLocaleId => LocaleManager.exists ? GetLocaleId(LocaleManager.instance.language) : GetLocaleId(new SavedString(Settings.localeID, Settings.gameSettingsFile, DefaultSettings.localeID).value);
    public static string ModActiveLocaleId => CurrentLocaleSource.Id;
    private string ModDirectory { get; set; }
    public ILocalizationSetting Localization { get; private set; }
    public static Dictionary<string, LocaleSource> LocaleSources { get; set; }
    public static LocaleSource CurrentLocaleSource { get; private set; }
    public bool Processing { get; private set; }
    public bool IsInitialized { get; private set; }
    public List<string> LocaleIdList { get; private set; } = new();

    private void GetLocaleIdList() {
        LocaleIdList.Clear();
        LocaleIdList.Add(USE_GAME_LANGUAGE);
        LocaleIdList.AddRange(LocaleSources.Keys);
    }

    public void OnResetSettings(ILocalizationSetting localizationSetting) {
        Localization = localizationSetting;
        IsInitialized = false;
        ChangeLocale();
        GetLocaleIdList();
        GetLanguagesOption();
        IsInitialized = true;
    }

    public void OnLanguagesOptionChanged(int index, Action<string> action = null) {
        if (Processing)
            return;
        Processing = true;
        if (!LocaleIdList.Any())
            return;
        string actionString;
        if (index == 0) {
            if (TryGetLocaleSource(GameActiveLocaleId, out var localeSource))
                CurrentLocaleSource = localeSource;
            else
                SetDefaultLocale();
            actionString = USE_GAME_LANGUAGE;
            Logger.Info($"Change locale on languages option changed, use game language, mod active locale: {ModActiveLocaleId}, game active locale: {GameActiveLocaleId}");
        }
        else {
            var localeId = LocaleIdList[index];
            if (TryGetLocaleSource(localeId, out var source))
                CurrentLocaleSource = source;
            else
                SetDefaultLocale();
            actionString = ModActiveLocaleId;
            Logger.Info($"Change locale on languages option changed, customize locale, mod active locale: {ModActiveLocaleId}, game active locale: {GameActiveLocaleId}");
        }
        action?.Invoke(actionString);
        NotifyModActiveLocaleIdChanged();
        Processing = false;
    }

    public string[] GetLanguagesOption() {
        var list = new List<string> {
            Localize("Language_GameLanguage")
        };
        var localeIds = LocaleSources.Keys;
        foreach (var localeId in localeIds) {
            var LocalizedLocaleId = localeId.Replace('-', '_');
            var prefix = Localize($"Language_{LocalizedLocaleId}");
            var suffix = $"({Localize(localeId, $"Language_{LocalizedLocaleId}")})";
            var total = prefix + suffix;
            list.Add(ModActiveLocaleId == localeId ? prefix : total);
        }
        return list.ToArray();
    }

    private void OnLocaleChanged() {
        if (Processing)
            return;
        Processing = true;
        ChangeLocale();
        NotifyModActiveLocaleIdChanged();
        Processing = false;
    }

    private void NotifyModActiveLocaleIdChanged() {
        OnModActiveLocaleIdChanged?.Invoke();
        OnModActiveLocaleIdChanged2?.Invoke(ModActiveLocaleId);
    }

    public static string Localize(string localeId, string key) {
        if (string.IsNullOrEmpty(localeId) || string.IsNullOrEmpty(key))
            throw new ArgumentNullException();
        if (LocaleSources.TryGetValue(localeId, out var source)) {
            if (source.TryGetValue(key, out var value)) {
                return value;
            }
            if (LocaleSources[LocaleSource.EN_LOCALE_ID].TryGetValue(key, out var value2)) {
                LogManager.GetLogger().Info($"Cannot find {key} in {ModActiveLocaleId} source, fallback en-US value");
                return value2;
            }
        }
        LogManager.GetLogger().Info($"Cannot find {key} in  LocaleSources, fallback key");
        return key;
    }

    public static string Localize(string key) {
        if (key is null)
            throw new ArgumentNullException(nameof(key));
        if (CurrentLocaleSource is null || LocaleSources is null) {
            return key;
        }
        if (CurrentLocaleSource.TryGetValue(key, out var value1)) {
            return value1;
        }
        if (LocaleSources[LocaleSource.EN_LOCALE_ID].TryGetValue(key, out var value2)) {
            LogManager.GetLogger().Info($"Cannot find [{key}] in {ModActiveLocaleId} source, fallback en-US value");
            return value2;
        }
        LogManager.GetLogger().Info($"Cannot find [{key}] in LocaleSources, fallback key");
        return key;
    }

    public void ChangeLocale(string localeId, Action action = null) {
        Processing = true;
        if (localeId == USE_GAME_LANGUAGE) {
            if (TryGetLocaleSource(GameActiveLocaleId, out var localeSource))
                CurrentLocaleSource = localeSource;
            else
                SetDefaultLocale();
            Logger.Info($"Change locale, use game language, mod active locale: {ModActiveLocaleId}, game active locale: {GameActiveLocaleId}");
        }
        else {
            if (TryGetLocaleSource(localeId, out var source))
                CurrentLocaleSource = source;
            else
                SetDefaultLocale();
            Logger.Info($"Change locale, customize locale, mod active locale: {ModActiveLocaleId}, game active locale: {GameActiveLocaleId}");
        }
        action?.Invoke();
        NotifyModActiveLocaleIdChanged();
        Processing = false;
    }

    public void ChangeLocale(Action action = null) {
        Processing = true;
        var settingLocaleId = Localization.LocaleId;
        var tag = IsInitialized ? "Change" : "Init";
        if (settingLocaleId == USE_GAME_LANGUAGE) {
            if (TryGetLocaleSource(GameActiveLocaleId, out var localeSource))
                CurrentLocaleSource = localeSource;
            else
                SetDefaultLocale();
            Logger.Info($"{tag} locale, use game language, mod active locale: {ModActiveLocaleId}, game active locale: {GameActiveLocaleId}");
        }
        else {
            if (TryGetLocaleSource(settingLocaleId, out var source))
                CurrentLocaleSource = source;
            else
                SetDefaultLocale();
            Logger.Info($"{tag} locale, customize locale, mod active locale: {ModActiveLocaleId}, game active locale: {GameActiveLocaleId}");
        }
        action?.Invoke();
        Processing = false;
    }

    public bool GameSupportsLocale() {
        foreach (var id in LocaleManager.instance.supportedLocaleIDs) {
            if (LocaleSources.ContainsKey(GetLocaleId(id)))
                return true;
        }
        return false;
    }

    private void SetDefaultLocale() => CurrentLocaleSource = LocaleSources[LocaleSource.EN_LOCALE_ID];

    private bool TryGetLocaleSource(string localeId, out LocaleSource localeSource) => LocaleSources.TryGetValue(localeId, out localeSource);

    private string GetLocaleId(string localeId) {
        if (localeId == "en") {
            localeId = "en-US";
        }
        else if (localeId == "zh") {
            var culture = CultureInfo.InstalledUICulture.Name;
            if (culture == "zh-TW" || culture == "zh-HK") {
                localeId = "zh-TW";
            }
            else {
                localeId = "zh-CN";
            }
        }
        else if (localeId == "es") {
            localeId = "es-ES";
        }
        else if (localeId == "pt") {
            localeId = "pt-BR";
        }
        return localeId;
    }

    private void LoadAllLocaleSources() {
        StringBuilder stringBuilder = new();
        stringBuilder.Append("Added mod locale source: ");
        LocaleSources.Clear();
        var directory = Path.Combine(ModDirectory, "Localization");
        if (!Directory.Exists(directory))
            Logger.Warn("Localization folder not found");
        else {
            foreach (var file in new DirectoryInfo(directory).GetFiles("*.json")) {
                var localeID = Path.GetFileNameWithoutExtension(file.Name);
                if (!string.IsNullOrEmpty(localeID)) {
                    var source = JsonTool.DeserializeObjectFromFile<Dictionary<string, string>>(file.FullName);
                    LocaleSources.Add(localeID, new(localeID, source));
                    stringBuilder.Append($"{localeID} ");
                }
            }
            Logger.Info(stringBuilder.ToString());
        }
        stringBuilder.Remove(0, stringBuilder.Length);
        stringBuilder.Append($"Added common locale source: ");
        directory = Path.Combine(directory, "Common");
        if (!Directory.Exists(directory))
            Logger.Warn("Localization/Common folder not found");
        else {
            foreach (var file in new DirectoryInfo(directory).GetFiles("*.json")) {
                var localeID = Path.GetFileNameWithoutExtension(file.Name);
                if (!string.IsNullOrEmpty(localeID) && LocaleSources.TryGetValue(localeID, out var localeISource)) {
                    var source = JsonTool.DeserializeObjectFromFile<Dictionary<string, string>>(file.FullName);
                    localeISource.Add(source);
                    stringBuilder.Append($"{localeID} ");
                }
            }
            Logger.Info(stringBuilder.ToString());
        }
    }

    public void OnCreated(string t1, ILocalizationSetting t2) {
        ModDirectory = t1;
        Localization = t2;
        Logger = LogManager.GetLogger();
        LocaleSources = new();
        LoadAllLocaleSources();
        GetLocaleIdList();
        ChangeLocale();
        LocaleManager.eventLocaleChanged += OnLocaleChanged;
        IsInitialized = true;
    }

    public void OnCreated() { }

    public void OnReleased() {
        LocaleManager.eventLocaleChanged -= OnLocaleChanged;
        LocaleSources = null;
        IsInitialized = false;
    }

    public void Reset() {
        IsInitialized = false;
        LocaleManager.eventLocaleChanged -= OnLocaleChanged;
        LocaleSources.Clear();
        LoadAllLocaleSources();
        GetLocaleIdList();
        ChangeLocale();
        LocaleManager.eventLocaleChanged += OnLocaleChanged;
        IsInitialized = true;
    }

    public void Update() { }
}