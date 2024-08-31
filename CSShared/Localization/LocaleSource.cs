using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CSShared.Localization;

public class LocaleSource : IEnumerable {
    public const string EN_LOCALE_ID = "en-US";
    public string Id { get; set; }
    public Dictionary<string, string> Source { get; set; }
    public int Count => Source.Count;
    public bool IsDefault { get; private set; }

    public LocaleSource(string id) {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentNullException("LocaleSource Id cannot be null or empty when init LocaleSource");
        Id = id;
        Source = new Dictionary<string, string>();
    }
    public LocaleSource(string id, Dictionary<string, string> source) {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentNullException("LocaleSource Id cannot be null or empty when init LocaleSource");
        Id = id;
        if (source is null)
            throw new ArgumentNullException("LocaleSource Source cannot be null or empty when init LocaleSource");
        Source = source;
    }
    public LocaleSource(string id, Dictionary<string, string> source, bool isDefault) : this(id, source) {
        IsDefault = isDefault;
    }
    public LocaleSource(Dictionary<string, string> source) {
        Id = EN_LOCALE_ID;
        if (source is null)
            throw new ArgumentNullException("LocaleSource Source cannot be null or empty when init LocaleSource");
        Source = source;
        IsDefault = true;
    }

    public string GetLocalizedId() => GetValue(Id);
    public bool Add(LocaleSource localeSource) {
        if (localeSource is null || localeSource.Id != Id)
            throw new ArgumentException("The source ID must be the same as the target ID when add source");
        return Add(localeSource.Source);
    }
    public bool Add(string key, string value) {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException("Locale key cannot be null or empty when add source");
        Source.Add(key, value);
        return true;
    }
    public bool Add(Dictionary<string, string> source) {
        if (source is null)
            return false;
        foreach (var e in source) {
            if (!Source.ContainsKey(e.Key))
                Source.Add(e.Key, e.Value);
        }
        return true;
    }
    public bool TryGetValue(string entry, out string value) => Source.TryGetValue(entry, out value);
    public bool ContainsKey(string entry) => Source.ContainsKey(entry);
    public void Clear() => Source.Clear();
    public bool MergeFrom(Dictionary<string, string> source) {
        if (source is null)
            return false;
        foreach (var item in source) {
            if (!Source.ContainsKey(item.Key)) {
                Source.Add(item.Key, item.Value);
            }
        }
        return true;
    }
    public bool MergeFrom(LocaleSource localeSource) {
        if (localeSource is null || string.IsNullOrEmpty(localeSource.Id) || localeSource.Source is null || !localeSource.Source.Any())
            return false;
        foreach (var item in localeSource.Source) {
            if (!Source.ContainsKey(item.Key)) {
                Source.Add(item.Key, item.Value);
            }
        }
        return true;
    }
    public string GetValue(string localeKey) {
        if (string.IsNullOrEmpty(localeKey)) {
            return string.Empty;
        }
        if (Source.TryGetValue(localeKey, out var value)) {
            return value;
        }
        return string.Empty;
    }
    public IEnumerator GetEnumerator() => Source.GetEnumerator();
}