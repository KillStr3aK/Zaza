namespace Zaza
{
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using UnityEngine;

    using static MessageHud;

    internal sealed partial class Zaza : MonoBehaviour
    {
        void Awake()
        {
            ZazaConsole.WriteLine($"Injected version: <color=yellow>{ZazaVersion.GetCheatVersion()}</color>");
            ZazaConsole.WriteLine("Developers: <color=purple>Nexd @ Eternar</color>");

            ZazaConsole.RegisterCommand("quit", "Quit from the game.", async (args) =>
            {
                ZazaConsole.WriteLine("helo :D");

                await Task.Delay(500);
                Application.Quit();
            });

            ZazaConsole.RegisterCommand("prefab_dump", "[(Optional)filepath] Dump game prefabs.", (args) =>
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;

                if (args.Length == 2)
                {
                    path = args[1];
                }

                try
                {
                    PrefabManager.Dump(path);
                }
                catch (Exception ex)
                {
                    ZazaConsole.Exception(ex);
                }
            });

            ZazaConsole.RegisterCommand("run_script", "[name] [filepath] Run script.", (args) =>
            {
                if (args.Length != 3)
                {
                    args.ReplyError("Syntax Error: /run_script [name] [filepath]");
                    return;
                }

                string name = args[1].ToString();
                string filepath = args[2].ToString();

                args.Reply($"{name} {filepath}");

                try
                {
                    args.Reply($"Creating scripting environment for {name}");
                    /*
                    ScriptContext<object> ctx = ScriptingEnvironment.Create(name, filepath);
                    {
                        args.Reply($"Compiling {name}...");

                        ctx.Compile();

                        args.Reply($"Running {name}...");
                        ctx.RunAsync();
                    }
                    */
                }
                catch (Exception ex)
                {
                    ZazaConsole.Exception(ex);
                }
            });

            ZazaConsole.RegisterCommand("run_lua", "[name] [filepath] Run Lua script.", (args) =>
            {
                if (args.Length != 3)
                {
                    args.ReplyError("Syntax Error: /run_script [name] [filepath]");
                    return;
                }

                string name = args[1].ToString();
                string filepath = args[2].ToString();

                args.Reply($"{name} {filepath}");

                try
                {
                    args.Reply($"Creating scripting environment for {name}");
                    /*
                    using (Lua state = new Lua())
                    {
                        object[] results = state.DoFile(filepath);

                        foreach (var result in results)
                        {
                            ZazaConsole.WriteLine($"{result}");
                        }
                    }
                    */
                }
                catch (Exception ex)
                {
                    ZazaConsole.Exception(ex);
                }
            });

            ZazaConsole.RegisterCommand("unlockdlc", "[(Optional)dlcname] Soft-Unlock the given DLC.", (args) =>
            {
                if (args.Length == 1)
                {
                    List<DLCMan.DLCInfo> dlcs = DLCManager.GetDLCs();
                    string text = "";

                    foreach (DLCMan.DLCInfo dlc in dlcs)
                    {
                        text += $"{dlc.m_name} ";
                    }

                    args.Reply($"Available DLCs: {text}");
                    return;
                }
                else if (args.Length != 2)
                {
                    args.ReplyError("Syntax Error: /unlockdlc [(Optional)dlcname]");
                    return;
                }

                try
                {
                    string dlcName = args[1];
                    DLCMan.DLCInfo dlcInfo = DLCManager.GetDLC(dlcName);

                    if (dlcInfo == null)
                    {
                        args.ReplyError($"The given DLC ({dlcName}) does not exists.");
                        return;
                    }

                    if(DLCManager.IsDLCInstalled(dlcName))
                    {
                        args.Reply($"The given DLC ({dlcName}) is already unlocked.");
                        return;
                    }

                    DLCManager.UnlockDLC(dlcInfo);

                    if (DLCManager.IsDLCInstalled(dlcName))
                    {
                        args.Reply($"Successfully unlocked DLC {dlcName}");
                    }
                    else
                    {
                        args.ReplyError($"Failed to unlock DLC \"{dlcName}\"");
                    }
                }
                catch (Exception ex)
                {
                    ZazaConsole.Exception(ex);
                }
            });

            ZazaConsole.RegisterCommand("prefab_spawn", "[name] [amount] [level] Spawn any game prefab. (Items, mobs, etc.)", (args) =>
            {
                Player localPlayer = GetLocalPlayer();

                if (localPlayer == null)
                {
                    args.ReplyError("You can only spawn prefabs in-game.");
                    return;
                }

                if (args.Length != 4)
                {
                    args.ReplyError("Syntax Error: /prefab_spawn [name] [amount] [level]");
                    return;
                }

                string prefabName = args[1];
                GameObject prefabObject = PrefabManager.GetPrefab(prefabName);

                if (prefabObject == null)
                {
                    args.ReplyError("Invalid prefab! For prefab names, use command 'prefab_dump'");
                    return;
                }

                try
                {
                    int amount = args.TryParameterInt(2);
                    if (amount < 1 || amount > 100)
                    {
                        args.ReplyError("Amount should be between 1 - 100");
                        return;
                    }

                    int level = args.TryParameterInt(3);
                    if (level < 1 || level > 5)
                    {
                        args.ReplyError("Level should be between 1 - 5");
                        return;
                    }

                    DateTime now = DateTime.Now;

                    for (int i = 0; i < amount; i++)
                    {
                        Character characterComponent = PrefabManager.SpawnPrefab(prefabObject, localPlayer.transform.position + localPlayer.transform.forward * 2.0f + Vector3.up + (UnityEngine.Random.insideUnitSphere * 0.5f), Quaternion.identity).GetComponent<Character>();
                        
                        if(characterComponent != null)
                        {
                            characterComponent.SetLevel(level);
                        }
                    }

                    string log = $"Spawned x<color=yellow>{amount}</color> of <color=cyan>{prefabObject.name}</color> (Level <color=orange>{level}</color>) in <color=#5f47d6>{(DateTime.Now - now).TotalMilliseconds}</color> ms";
                    ZazaConsole.WriteLine(log);
                    ZazaChat.WriteLine(log);
                } catch (Exception ex)
                {
                    ZazaConsole.Exception(ex);
                }
            });

            ZazaConsole.RegisterCommand("kill_animals", "[distance] [(Optional)maxamount] Kill entities", (args) =>
            {
                Player localPlayer = GetLocalPlayer();

                if (localPlayer == null)
                {
                    args.ReplyError("You can only kill animals in-game.");
                    return;
                }

                if (args.Length < 2 || args.Length > 3)
                {
                    args.ReplyError("Syntax Error: /kill_animals [distance] [(Optional)maxamount]");
                    return;
                }

                float distance = args.TryParameterFloat(1, 10.0f);
                if(distance < 0.0f)
                {
                    args.ReplyError("Distance must be greater than zero.");
                    return;
                }

                int maxAmount = args.TryParameterInt(2, 10);
                if (maxAmount < 0)
                {
                    args.ReplyError("Max Amount must be greater than zero.");
                    return;
                }

                int result = 0;

                try
                {
                    result = KillMobs(distance, maxAmount);
                } catch (Exception ex)
                {
                    ZazaConsole.Exception(ex);
                }

                args.Reply($"Killed <color=yellow>{result}</color> animals.");
            });

            ZazaConsole.WriteLine($"Registered <color=yellow>{ZazaConsole.Commands.Count}</color> commands.");
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Insert))
            {

            }
        }

        /// <summary>
        /// Get local <see cref="Player"/> instance.
        /// </summary>
        /// <returns>Local <see cref="Player"/> instance</returns>
#nullable enable
        public static Player? GetLocalPlayer()
            => Player.m_localPlayer;
#nullable disable

        public static void ShowBiomeFoundMsg(string text, bool playStinger = false)
        {
            if (GetLocalPlayer() != null)
            {
                MessageHud.instance.ShowBiomeFoundMsg(text, playStinger);
            }
        }

        public static void ShowMessage(MessageType type, string text, int amount = 0, Sprite icon = null)
        {
            if (GetLocalPlayer() != null)
            {
                MessageHud.instance.ShowMessage(type, text, amount, icon);
            }
        }

        public static void MessageAll(MessageType type, string text)
        {
            if (GetLocalPlayer() != null)
            {
                MessageHud.instance.MessageAll(type, text);
            }
        }

        public static int KillMobs(float maxDistance, int maxAmount = -1)
        {
            Player localPlayer = GetLocalPlayer();

            int amount = 0;
            List<Character> characters = Character.GetAllCharacters();
            HitData hit = new HitData();
            hit.m_damage.m_damage = 9999.9f;

            foreach(Character character in characters)
            {
                if (character.IsPlayer())
                    continue;

                if (Vector3.Distance(localPlayer.transform.position, character.transform.position) > maxDistance)
                    continue;

                localPlayer.Attack(character, hit);
                ++amount;

                if (maxAmount != -1 && amount == maxAmount)
                    break;
            }

            return amount;
        }
    }
}
