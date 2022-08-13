namespace Zaza
{
    using Nexd.Reflection;
    using System.Reflection;

    public sealed class ZazaVersion
    {
        public static string GetCheatVersion()
            => "1.0";

        public static string GetGameVersion()
        {
            Assembly valheim = Pumped.GetAssembly("assembly_valheim");
            if(valheim != null)
            {
                return Pumped.InvokeInternal<string>(valheim, "Version", "GetVersionString", null);
            }

            return "unknown";
        }
    }
}
