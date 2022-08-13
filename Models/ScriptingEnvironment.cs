namespace Zaza
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Collections.Generic;

    using Nexd.Reflection;

    internal static class PluginManager
    {
        private static AppDomain AppDomain { get; set; }

        private static Dictionary<string, ZazaPlugin> Plugins { get; set; } = new Dictionary<string, ZazaPlugin>();

        public static void CreateEnvironment()
            => AppDomain = AppDomain.CurrentDomain;

        public static void LoadPlugins()
        {
            if (!Directory.Exists("Zaza"))
            {
                return;
            }

            foreach (string filePath in Directory.GetFiles("Zaza"))
            {
                ZazaConsole.WriteLine($"Loading plugin from {filePath}...");
                Assembly assembly = Assembly.LoadFrom(filePath);
                AssemblyName assemblyName = assembly.GetName();
                Plugins[assemblyName.Name] = new ZazaPlugin(assembly, assemblyName);
            }
        }
    }

    public enum PluginState : uint
    {
        Unknown = 0x0,

        Loading = 0x1,

        Failed = 0x2,

        Loaded = 0x3
    }

    public class ZazaPlugin
    {
        internal Assembly Assembly { get; set; }

        internal AssemblyName AssemblyName { get; set; }

        public PluginState State { get; set; } = PluginState.Unknown;

        internal ZazaPlugin(Assembly assembly, AssemblyName assemblyName)
        {
            this.Assembly = assembly;
            this.AssemblyName = assemblyName;

            this.Initialize();
        }

        public string GetName()
            => this.AssemblyName.Name;

#nullable enable
        internal void Initialize()
        {
            this.State = PluginState.Loading;

            Type? type = Pumped.FindDerivedType(this.Assembly, typeof(ZazaPlugin));
            if(type == null)
            {
                this.State = PluginState.Failed;
                throw new PluginException($"Plugin {this.GetName()} does not have any class that inherits from 'ZazaPlugin'.");
            }

            MethodInfo? methodInfo = type.GetMethod("OnPluginStart");
            if(methodInfo == null)
            {
                this.State = PluginState.Failed;
                throw new PluginException($"Plugin {this.GetName()} does not implement OnPluginStart method.");
            }

            object instance = Activator.CreateInstance(type);
            this.State = (PluginState)methodInfo.Invoke(instance, null);
        }
#nullable disable
    }
}
