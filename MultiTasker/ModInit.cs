using ModTek.Public;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;


namespace MultiTasker
{

    public static class Mod
    {

        public const string HarmonyPackage = "us.ltshade.MultiTasker";
        public const string LogName = "used_dropship_salesman";
        public const string LogLabel = "USEDDROPSHIP";

        internal static NullableLogger Log = NullableLogger.GetLogger("MultiTasker", NullableLogger.TraceLogLevel);
        internal static string ModDir;
        internal static ModConfig Config;


        //public static readonly Random Random = new Random();

        public static void Init(string modDirectory, string settingsJSON)
        {
            ModDir = modDirectory;

            Exception settingsE = null;
            try
            {
                string settingsFile = Path.Combine(modDirectory, "settings.json");
                using StreamReader reader = new(settingsFile);
                string settingsText = reader.ReadToEnd();
                Mod.Config = JsonConvert.DeserializeObject<ModConfig>(settingsText);
            }
            catch (Exception e)
            {
                settingsE = e;
                Mod.Config = new ModConfig();
            }

            Assembly asm = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);
            Log.Info?.Log($"AssemblyVersion: {fvi.FileVersion}");

            // Initialize the mod settings
            Mod.Config.Init();

            Log.Debug?.Log($"ModDir is:{modDirectory}");
            Log.Debug?.Log($"mod.json settings are:({settingsJSON})");
            Mod.Config.LogConfig();

            if (settingsE != null)
            {
                Log.Info?.Log($"ERROR reading settings file! Error was: {settingsE}");
            }
            else
            {
                Log.Info?.Log($"INFO: No errors reading settings file.");
            }

            // Initialize modules
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), HarmonyPackage);
        }

public static void FinishedLoading()
        {
            Mod.Log.Trace?.Log("==== ModInit::FinishedLoading invoked.");
        }
    }
}
