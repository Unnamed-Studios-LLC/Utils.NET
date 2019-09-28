using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Utils.NET.Logging;

namespace Utils.NET.Modules
{
    public class ModularProgram
    {
        /// <summary>
        /// The static instance of the program
        /// </summary>
        public static ModularProgram Instance = new ModularProgram();
        
        /// <summary>
        /// Runs the program with given modules
        /// </summary>
        public static void Run(params Module[] modules)
        {
            for (int i = 0; i < modules.Length; i++)
                Instance.AddModule(modules[i]);

            Instance.LoadExternalModules();

            Log.Run();

            Instance.Stop();
        }

        /// <summary>
        /// Collection of all loaded modules
        /// </summary>
        private List<Module> Modules = new List<Module>();

        /// <summary>
        /// Adds a module to the program
        /// </summary>
        /// <param name="module"></param>
        public void AddModule(Module module)
        {
            lock (Modules)
            {
                Modules.Add(module);
                Log.Write("Starting Module: " + module.Name);
                module.Start();
            }
        }

        /// <summary>
        /// Loads any modules in external dll's within the modules directory
        /// </summary>
        private void LoadExternalModules()
        {
            string path = "Modules";
            if (!Directory.Exists(path)) return;

            foreach (var file in Directory.EnumerateFiles(path))
            {
                if (!file.EndsWith(".dll", StringComparison.Ordinal)) continue; // only load dlls
                LoadModuleDll(file);
            }
        }

        /// <summary>
        /// Loads a module type from a given DLL, then adds the module to the program
        /// </summary>
        /// <param name="path"></param>
        private void LoadModuleDll(string path)
        {
            var assembly = Assembly.LoadFrom(path);
            var moduleType = typeof(Module);
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsSubclassOf(moduleType)) continue; // exlude all types that aren't modules
                AddModule((Module)Activator.CreateInstance(type));
            }
        }

        /// <summary>
        /// Stops the program and all modules
        /// </summary>
        private void Stop()
        {
            lock (Modules)
            {
                for (int i = 0; i < Modules.Count; i++)
                {
                    var module = Modules[i];

                    Log.Write("Stopping Module: " + module.Name);
                    module.Stop();
                }
            }
        }
    }
}
