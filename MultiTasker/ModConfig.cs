
using MultiTasker;


namespace MultiTasker
{
    public class ModConfig
    {

        // If true, many logs will be printed
        public bool Debug = false;
        // If true, all logs will be printed
        public bool Trace = false;


        public void LogConfig()
        {
            Mod.Log.Info?.Log("=== MOD CONFIG BEGIN ===");
            Mod.Log.Info?.Log($"  DEBUG:{this.Debug} Trace:{this.Trace}");
            Mod.Log.Info?.Log("=== MOD CONFIG END ===");
        }

        public void Init()
        {
            Mod.Log.Debug?.Log(" == Initializing Configuration");
            Mod.Log.Debug?.Log(" == Configuration Initialized");
        }
    }
}
