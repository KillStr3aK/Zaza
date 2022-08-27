namespace Zaza
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using UnityEngine;

    using global::Zaza.SDK;
    using static Terminal;
    using System.Linq;

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

            ZazaConsole.RegisterCommand("font_size", "[size] Set console output size", Commands.SetFontSize);

            ZazaConsole.RegisterCommand("coords", "Get current position", Commands.GetCoords);

            ZazaConsole.RegisterCommand("assemblies", "Get loaded assemblies", Commands.GetAssemblies);
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

        public static void GetCoords(ConsoleEventArgs args)
        {
            if (!Game.IsInGame())
            {
                args.ReplyError("You must be in-game.");
                return;
            }

            args.Reply($"{Game.GetLocalPlayer().transform.position}");
        }

        public static void GetAssemblies(ConsoleEventArgs args)
        {
            List<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

            args.Reply($"Loaded assemblies ({assemblies.Count} in total)");

            foreach(Assembly assembly in assemblies)
            {
                args.Reply($"{assembly.GetName().Name}");
            }
        }

        public static void SetFontSize(ConsoleEventArgs args)
        {
            if (args.Length != 2)
            {
                args.ReplyError("Syntax Error: /font_size [size]");
                return;
            }

            if (!int.TryParse(args[1], out int fontSize) && fontSize < 3)
            {
                args.ReplyError($"Parameter 1 is invalid!");
                return;
            }

            ZazaConsole.SetFontSize(fontSize);
            args.Reply( $"Changed Console font size to <color=yellow>{fontSize}</color>");
        }
    }
}