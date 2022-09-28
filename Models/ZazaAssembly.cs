namespace Zaza
{
    using Nexd.Reflection;
    using System.Reflection;

    /// <summary>
    /// Wrapper class to access internal game methods.
    /// </summary>
    public sealed class ZazaAssembly
    {
        internal static Assembly Valheim { get; set; }

        static ZazaAssembly()
            => Valheim = Pumped.GetAssembly("assembly_valheim");

        public static string GetCheatVersion()
            => "1.0";

        public static string GetGameVersion()
        {
            string version = Invoke<string>("Version", "GetVersionString");

            if (!string.IsNullOrEmpty(version))
                return version;

            return "unknown";
        }

#nullable enable
        private static T? Invoke<T>(string @class, string method, params object[]? args)
        {
            if (Valheim != null)
            {
                return Pumped.InvokeInternal<T>(Valheim, @class, method, args);
            }

            return default(T);
        }

        private static void InvokeVoid<T>(string @class, string method, params object[]? args)
        {
            if (Valheim != null)
            {
                Pumped.InvokeInternalVoid(Valheim, @class, method, args);
            }
        }
#nullable disable
    }
}
