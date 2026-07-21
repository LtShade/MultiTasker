
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiTasker.Helper
{
    public static class EfficienciesHelper
    {
        public static Dictionary<int, float> GetEfficiencies(SimGameState sgs)
        {

            //Set Efficiencies variable to populate what queue positions get which efficiencies
            Dictionary<int, float> efficiencies = new Dictionary<int, float>
            {
                //Position 0 is always 100% efficiency
                {0, 1}
            };
            //Build out baseline parallel workers and their efficiencies
            foreach (ParallelRepairUpgrades upgrade in Mod.Config.RepairBays)
            {
                if(sgs.shipUpgrades.Any((ShipModuleUpgrade u) => u.Tags.Any((string t) => t.Contains(upgrade.UpgradeID))))
                {
                    float eff = 0;
                    if(efficiencies.TryGetValue(upgrade.WorkerID, out eff))
                    {
                        efficiencies[upgrade.WorkerID] = Math.Max(eff, upgrade.BaseEfficiency);
                    }
                    else
                    {
                        efficiencies.Add(upgrade.WorkerID,upgrade.BaseEfficiency);
                    }
                } 
            }
            //Log it
            Mod.Log.Debug.Log($"Base Efficiencies: \n{string.Join(Environment.NewLine, efficiencies)}");

            //Find available efficiency upgrades and update efficiency values
            foreach ( ParallelEfficiencyUpgrades upgrade in Mod.Config.BayEfficiencies)
            {
                if(sgs.shipUpgrades.Any((ShipModuleUpgrade u) => u.Tags.Any((string t) => t.Contains(upgrade.UpgradeID))))
                {
                    float eff = 0;
                    if(efficiencies.TryGetValue(upgrade.WorkerID, out eff))
                    {
                        efficiencies[upgrade.WorkerID] = Math.Max(eff, upgrade.NewEfficiency);
                    }
                    else
                    {
                        efficiencies.Add(upgrade.WorkerID,upgrade.NewEfficiency);
                    }
                 
                } 
            }
            //Log it
            Mod.Log.Debug.Log($"Final Efficiencies: \n{string.Join(Environment.NewLine, efficiencies)}");
            return efficiencies;
        }
    }
}