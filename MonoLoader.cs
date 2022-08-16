namespace Zaza
{
    using UnityEngine;

    public sealed class MonoLoader
    {
        public static void Init()
        {
            (Load = new GameObject()).AddComponent<Zaza>();
            GameObject.DontDestroyOnLoad(Load);
        }

        public static void Dispose()
        {
            ZazaConsole.WriteLine("UNLOADING..");
            {
                ZazaConsole.RemoveCommands();
                ScriptHandler.UnloadScripts();
            } ZazaConsole.WriteLine("UNLOADED!");

            GameObject.Destroy(Load);
            Load = null;
        }

        private static GameObject Load { get; set; } = null;
    }
}
