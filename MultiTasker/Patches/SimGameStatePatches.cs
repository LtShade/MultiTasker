
using System;
using System.Collections.Generic;
using BestHTTP;
using MultiTasker.Helper;



namespace MultiTasker.Patches
{
    [HarmonyPatch(typeof(SimGameState), "UpdateMechLabWorkQueue", MethodType.Normal)]

    static class SimGameState_UpdateMechLabWorkQueue
    {
        static void Prefix(SimGameState __instance, bool passDay)
        {
            //Debug logging all shipUpgrades detected
            foreach (var item in __instance.shipUpgrades)
            {
                Mod.Log.Debug.Log($"Detected: {item.Tags}");
            }

           Dictionary<int,float> efficiencies = EfficienciesHelper.GetEfficiencies(__instance);

            int j = Math.Min(efficiencies.Count,__instance.MechLabQueue.Count);
            //For each path that exists, update efficiency to highest allowed
            //Queue Position 0 (MechBay 1) is always 100%.
            for (int i = 1; i < j; i++)
            {
                if (!passDay)
                {
                    return;
                }
                float eff = efficiencies[i];
                if (i != 0)
                {
                    __instance.MechLabQueue[i].PayCost((int)Math.Ceiling((float)eff * __instance.MechTechSkill));
                }
                Mod.Log.Debug.Log($"Queue Position {i} Paid Cost: {(int)Math.Ceiling((float)eff * __instance.MechTechSkill)}");
            }
            if(efficiencies.Count == 0)
            {
                __instance.MechLabQueue[0].PayCost((int)Math.Ceiling(1 * (float)__instance.MechTechSkill));
                Mod.Log.Debug.Log($"||Fallback|| Queue Postion 0 Paid Cost: {(int)Math.Ceiling(1 * (float)__instance.MechTechSkill)}");
            }

        }
    }

}



