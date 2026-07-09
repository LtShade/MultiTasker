
using InControl;
using MultiTasker;
using System.Collections.Generic;


namespace MultiTasker
{

            //Define structure of JSON from settings

        public record ParallelRepairUpgrades
        {
            public int ParallelPathID;
            public string UpgradeID;
            public float BaseEfficiency;
        }

        public record ParallelEfficiencyUpgrades
        {
            public int ParallelPathID;
            public string UpgradeID;
            public float NewEfficiency;
        }

    public class ModConfig
    {

        // If true, many logs will be printed
        public bool Debug = true;
        // If true, all logs will be printed
        public bool Trace = true;

        public List<ParallelRepairUpgrades> RepairBays = new List<ParallelRepairUpgrades>();
        public List<ParallelEfficiencyUpgrades> BayEfficiencies = new List<ParallelEfficiencyUpgrades>();

        

        public void LogConfig()
        {
            Mod.Log.Info?.Log("=== MOD CONFIG BEGIN ===");
            Mod.Log.Info?.Log($"  DEBUG:{this.Debug} Trace:{this.Trace}");
            Mod.Log.Debug?.Log("---Configurations---");
            Mod.Log.Debug.Log($"Repair Paths: {this.RepairBays.Count}");
            Mod.Log.Debug.Log($"Efficiencies: {this.BayEfficiencies.Count}");
            Mod.Log.Debug?.Log("---Repair Bays---");
            foreach (var upgrade in this.RepairBays)
            {
                Mod.Log.Debug?.Log($"path: {upgrade.ParallelPathID} | id: {upgrade.UpgradeID} | base: {upgrade.BaseEfficiency}");
            }
            Mod.Log.Debug?.Log("---Effic. Bays---");
            foreach (var upgrade in this.BayEfficiencies)
            {
                Mod.Log.Debug?.Log($"path: {upgrade.ParallelPathID} | id: {upgrade.UpgradeID} | base: {upgrade.NewEfficiency}");
            }
            Mod.Log.Info?.Log("=== MOD CONFIG END ===");
        }

        public void Init()
        {
            Mod.Log.Debug?.Log(" == Initializing Configuration");
            Mod.Log.Debug?.Log(" == Configuration Initialized");
        }
    }
}
