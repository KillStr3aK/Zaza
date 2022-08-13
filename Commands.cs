namespace Zaza
{
    using System;
    using System.IO;
    using System.Collections.Generic;

    using UnityEngine;

    using global::Zaza.SDK;
    using static Terminal;

    internal sealed class Commands
    {
        public static void PrefabDump(ConsoleEventArgs args)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Zaza";

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
        }

        public static void LoadScript(ConsoleEventArgs args)
        {
            if (args.Length != 2)
            {
                args.ReplyError("Syntax Error: /load_script [filename]");
                return;
            }

            string path = "\\Zaza\\Scripts\\" + args[1].ToString() + ".dll";

            try
            {
                ScriptHandler.LoadScript(path);
            } catch (FileNotFoundException ex)
            {
                args.ReplyError(ex.Message);
            } catch (BadImageFormatException ex)
            {
                args.ReplyError(ex.Message);
            } catch (Exception ex)
            {
                ZazaConsole.Exception(ex);
            }
        }

        public static void UnloadScript(ConsoleEventArgs args)
        {
            if (args.Length != 2)
            {
                args.ReplyError("Syntax Error: /unload_script [filename]");
                return;
            }

            string path = "\\Zaza\\Scripts\\" + args[1].ToString() + ".dll";

            try
            {
                ScriptHandler.UnloadScript(path);
            }
            catch (FileNotFoundException ex)
            {
                args.ReplyError(ex.Message);
            } catch (Exception ex)
            {
                ZazaConsole.Exception(ex);
            }
        }

        public static void UnlockDLC(ConsoleEventArgs args)
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

                if (DLCManager.IsDLCInstalled(dlcName))
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
        }

        public static void PrefabSpawn(ConsoleEventArgs args)
        {
            Player localPlayer = Game.GetLocalPlayer();

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

                    if (characterComponent != null)
                    {
                        characterComponent.SetLevel(level);
                    }
                }

                string log = $"Spawned x<color=yellow>{amount}</color> of <color=cyan>{prefabObject.name}</color> (Level <color=orange>{level}</color>) in <color=#5f47d6>{(DateTime.Now - now).TotalMilliseconds}</color> ms";
                ZazaConsole.WriteLine(log);
                ZazaChat.WriteLine(log);
            }
            catch (Exception ex)
            {
                ZazaConsole.Exception(ex);
            }
        }

        public static void KillMobs(ConsoleEventArgs args)
        {
            Player localPlayer = Game.GetLocalPlayer();

            if (localPlayer == null)
            {
                args.ReplyError("You can only kill animals in-game.");
                return;
            }

            if (args.Length < 2 || args.Length > 3)
            {
                args.ReplyError("Syntax Error: /kill_mobs [distance] [(Optional)maxamount]");
                return;
            }

            float distance = args.TryParameterFloat(1, 10.0f);
            if (distance < 0.0f)
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
                result = Game.KillMobs(distance, maxAmount);
            }
            catch (Exception ex)
            {
                ZazaConsole.Exception(ex);
            }

            args.Reply($"Killed <color=yellow>{result}</color> animals.");
        }
    }
}