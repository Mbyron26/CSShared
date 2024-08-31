using ColossalFramework.Plugins;
using ColossalFramework.UI;
using System;
using System.Reflection;

namespace CSShared.Tools;

public static class AssemblyTools {
    public static string CurrentAssemblyName { get; } = GetCurrentAssemblyName();
    public static Version CurrentAssemblyVersion { get; } = GetCurrentAssemblyVersion();
    public static string CurrentAssemblyDirectory { get; } = GetCurrentAssemblyDirectory();

    private static string GetCurrentAssemblyName() => Assembly.GetExecutingAssembly().GetName().Name;
    private static Version GetCurrentAssemblyVersion() => Assembly.GetExecutingAssembly().GetName().Version;
    private static string GetCurrentAssemblyDirectory() {
        foreach (var item in PluginManager.instance.GetPluginsInfo()) {
            if (item.assembliesString.Contains(CurrentAssemblyName))
                return item.modPath;
        }
        return null;
    }
    public static Assembly GetAssembly(string name) {
        Assembly assembly = null;
        PluginManager.instance.GetPluginsInfo().ForEach(plugin => {
            if (plugin.isEnabled) {
                plugin.GetAssemblies().ForEach(a => {
                    if (a.GetName().Name.Equals(name))
                        assembly = a;
                });
            }
        });
        return assembly;
    }
}