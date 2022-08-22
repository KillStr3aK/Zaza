namespace Zaza
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using UnityEngine;

    using global::Zaza.SDK;
    using static Terminal;

    internal sealed class Commands
    {
        internal static void Register()
        {
            ZazaConsole.RegisterCommand("quit", "Quit from the game.", async (args) =>
            {
                args.Reply("helo :D");
                await Task.Delay(500);
                Application.Quit();
            });

            ZazaConsole.RegisterCommand("prefab_dump", "[(Optional)filepath] Dump game prefabs.", Commands.PrefabDump);

            ZazaConsole.RegisterCommand("load_script", "[filename] Run C# script.", Commands.LoadScript);

            ZazaConsole.RegisterCommand("echo", "[text] Print to console", Commands.Echo);

            ZazaConsole.RegisterCommand("print", "[text] Print to console", Commands.Echo);

            ZazaConsole.RegisterCommand("connect", "[ip:port] Connect to server IP:Port", Commands.Connect);
        }

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

            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Zaza\\Scripts\\" + args[1].ToString() + ".dll";

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

        public static void Echo(ConsoleEventArgs args)
        {
            string buffer = "";

            foreach(var arg in args.Args)
            {
                buffer += arg + " ";
            }

            args.Reply(buffer);
        }

        public static void Connect(ConsoleEventArgs args)
        {
            if (args.Length != 2)
            {
                args.ReplyError("Syntax Error: /connect [ip:port]");
                return;
            }


        }
    }
}