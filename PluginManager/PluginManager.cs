using System;
using System.Runtime.InteropServices;
using PluginManager.SDK;
using Microsoft.VisualBasic;
using RGiesecke.DllExport;

namespace PluginManager
{
    public class PluginManager
    {
        public static bool PluginInit(Plugins.PLUG_INITSTRUCT initStruct)
        {
            Console.SetOut(new TextWriterPlugin());
            return true;
        }

        public static void PluginStop()
        {
        }

        public static void PluginSetup(Plugins.PLUG_SETUPSTRUCT setupStruct)
        {
        }

        [DllExport("CBMENUENTRY", CallingConvention.Cdecl)]
        public static void CBMENUENTRY(Plugins.CBTYPE cbType, ref Plugins.PLUG_CB_MENUENTRY info)
        {
        }
    }
}
