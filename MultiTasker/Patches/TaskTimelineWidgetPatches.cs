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
			List<SimWorkOrder> simQueue = new List<SimWorkOrder>();
			List<SimWorkOrder> simOutput = new List<SimWorkOrder>();
			TaskManagementElement tme = null;
			//Build sim queue
			int simMLQIndex = 0;
			foreach (WorkOrderEntry workOrder in ___Sim.MechLabQueue)
			{
				if(__instance.ActiveItems.TryGetValue(workOrder, out tme) && ___Sim.WorkOrderIsMechTech(workOrder.Type) && workOrder.GetRemainingCost() != 0)
				{
					SimWorkOrder simWorkOrder = new SimWorkOrder
					{
						GUID = workOrder.GUID,
						elapsedDays =  0,
						remainingCost = workOrder.GetRemainingCost(),
						origIndex = simMLQIndex,
						mechOrder = workOrder.Description
					};
					simMLQIndex++;
					simQueue.Add(simWorkOrder);
				}
			}
			//Bail if nothing to simulate
			if(simQueue.Count ==0)
			{
				return;
			}


			//Build parallel working queues
			Mod.Log.Debug.Log("== Building and Populating Initial Parallel Queues for Sim ==");
			List<ParallelWorker> workerQueue = new List<ParallelWorker>();
			int parallelQueueSize = simQueue.Count > efficiencies.Count ? efficiencies.Count : simQueue.Count;
			for (int i = 0; i < parallelQueueSize; i++)
			{
				ParallelWorker queue = ParallelWorkerHelper.CreateParallelWorker(simQueue[i].GUID, simQueue[i].remainingCost, simQueue[i].origIndex, simQueue[i].mechOrder);
				workerQueue.Add(queue);
				Mod.Log.Debug.Log($"Pos: {i} | {queue.mechOrder} | {queue.remainingCost} | {efficiencies[i]}");
			}
			
			foreach (var item in simQueue)
			{
				Mod.Log.Debug.Log($"\t{item.remainingCost}");
			}
			
			Mod.Log.Debug.Log("== Simulation Begins ==");
			//Simulation Loop.
			bool hasWork = true;
			int parallelWorkers = workerQueue.Count;
			int simElapsedDays = 0;
			int nextIndex = parallelWorkers;
			int totalTasks = simQueue.Count;
			int mechTech = ___Sim.MechTechSkill;
			while (hasWork)
			{	
				foreach (var item in workerQueue)
				{
					Mod.Log.Debug.Log($"\t\t{item.mechOrder} | Eff Cost{(int)Math.Ceiling(item.remainingCost / efficiencies[workerQueue.IndexOf(item)])}");
				}
				Mod.Log.Debug.Log($"\tMechTech: {mechTech}");
				Mod.Log.Debug.Log("\tCheck Lowest");
				//Find the lowest effective cost
				int lowestEffCost = workerQueue.Min((ParallelWorker pW) => (int)Math.Ceiling(pW.remainingCost / efficiencies[workerQueue.IndexOf(pW)]));
				int lowestDays = (int)Math.Ceiling((double)lowestEffCost / mechTech);
				Mod.Log.Debug.Log($"\tLowest Days: {lowestDays}");
				Mod.Log.Debug.Log($"\tLowest Cost: {lowestEffCost}");
				Mod.Log.Debug.Log("\tParallel Queue Values");
				//Remove cost from active tasks
				//timeElapsed check to remove double elapsed accounting during loop
				bool timeElapsed = false;
				foreach (var task in workerQueue)
				{
					double workerEfficiency = efficiencies[workerQueue.IndexOf(task)];

					Mod.Log.Debug.Log($"\tPreRemaining Cost: {task.remainingCost}");
					Mod.Log.Debug.Log($"\tLowest Cost: {lowestEffCost}");
					if(task.remainingCost <= lowestEffCost)
					{
						simElapsedDays = timeElapsed ? simElapsedDays : simElapsedDays + lowestDays;
						timeElapsed = true;
						SimWorkOrder finishedTask = simQueue.Find((SimWorkOrder sWO)=> sWO.GUID == task.activeID);
						finishedTask.elapsedDays = simElapsedDays;
						Mod.Log.Debug.Log($"\tMech: {task.mechOrder} | Elapsed: {finishedTask.elapsedDays}");
					}
					//Remember that paid cost is always some multiple of MechTech skill, thus lowestEffCost is not reliable here.
					task.remainingCost = task.remainingCost - (int)Math.Ceiling(lowestDays*workerEfficiency*mechTech);
					Mod.Log.Debug.Log($"\tPostRemaining Cost: {task.remainingCost}");
				}
				
				Mod.Log.Debug.Log("\tAdd to Output List");
				//If any are complete add them to the output list with updated days
				foreach (var task in workerQueue.Where((ParallelWorker t) => t.remainingCost<=0))
				{
					SimWorkOrder finishedTask = simQueue.Find((SimWorkOrder sWO)=> sWO.GUID == task.activeID);
					simOutput.Add(finishedTask);
				}
				//Nuke the finished tasks
				Mod.Log.Debug.Log("\tFlush Finished Tasks");
				foreach (var item in workerQueue.Where(i=>i.remainingCost<=0))
				{
					Mod.Log.Debug.Log($"\tPopping Mech: {item.mechOrder} | {simElapsedDays}");
				}
				workerQueue.RemoveAll((ParallelWorker pW)=>pW.remainingCost<=0);


				//Refill worker queue
				Mod.Log.Debug.Log("\tRefill Worker Queue");
				for (int i = 0; i < parallelWorkers-workerQueue.Count; i++)
				{
					if(nextIndex <= totalTasks && nextIndex < simQueue.Count)
					{
						ParallelWorker newTask = ParallelWorkerHelper.CreateParallelWorker(simQueue[nextIndex].GUID, simQueue[nextIndex].remainingCost,simQueue[nextIndex].origIndex, simQueue[nextIndex].mechOrder );
						nextIndex++;
						workerQueue.Add(newTask);
					}
				}

				//Out of work, we're done here.
				Mod.Log.Debug.Log($"\t== Elapsed: {simElapsedDays}");
				Mod.Log.Debug.Log("\tOut of Work Check");
				if(workerQueue.Count == 0)
				{
					hasWork = false;
				}
			}

			//Correct any order issues caused by items finishing out of the initial sequence
			List<SimWorkOrder> sortedSimOutput = simOutput.OrderBy((SimWorkOrder sWO)=> sWO.origIndex).ToList();

			//Debug check sim output
			Mod.Log.Debug.Log("== Checking Simulation Output ==");
			Mod.Log.Debug.Log($"SimOutput: {sortedSimOutput.Count} | simQueue: {simQueue.Count}");
			foreach (var item in sortedSimOutput)
			{
				Mod.Log.Debug.Log($"\t {item.origIndex} | {item.elapsedDays}");
			}


			//Take simulation output and update Task TimelineWidget
			foreach (WorkOrderEntry workOrder in ___Sim.MechLabQueue)
			{
				tme = null;
				if(__instance.ActiveItems.TryGetValue(workOrder, out tme) && ___Sim.WorkOrderIsMechTech(workOrder.Type))
				{
					int simIndex = sortedSimOutput.FindIndex((SimWorkOrder sWO) => sWO.GUID == workOrder.GUID);
					SimWorkOrder activeWO = sortedSimOutput[simIndex];
					int effIndex = simIndex < parallelWorkers ? simIndex : 0;
					float eff = efficiencies[effIndex];
					string efficiencyText = eff == 1 ? "" : $" at {eff*100}% Speed";
					tme.daysText.SetText("{0} Day{1}", activeWO.elapsedDays, (activeWO.elapsedDays == 1) ? "" : "s");
					//Active tasks
					if(efficiencies.Any(e => e.Key == simIndex))
					{
						tme.subTitleText.SetText($"Bay {effIndex + 1}{efficiencyText}"); 
					}
					//Inactive tasks
					else
					{
						tme.subTitleText.SetText($"Waiting for Open Bay"); 
					}
				}
			}

			Mod.Log.Debug.Log("=== Ending Work Time Estimates ===");	
		}
	}
}

	