namespace Zaza.SDK
{
    using System.Collections.Generic;

    using Nexd.Reflection;

    using static DLCMan;

    public sealed class DLCManager
    {
        public static bool IsDLCInstalled(DLCInfo dlcInfo)
            => Pumped.Invoke<bool, DLCMan>(DLCMan.instance, "IsDLCInstalled", dlcInfo);

        public static bool IsDLCInstalled(uint id)
            => Pumped.Invoke<bool, DLCMan>(DLCMan.instance, "IsDLCInstalled", id);

        public static void CheckDLCsSTEAM()
            => Pumped.InvokeVoid<DLCMan>(DLCMan.instance, "CheckDLCsSTEAM");

        public static List<DLCInfo> GetDLCs()
            => DLCMan.instance.m_dlcs;

        public static bool IsDLCInstalled(string name)
            => DLCMan.instance.IsDLCInstalled(name);

        public static void UnlockDLC(DLCInfo dlcInfo)
            => dlcInfo.m_installed = true;

        public static void LockDLC(DLCInfo dlcInfo)
            => dlcInfo.m_installed = false;

#nullable enable
        public static DLCInfo? GetDLC(string name)
            => GetDLCs().Find(x => x.m_name == name);
#nullable disable
    }
}
