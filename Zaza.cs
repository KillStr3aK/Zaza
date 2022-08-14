namespace Zaza
{
    using System;
    using System.Threading.Tasks;

    using UnityEngine;

    internal sealed class Zaza : MonoBehaviour
    {
        void Awake()
        {
            ZazaConsole.WriteLine($"Injected version: <color=yellow>{ZazaVersion.GetCheatVersion()}</color>");
            ZazaConsole.WriteLine("Developers: <color=purple>Nexd @ Eternar</color>");

            Zaza.RegisterCommands();

            try
            {
                // Any code that may throw
                ScriptHandler.LoadScriptsFromDirectory("Zaza/Scripts");
            } catch (Exception ex)
            {
                ZazaConsole.Exception(ex);
            }
        }

        void Update()
        {
            ScriptRuntime.Tick();
        }

        private static void RegisterCommands()
        {
            ZazaConsole.RegisterCommand("quit", "Quit from the game.", async (args) =>
            {
                args.Reply("helo :D");
                await Task.Delay(500);
                Application.Quit();
            });

            ZazaConsole.RegisterCommand("prefab_dump", "[(Optional)filepath] Dump game prefabs.", Commands.PrefabDump);

            ZazaConsole.RegisterCommand("load_script", "[filename] Run C# script.", Commands.LoadScript);

            ZazaConsole.RegisterCommand("unlockdlc", "[(Optional)dlcname] Soft-Unlock the given DLC.", Commands.UnlockDLC);

            ZazaConsole.RegisterCommand("prefab_spawn", "[name] [amount] [level] Spawn any game prefab. (Items, mobs, etc.)", Commands.PrefabSpawn);

            ZazaConsole.RegisterCommand("kill_mobs", "[distance] [(Optional)maxamount] Kill entities", Commands.KillMobs);

            ZazaConsole.WriteLine($"Registered <color=yellow>{ZazaConsole.Commands.Count}</color> commands.");
        }
    }
}
