namespace Zaza
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Reflection;
    using System.Globalization;
    using System.Collections.Generic;

    using global::Zaza.SDK;

    internal class ScriptHandler
    {
        public static void LoadScriptsFromDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                ZazaConsole.WriteLine("Folder not found.");
                return;
            }

            string[] files = Directory.GetFiles(directoryPath);

            if(files.Length == 0)
            {
                return;
            }

            ZazaConsole.WriteLine("Loading scripts..");

            foreach (string filePath in files)
            {
                ScriptHandler.LoadScript(filePath);
            }
        }

        public static void LoadScript(string scriptFile)
        {
            if (!File.Exists(scriptFile))
                throw new FileNotFoundException("File not found at given path.");

            if (!scriptFile.EndsWith(".dll"))
            {
                throw new BadImageFormatException("File is not a class library.");
            }

            string scriptName = Path.GetFileNameWithoutExtension(scriptFile);
            ZazaConsole.WriteLine($"Creating script environment for {scriptName}");

            ScriptRuntime scriptRuntime = new ScriptRuntime();
            scriptRuntime.Create();
            scriptRuntime.SetScriptName(scriptName);
            scriptRuntime.RunScript(scriptFile);
        }

        public static void UnloadScript(string scriptFile)
        {
            string scriptName = Path.GetFileNameWithoutExtension(scriptFile);
            Script script = ScriptManager.GetScripts().Find(x => x.Context.GetName() == scriptName);

            if(script != null)
            {
                ScriptHandler.UnloadScript(script.Context.ScriptManager.GetScriptRuntime());
            } else
            {
                throw new FileNotFoundException("File not found at given path.");
            }
        }

        public static void UnloadScript(ScriptRuntime scriptRuntime)
        {
            try
            {
                if (scriptRuntime != null)
                {
                    ZazaConsole.WriteLine($"Unloading script {scriptRuntime.GetScriptName()}");

                    scriptRuntime.Dispose();
                    scriptRuntime = null;
                } else
                {
                    ZazaConsole.Error("Invalid ScriptRuntime instance");
                }
            } catch (Exception ex)
            {
                ZazaConsole.Exception(ex);
            }
        }

        public static void UnloadScripts()
        {
            var runtimes = ScriptRuntime.GetScriptRuntimes();

            foreach (var runtime in runtimes)
            {
                ScriptHandler.UnloadScript(runtime.Value);
            }
        }
    }

    internal class ScriptRuntime : IDisposable
    {
        private static Dictionary<int, ScriptRuntime> ScriptRuntimes { get; set; } = new Dictionary<int, ScriptRuntime>();

        private ScriptManager ScriptManager { get; set; }

        private static readonly Random Random = new Random();

        private readonly int InstanceID;

        private string Name { get; set; }

        private AppDomain AppDomain { get; set; }

        static ScriptRuntime()
            { }

        internal ScriptRuntime()
            => this.InstanceID = Random.Next();

        internal int GetInstanceID()
            => this.InstanceID;

        internal void Create()
        {
            try
            {
                /* Unity does not really likes multiple domains
                this.AppDomain = AppDomain.CreateDomain($"ScriptDomain_{this.GetInstanceID()}", AppDomain.CurrentDomain.Evidence, new AppDomainSetup() { ApplicationBase = AppDomain.CurrentDomain.BaseDirectory });
                this.ScriptManager = (ScriptManager)this.AppDomain.CreateInstanceAndUnwrap(typeof(ScriptManager).Assembly.FullName, typeof(ScriptManager).FullName);
                */

                this.ScriptManager = new ScriptManager(this);
                ScriptRuntimes.Add(this.InstanceID, this);
            } catch (Exception ex)
            {
                ZazaConsole.Exception(ex);
            }
        }

        internal void SetScriptName(string name)
            => this.Name = name;

        internal string GetScriptName()
            => this.Name;

        internal bool IsValid()
            => this.AppDomain != null && this.ScriptManager != null;

        internal void RunScript(string scriptFile)
        {
            try
            {
                this.ScriptManager.LoadScript(scriptFile);
            } catch (Exception ex)
            {
                ZazaConsole.Exception(ex);
            }
        }

        internal static Dictionary<int, ScriptRuntime> GetScriptRuntimes()
            => ScriptRuntime.ScriptRuntimes;

        internal ScriptManager GetScriptManager()
            => this.ScriptManager;

        public void Dispose()
        {
            // AppDomain.Unload(this.AppDomain);

            if(this.IsValid())
            {
                this.AppDomain = null;
                this.ScriptManager = null;
            }

            if(ScriptRuntimes.ContainsKey(this.InstanceID))
                ScriptRuntimes.Remove(this.InstanceID);
        }
    }

    internal class ScriptManager : MarshalByRefObject
    {
        private static readonly List<Script> Scripts = new List<Script>();

        private static Dictionary<string, Assembly> Assemblies { get; set; } = new Dictionary<string, Assembly>();

        private readonly int InstanceID;

        private static readonly Random Random = new Random();

        private ScriptRuntime ScriptRuntime;

        // actually, domain-global
        // internal static ScriptManager GlobalManager { get; set; }

        public ScriptManager(ScriptRuntime scriptRuntime)
        {
            this.InstanceID = Random.Next();
            this.ScriptRuntime = scriptRuntime;
            // GlobalManager = this;

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            AppDomain.CurrentDomain.UnhandledException += (s, args) => { ZazaConsole.Exception(args.ExceptionObject as ScriptException); };
            AppDomain.CurrentDomain.AssemblyResolve += (s, args) =>
            {
                if (Assemblies.ContainsKey(args.Name))
                {
                    return Assemblies[args.Name];
                }

                return this.LoadScript(args.Name.Split(',')[0]);
            };
        }

        private static Assembly CreateScriptInternal(ScriptManager scriptManager, string assemblyFile)
        {
            if(Assemblies.ContainsKey(assemblyFile))
            {
                ZazaConsole.WriteLine($"Returning previously loaded assembly {Assemblies[assemblyFile].FullName}");
                return Assemblies[assemblyFile];
            }

            Assembly assembly = null;
            string assemblyName = Path.GetFileNameWithoutExtension(assemblyFile);

            Func<Type, bool> predicate = (t =>
                t != null && !t.IsAbstract && t.IsSubclassOf(typeof(Script)) && t.GetConstructor(Type.EmptyTypes) != null);

            IEnumerable<Type> definedTypes;

            try
            {
                assembly = Assembly.LoadFile(assemblyFile);
                Assemblies[assemblyFile] = assembly;

                ZazaConsole.WriteLine($"Loaded {assembly.FullName} into {AppDomain.CurrentDomain.FriendlyName}");
                definedTypes = assembly.GetTypes().Where(predicate);

                if (definedTypes.Count() != 0)
                {
                    foreach (Type type in definedTypes)
                    {
                        try
                        {
                            ZazaConsole.WriteLine($"Instantiating script instance.. {type.FullName}");

                            Script derivedScript = Activator.CreateInstance(type) as Script;
                            derivedScript.Context = new ScriptContext(assembly, scriptManager);

                            AddScript(derivedScript);
                        } catch (Exception ex)
                        {
                            ZazaConsole.Error($"Failed to instantiate instance of script {assemblyName}");
                            ZazaConsole.Exception(ex);
                        }
                    }
                } else
                {
                    throw new ScriptException("Library does not contains any class that inherits from Script.");
                }
            } catch (Exception ex)
            {
                ZazaConsole.Error($"Failed to load Assembly {assemblyName}");
                ZazaConsole.Exception(ex);
            }

            return assembly;
        }

#nullable enable
        private static Assembly? LoadScriptInternal(ScriptManager scriptManager, string assemblyFile)
        {
            try
            {
                return scriptManager.CreateScript(assemblyFile);
            } catch (Exception ex)
            {
                ZazaConsole.Exception(ex);
            }

            return null;
        }
#nullable disable

        internal Assembly CreateScript(string scriptFile)
            => CreateScriptInternal(this, scriptFile);

        internal Assembly LoadScript(string scriptFile)
            => LoadScriptInternal(this, scriptFile);

        internal int GetInstanceID()
            => this.InstanceID;

        internal static void AddScript(Script script)
        {
            if (!Scripts.Contains(script))
            {
                Scripts.Add(script);
            }
        }

        internal static void RemoveScript(Script script)
        {
            if (Scripts.Contains(script))
            {
                Scripts.Remove(script);
            }
        }

        internal static List<Script> GetScripts()
            => ScriptManager.Scripts;

        internal ScriptRuntime GetScriptRuntime()
            => this.ScriptRuntime;

        public void Tick()
        {
            foreach(Script script in Scripts)
            {
                script.OnTick();
            }
        }
    }

    internal class ScriptContext
    {
        internal Assembly Assembly { get; set; } = null;

        internal AssemblyName AssemblyName { get; set; } = null;

        internal ScriptManager ScriptManager { get; set; } = null;

        public ScriptContext(Assembly assembly, ScriptManager scriptManager)
        {
            this.Assembly = assembly;
            this.AssemblyName = assembly.GetName();

            this.ScriptManager = scriptManager;
        }

        public string GetName()
            => this.AssemblyName.Name;
    }

    public abstract class Script
    {
        internal ScriptContext Context { get; set; } = null;

        protected Player LocalPlayer
            => Game.GetLocalPlayer();

        public virtual void OnTick()
            { }

        protected string GetScriptName()
            => this.Context.GetName();

        /// <inheritdoc cref="ZazaConsole.WriteLine"/>
        protected void Log(string text)
            => ZazaConsole.WriteLine($"script: {this.GetScriptName()} " + text);

        /// <inheritdoc cref="ZazaConsole.WriteLine"/>
        protected void Error(string text)
            => ZazaConsole.Error($"script: {this.GetScriptName()} " + text);

        /// <summary>
        /// Prints exception to console.
        /// </summary>
        /// <param name="ex">Thrown exception.</param>
        protected void Exception(Exception ex)
            => ZazaConsole.Exception(ex);

        public static void RegisterScript(Script script)
            => ScriptManager.AddScript(script);

        public static void UnregisterScript(Script script)
            => ScriptManager.RemoveScript(script);
    }
}
