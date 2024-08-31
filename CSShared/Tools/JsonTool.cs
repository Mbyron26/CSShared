using JsonFx.Json;
using System;
using System.IO;

namespace CSShared.Tools;

public static class JsonTool {
    public static JsonReader Reader { get; set; }
    public static JsonWriter Writer { get; set; }

    static JsonTool() {
        Reader = new();
        Writer = new();
    }

    public static string SerializeObjectToJson<T>(T obj) {
        if (obj is null)
            throw new ArgumentNullException("Target object cannot be null when serialize object to json");
        return Writer.Write(obj);
    }

    public static T DeserializeObjectFromJson<T>(string json) {
        if (string.IsNullOrEmpty(json))
            throw new ArgumentNullException("Json string cannot be null or empty when deserialize object from json");
        return Reader.Read<T>(json);
    }

    public static void SerializeObjectToFile<T>(T obj, string filePath) {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentNullException("File path cannot be null or empty whenserialize object to file");
        var json = SerializeObjectToJson(obj);
        File.WriteAllText(filePath, json);
    }

    public static T DeserializeObjectFromFile<T>(string filePath) {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentNullException("File path cannot be null or empty whenserialize object to file");
        string json = File.ReadAllText(filePath);
        return DeserializeObjectFromJson<T>(json);
    }
}