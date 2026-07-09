using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech.UI;
using MultiTasker.Helper;


namespace MultiTasker.Patches
{
	[HarmonyPatch(typeof(TaskTimelineWidget), "RefreshEntries", MethodType.Normal)]
	public static class TaskTimelineWidget_RefreshEntries
	{
		public static void Postfix(TaskTimelineWidget __instance, SimGameState ___Sim)
		{
				Mod.Log.Debug.Log("=== Initiating Work Time Estimates ===");
				//Get Efficiencies and set cumulative variables
				Dictionary<int,float> efficiencies = EfficienciesHelper.GetEfficiencies(___Sim);
				//I'm not happy with Stacks. Don't @ me.
				List<int> workerQueue = new List<int>{};
				//Expand queue to size and give 0s
				foreach (var item in efficiencies)
				{
					workerQueue.Add(0);
				}
				foreach (WorkOrderEntry workOrder in ___Sim.MechLabQueue)
				{
					Mod.Log.Debug.Log($"WO: {workOrder.Description}");
					//Set Earliest Worker to the location of the minimum value in Worker Queue
					int earliestWorker = workerQueue.IndexOf(workerQueue.Min());
					//Mod.Log.Debug.Log($"EW: {earliestWorker}");
					//Mod.Log.Debug.Log($"\n{string.Join(Environment.NewLine, workerQueue)}");
					TaskManagementElement tme = null;
					if(__instance.ActiveItems.TryGetValue(workOrder, out tme) && ___Sim.WorkOrderIsMechTech(workOrder.Type) && workOrder.GetRemainingCost() != 0)
						{
							//Set efficiency
							float eff = efficiencies[earliestWorker];
							int daysEstimate = (int)Math.Ceiling((double)workOrder.GetRemainingCost()/___Sim.MechTechSkill/eff);
							string efficiencyText = eff == 1 ? "" : $" at {eff*100}% Speed";
							int displayQueuePosition = earliestWorker + 1;

							Mod.Log.Debug.Log($"Days Math: {workOrder.GetRemainingCost()} / {___Sim.MechTechSkill} / {eff} = {daysEstimate}");
							Mod.Log.Debug.Log($"Days Estimate: {daysEstimate}");
							//Active Queues
							if (workerQueue[earliestWorker] == 0 )
							{
								workerQueue[earliestWorker] = daysEstimate;
								tme.UpdateItem(0);
								tme.daysText.SetText("{0} Day{1}", daysEstimate, (daysEstimate == 1) ? "" : "s");
								tme.subTitleText.SetText($"Bay {displayQueuePosition}{efficiencyText}"); 
							}
							else //Waiting on queue
							{
								tme.UpdateItem(workerQueue[earliestWorker]);
								daysEstimate = daysEstimate + workerQueue[earliestWorker];
								tme.daysText.SetText("{0} Day{1}", daysEstimate, (daysEstimate == 1) ? "" : "s");
								tme.subTitleText.SetText($"Waiting on Bay {displayQueuePosition}"); 
								//Pseudo pop/add to the workerQueue to anticipate the priorities shifting as the queue progresses
								workerQueue.RemoveAt(earliestWorker);
								workerQueue.Add(daysEstimate);
							}
						}
			}
			Mod.Log.Debug.Log("=== Ending Work Time Estimates ===");
		}
	}
}

	