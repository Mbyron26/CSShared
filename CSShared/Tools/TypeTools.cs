using System.Reflection;

namespace CSShared.Tools;

public static class TypeTools {
    public static T GetFieldValue<T>(object obj, string fieldName) => (T)(obj.GetType().GetField(fieldName)).GetValue(obj);
    public static void SetFieldValue<T>(object obj, string fieldName, BindingFlags bindingFlags, T value) => obj.GetType().GetField(fieldName, bindingFlags).SetValue(obj, value);
    public static object GetFieldValue(object obj, string fieldName) => obj.GetType().GetField(fieldName).GetValue(obj);
    public static void SetFieldValue(object obj, string fieldName, BindingFlags bindingFlags, object value) => obj.GetType().GetField(fieldName, bindingFlags).SetValue(obj, value);
}