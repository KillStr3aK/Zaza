namespace Zaza
{
    using System.Threading.Tasks;

    using UnityEngine;

    internal sealed partial class Zaza : MonoBehaviour
    {
        void Awake()
        {
            ZazaConsole.WriteLine($"Injected version: <color=yellow>{ZazaVersion.GetCheatVersion()}</color>");
            ZazaConsole.WriteLine("Developers: <color=purple>Nexd @ Eternar</color>");

            ZazaConsole.RegisterCommand("quit", "Quit from the game.", async (args) =>
            {
                args.Reply("helo :D");
                await Task.Delay(500);
                Application.Quit();
            });

            ZazaConsole.RegisterCommand("prefab_dump", "[(Optional)filepath] Dump game prefabs.", Commands.PrefabDump);

            ZazaConsole.RegisterCommand("run_script", "[name] [filepath] Run script.", Commands.RunScript);

            ZazaConsole.RegisterCommand("run_lua", "[name] [filepath] Run Lua script.", Commands.RunLua);

            ZazaConsole.RegisterCommand("unlockdlc", "[(Optional)dlcname] Soft-Unlock the given DLC.", Commands.UnlockDLC);

            ZazaConsole.RegisterCommand("prefab_spawn", "[name] [amount] [level] Spawn any game prefab. (Items, mobs, etc.)", Commands.PrefabSpawn);

            ZazaConsole.RegisterCommand("kill_mobs", "[distance] [(Optional)maxamount] Kill entities", Commands.KillMobs);

            ZazaConsole.WriteLine($"Registered <color=yellow>{ZazaConsole.Commands.Count}</color> commands.");
        }
    }
}
