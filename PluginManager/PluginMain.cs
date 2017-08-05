using System.Runtime.InteropServices;
using PluginManager.SDK;
using RGiesecke.DllExport;

namespace PluginManager
{
    public static class PluginMain
    {
        private const string plugin_name = "PluginManager";
        private const int plugin_version = 1;

        [DllExport("pluginit", CallingConvention.Cdecl)]
        public static bool pluginit(ref Plugins.PLUG_INITSTRUCT initStruct)
        {
            Plugins.pluginHandle = initStruct.pluginHandle;
            initStruct.sdkVersion = Plugins.PLUG_SDKVERSION;
            initStruct.pluginVersion = plugin_version;
            initStruct.pluginName = plugin_name;
            return PluginManager.PluginInit(initStruct);
        }

        [DllExport("plugstop", CallingConvention.Cdecl)]
        private static bool plugstop()
        {
            PluginManager.PluginStop();
            return true;
        }

        [DllExport("plugsetup", CallingConvention.Cdecl)]
        private static void plugsetup(ref Plugins.PLUG_SETUPSTRUCT setupStruct)
        {
            PluginManager.PluginSetup(setupStruct);
        }
    }
}
