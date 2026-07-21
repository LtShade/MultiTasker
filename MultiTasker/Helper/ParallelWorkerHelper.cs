
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiTasker.Helper
{
    	public class SimWorkOrder
	{
		public string GUID;
		public int elapsedDays;
		public int remainingCost;
        public int origIndex;
        public string mechOrder;
	}

	public class ParallelWorker
	{
		public string activeID;
		public int remainingCost;
        public int origIndex;
        public string mechOrder;
	};
    
    public class ParallelWorkerHelper
    {
        public static ParallelWorker CreateParallelWorker(string UID, int RemainingCost, int Index,string MechOrder)
        {
            ParallelWorker parallelWorker = new ParallelWorker
            {
                activeID = UID,
                remainingCost = RemainingCost,
                origIndex = Index,
                mechOrder = MechOrder
            };
            return parallelWorker;
        
        }
    }
}