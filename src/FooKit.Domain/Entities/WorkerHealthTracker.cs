using System;

namespace MyProject.Domain.Entities
{
    public class WorkerHealthTracker
    {
        public bool IsWorkerRunning { get; set; }
        public DateTime? LastAffiliateSyncTime { get; set; }
    }
}
